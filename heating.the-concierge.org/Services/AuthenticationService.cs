using System.Diagnostics;

using Humanizer;

using Microsoft.AspNetCore.Mvc;

using RestSharp;

using TheConciergeOrg.Heating.Models;

namespace TheConciergeOrg.Heating.Services;

public class AuthenticationService : IHostedService, IDisposable {

   private Timer? _timer = null;

   private DateTime _FirstSignedinOn = DateTime.MinValue;
   private DateTime _LastSignedInOn = DateTime.MinValue;
   private DateTime _LastRefreshedOn = DateTime.MinValue;

   private DateTime _LastCheckedOKOn = DateTime.MinValue;
   private DateTime _LastCheckedFailedOn = DateTime.MinValue;

   private string _AccessToken = string.Empty;
   private string _AccessTokenObfuscated => !string.IsNullOrWhiteSpace(_AccessToken) ? $@"{_AccessToken[0..4]}....{_AccessToken[^4..^0]}" : "";

   private string _RefreshToken = string.Empty;
   private string _RefreshTokenObfuscated => !string.IsNullOrWhiteSpace(_RefreshToken) ? $@"{_RefreshToken[0..4]}....{_RefreshToken[^4..^0]}" : "";

   private DateTime _ExpiresOn = DateTime.MinValue;
   private TimeSpan _ExpiresIn => _ExpiresOn == DateTime.MinValue ? TimeSpan.MinValue : TimeSpan.FromSeconds((DateTime.Now - _ExpiresOn).TotalSeconds);


   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   public string auth => _AccessToken;
   public string AccessTokenObfuscated => _AccessTokenObfuscated;
   public string RefreshTokenObfuscated => _RefreshTokenObfuscated;

   public DateTime FirstSignedinOn => _FirstSignedinOn;
   public DateTime LastSignedInOn => _LastSignedInOn;
   public DateTime LastRefreshedOn => _LastRefreshedOn;

   public DateTime LastCheckedOKOn => _LastCheckedOKOn;
   public DateTime LastCheckedFailedOn => _LastCheckedFailedOn;

   public DateTime ExpiresOn => _ExpiresOn;
   public TimeSpan ExpiresIn => _ExpiresIn;


   public Task StartAsync(CancellationToken cancellationToken) {

      Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(StartAsync)}");

      _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

