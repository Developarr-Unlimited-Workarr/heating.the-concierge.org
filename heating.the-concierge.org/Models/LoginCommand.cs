using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using RestSharp;

namespace TheConciergeOrg.Heating.Models;

public static class Heating {

   public static async Task<UserDetails?> Authenticate() {

      try {

         LoginCommand authenticationRequest = new();

         RestClient apiClient = new(authenticationRequest.url);

         RestRequest authenticationAPIRequest = new(authenticationRequest.action);

         authenticationAPIRequest.AddJsonBody(authenticationRequest);

         LoginResult? authenticationAPIResponse = await apiClient.PostAsync<LoginResult>(authenticationAPIRequest);

         if (authenticationAPIResponse != null && !string.IsNullOrWhiteSpace(authenticationAPIResponse.idToken)) {

            AccountInformationRequest accountInformationRequest = new(authenticationAPIResponse);

            RestRequest accountinformationAPIRequest = new(accountInformationRequest.action);

            AccountInformationResponse? accountInformationResponse = await apiClient.PostAsync<AccountInformationResponse>(accountinformationAPIRequest);

            if (accountInformationResponse != null && accountInformationResponse.users.Length == 1) {
               return new UserDetails(authenticationAPIResponse, accountInformationResponse.users[0]);
            }

         }

      }
      catch (Exception ex) {
         Trace.WriteLine(ex.Message); Debugger.Break();
      }

      return null;

   }

   public static async Task<List<Installation>?> Installations(UserDetails userDetails) {

      List<Installation>? installationList = null;

      try {

         RestClient apiClient = new("https://elife-prod.firebaseio.com");

         RestRequest request = new("/installations2.json");

         request.AddQueryParameter("key", "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA");
         request.AddQueryParameter("auth", userDetails.Authentication.idToken);
         request.AddQueryParameter("orderBy", "\"userid\"");
         request.AddQueryParameter("equalTo", $@"""{userDetails.Authentication.localId}""");

         RestResponse response = apiClient.Get(request);

         if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content)) {


            Dictionary<string, Installation>? installations = JsonSerializer.Deserialize<Dictionary<string, Installation>>(response.Content);

            if (installations != null && installations.Count > 0) {

               installationList = new();

               foreach (KeyValuePair<string, Installation> installation in installations) {

                  try {

                     string id = installation.Key;

                     Installation home = installation.Value;
                     home.Id = id;

                     if (home.Zones != null && home.Zones.Count > 0) {

                        for (int zoneNumber = 0; zoneNumber < home.Zones.Count; zoneNumber++) {

                           try {

                              foreach (Device device in home.Zones[zoneNumber].Devices) {

                                 try {

                                    RestRequest deviceRequest = new($@"/devices/{device.id}/data.json");

                                    deviceRequest.AddQueryParameter("key", "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA");
                                    deviceRequest.AddQueryParameter("auth", userDetails.Authentication.idToken);

                                    RestResponse restResponse = apiClient.Get(deviceRequest);

                                    if (restResponse != null && !string.IsNullOrWhiteSpace(restResponse.Content)) {

                                       try {

                                          Appliance? appliance = JsonSerializer.Deserialize<Appliance>(restResponse.Content);

                                          if (appliance != null) {

                                             if (installations[installation.Key].Appliances == null) installations[installation.Key].Appliances = new();

                                             appliance.OrderBy = home.Zones[zoneNumber].OrderBy;
                                             appliance.id = device.id;

                                             installations[installation.Key].Appliances.Add(appliance);

                                          }

                                       }
                                       catch (Exception ex) {
                                          Trace.WriteLine(ex.Message); Debugger.Break();
                                       }

                                    }

                                 }
                                 catch (Exception ex) {
                                    Trace.WriteLine(ex.Message); Debugger.Break();
                                 }

                              }

                           }
                           catch (Exception ex) {
                              Trace.WriteLine(ex.Message); Debugger.Break();
                           }

                        }

                     }

                     installationList.Add(home);

                  }
                  catch (Exception ex) {
                     Trace.WriteLine(ex.Message); Debugger.Break();
                  }

               }

               // Debugger.Break();

            }
            else Debugger.Break();

         }

      }
      catch (Exception ex) {
         Trace.WriteLine(ex.Message); Debugger.Break();
      }

      return installationList;

   }


}

public class LoginCommand {

   [JsonIgnore]
   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   [JsonIgnore]
   public string url => $@"https://www.googleapis.com/identitytoolkit/v3/relyingparty";

   [JsonIgnore]
   public string action => $@"/verifyPassword?key={key}";

   public string email { get; set; } = "automation_d82jdy@24hollandroad.land";
   public string password { get; set; } = "5#eh8w@9XZLR54IfL6%6";
   public bool returnSecureToken { get; set; } = true;

}

public class RefreshLoginCommand {

   public RefreshLoginCommand(string refreshToken) { refresh_token = refreshToken; }

   [JsonIgnore]
   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   [JsonIgnore]
   public string url => $@"https://securetoken.googleapis.com/v1";

   [JsonIgnore]
   public string action => $@"/token?key={key}";

   public string grant_type { get; set; } = "refresh_token";
   public string refresh_token { get; set; } = "5#eh8w@9XZLR54IfL6%6";

}

public class LoginResult {
   public string kind { get; set; }
   public string localId { get; set; }
   public string email { get; set; }
   public string displayName { get; set; }
   public string idToken { get; set; }
   public bool registered { get; set; }
   public string refreshToken { get; set; }


   public int expiresIn { set { ExpiresOn = DateTime.Now.AddSeconds(value); } }

   [JsonIgnore]
   public DateTime ExpiresOn { get; private set; }

}


public class AccountInformationRequest {

   public AccountInformationRequest() { }

   public AccountInformationRequest(LoginResult authenticationResponse) {
      AuthenticationResponse = authenticationResponse;
   }

   [JsonIgnore]
   public LoginResult AuthenticationResponse { get; set; }

   [JsonIgnore]
   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   [JsonIgnore]
   public string url => $@"https://www.googleapis.com/identitytoolkit/v3/relyingparty/";

   [JsonIgnore]
   public string action => $@"/getAccountInfo?key={key}&idToken={AuthenticationResponse.idToken}";

}


public class AccountInformationResponse {
   public string kind { get; set; }
   public User[] users { get; set; }
}

public class User {
   public string localId { get; set; }
   public string email { get; set; }
   public string passwordHash { get; set; }
   public bool emailVerified { get; set; }
   public long passwordUpdatedAt { get; set; }
   public Provideruserinfo[] providerUserInfo { get; set; }
   public string validSince { get; set; }
   public string lastLoginAt { get; set; }
   public string createdAt { get; set; }
   public DateTime lastRefreshAt { get; set; }
}

public class Provideruserinfo {
   public string providerId { get; set; }
   public string federatedId { get; set; }
   public string email { get; set; }
   public string rawId { get; set; }
}


public class UserDetails {

   public UserDetails(LoginResult authentication, User user) => (Authentication, User) = (authentication, user);

   public LoginResult Authentication { get; set; }

   public User User { get; set; }

}