      return Task.CompletedTask;

   }

   private bool isWorking = false;

   private void DoWork(object? state) {

      if (isWorking) return;

      Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)}");

      isWorking = true;

      try {

         bool isExpiryDateLessThanNow = _ExpiresOn == DateTime.MinValue ? true : _ExpiresOn < DateTime.Now;

         bool isExpiryDateSoon = _ExpiresOn == DateTime.MinValue ? true : _ExpiresOn.AddMinutes(-10) <= DateTime.Now;

         bool isExpiryInTheFuture = _ExpiresOn == DateTime.MinValue ? false : _ExpiresOn.AddMinutes(-10) > DateTime.Now;

         bool doWeHaveAnExpiryToken = !string.IsNullOrWhiteSpace(_AccessToken);

         bool doWeHaveAnRefreshToken = !string.IsNullOrWhiteSpace(_RefreshToken);

         if (isExpiryDateLessThanNow || !doWeHaveAnExpiryToken || !doWeHaveAnRefreshToken) {

            Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)} : Logging in for access token");

            try {

               LoginCommand authenticationRequest = new();

               RestClient apiClient = new(authenticationRequest.url);

               RestRequest authenticationAPIRequest = new(authenticationRequest.action);

               authenticationAPIRequest.AddJsonBody(authenticationRequest);

               LoginResult? authenticationAPIResponse = null;
               RestResponse? restResponse = null;

               try {

                  // restResponse = apiClient.Post(authenticationAPIRequest);
                  Stopwatch sw = Stopwatch.StartNew();

                  authenticationAPIResponse = apiClient.Post<LoginResult>(authenticationAPIRequest);

                  sw.Stop();

                  if (authenticationAPIResponse != null && !string.IsNullOrWhiteSpace(authenticationAPIResponse.idToken)) {

                     _AccessToken = authenticationAPIResponse.idToken;
                     _RefreshToken = authenticationAPIResponse.refreshToken;
                     _ExpiresOn = authenticationAPIResponse.ExpiresOn;

                  }
                  else {

                     _LastCheckedFailedOn = DateTime.Now;

                  }

               }
               catch (Exception ex) {
                  _LastCheckedFailedOn = DateTime.Now;
                  Trace.WriteLine($@"{ex.Message}");
                  Debugger.Break();
               }

               isExpiryInTheFuture = _ExpiresOn == DateTime.MinValue ? false : _ExpiresOn.AddMinutes(-10) > DateTime.Now;

               doWeHaveAnExpiryToken = !string.IsNullOrWhiteSpace(_AccessToken);

               doWeHaveAnRefreshToken = !string.IsNullOrWhiteSpace(_RefreshToken);

               if (isExpiryInTheFuture && doWeHaveAnExpiryToken && doWeHaveAnRefreshToken) {

                  if (_FirstSignedinOn == DateTime.MinValue) {

                     _FirstSignedinOn = DateTime.Now;

                     _LastSignedInOn = DateTime.Now;

                  }
                  else {

                     _LastSignedInOn = DateTime.Now;

                  }

                  _LastCheckedOKOn = DateTime.Now;

                  Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)} : Logging in for access token granted: token {_AccessTokenObfuscated}, expires on {_ExpiresOn}, in {_ExpiresOn.Humanize()}");

               }
               else {

                  _LastCheckedFailedOn = DateTime.Now;

               }

            }
            catch (Exception ex) {
               _LastCheckedFailedOn = DateTime.Now;
               Trace.WriteLine($@"{ex.Message}");
               Debugger.Break();
            }

         }
         else if (isExpiryDateSoon && doWeHaveAnRefreshToken) {

            Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)} : Refreshing access token");

            try {

               RefreshLoginCommand authenticationRequest = new(_RefreshToken);

               RestClient apiClient = new(authenticationRequest.url);

               RestRequest authenticationAPIRequest = new(authenticationRequest.action);

               authenticationAPIRequest.AddJsonBody(authenticationRequest);

               LoginResult? authenticationAPIResponse = apiClient.Post<LoginResult>(authenticationAPIRequest);

               if (authenticationAPIResponse != null && !string.IsNullOrWhiteSpace(authenticationAPIResponse.idToken)) {

                  _AccessToken = authenticationAPIResponse.idToken;
                  _RefreshToken = authenticationAPIResponse.refreshToken;
                  _ExpiresOn = authenticationAPIResponse.ExpiresOn;

               }
               else {

                  _LastCheckedFailedOn = DateTime.Now;

               }

               isExpiryInTheFuture = _ExpiresOn == DateTime.MinValue ? false : _ExpiresOn.AddMinutes(-10) > DateTime.Now;

               doWeHaveAnExpiryToken = !string.IsNullOrWhiteSpace(_AccessToken);

               doWeHaveAnRefreshToken = !string.IsNullOrWhiteSpace(_RefreshToken);

               if (isExpiryInTheFuture && doWeHaveAnExpiryToken && doWeHaveAnRefreshToken) {

                  _LastRefreshedOn = DateTime.Now;

                  _LastCheckedOKOn = DateTime.Now;

                  Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)} : Logging in for access token granted: token {_AccessTokenObfuscated}, expires on {_ExpiresOn}, in {_ExpiresOn.Humanize()}");

               }
               else {

                  _LastCheckedFailedOn = DateTime.Now;

               }

            }
            catch (Exception ex) {
               _LastCheckedFailedOn = DateTime.Now;
               Trace.WriteLine($@"{ex.Message}");
               Debugger.Break();
            }

         }
         else if (isExpiryInTheFuture && doWeHaveAnExpiryToken && doWeHaveAnRefreshToken) {

            _LastCheckedOKOn = DateTime.Now;

            Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(DoWork)} : Access token still valid: token {_AccessTokenObfuscated}, expires on {_ExpiresOn}, in {_ExpiresOn.Humanize()}");

         }
         else {

            _LastCheckedFailedOn = DateTime.Now;

         }

      }
      catch (Exception ex) {
         _LastCheckedFailedOn = DateTime.Now;
         Trace.WriteLine($@"{ex.Message}");
         Debugger.Break();
      }

      isWorking = false;

   }

   public Task StopAsync(CancellationToken cancellationToken) {

      Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(StopAsync)}");

      _timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;

   }

   public void Dispose() {

      Console.WriteLine($@"{nameof(AuthenticationService)}.{nameof(Dispose)}");

      _timer?.Dispose();

   }

}

public static class AuthenticationEndPoints {

   public static void MapAuthenticationEndPoints(this IEndpointRouteBuilder routes) {

      var group = routes.MapGroup("/api/authentication").WithTags(nameof(Appliance));

      group.MapGet("/", ([FromServices] AuthenticationService authenticationService) => {

         string access = string.IsNullOrWhiteSpace(authenticationService.AccessTokenObfuscated) ? "n/a" : authenticationService.AccessTokenObfuscated;
         string refresh = string.IsNullOrWhiteSpace(authenticationService.RefreshTokenObfuscated) ? "n/a" : authenticationService.RefreshTokenObfuscated;

         string firstsignon = authenticationService.FirstSignedinOn == DateTime.MinValue ? "n/a" : authenticationService.FirstSignedinOn.ToString("dd MMM yyyy HH:mm");
         string lastsignon = authenticationService.LastSignedInOn == DateTime.MinValue ? "n/a" : authenticationService.LastSignedInOn.ToString("dd MMM yyyy HH:mm"); ;
         string lastrefeshedon = authenticationService.LastRefreshedOn == DateTime.MinValue ? "n/a" : authenticationService.LastRefreshedOn.ToString("dd MMM yyyy HH:mm"); ;

         string expireson = authenticationService.ExpiresOn == DateTime.MinValue ? "n/a" : authenticationService.ExpiresOn.ToString("dd MMM yyyy HH:mm");
         string expiresin = authenticationService.ExpiresIn == TimeSpan.MinValue ? "n/a" : authenticationService.ExpiresIn.Humanize();

         string lastOK = authenticationService.LastCheckedOKOn == DateTime.MinValue ? "n/a" : authenticationService.LastCheckedOKOn.Humanize();
         string lastFailed = authenticationService.LastCheckedFailedOn == DateTime.MinValue ? "n/a" : authenticationService.LastCheckedFailedOn.Humanize();

         return Results.Ok(new {

            access,
            refresh,
            firstsignon,
            lastsignon,
            lastrefeshedon,
            expireson,
            expiresin,
            lastOK,
            lastFailed

         });



      }).WithName("AuthenticationInformation").WithOpenApi();

   }

}
