using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

using RestSharp;

using TheConciergeOrg.Heating.Models;
using TheConciergeOrg.Heating.Services;
namespace TheConciergeOrg.Heating.Controllers;

public static class ApplianceEndpoints {

   public static void MapApplianceEndpoints(this IEndpointRouteBuilder routes) {

      var group = routes.MapGroup("/api/appliance").WithTags(nameof(Appliance));

      group.MapGet("/{id}", (string id, [FromServices] AuthenticationService authenticationService) => {

         ApplianceStatus? applianceStatus = null;
         try {

            string key = authenticationService.key;
            string auth = authenticationService.auth;

            ApplianceStatusCommand statusCommand = new(id);

            RestClient apiClient = new(statusCommand.url);
            apiClient.AddDefaultQueryParameter("key", key);
            apiClient.AddDefaultQueryParameter("auth", auth);

            RestRequest statusRequest = new(statusCommand.action);

            try {

               applianceStatus = new ApplianceStatus(id, apiClient.Get<Appliance>(statusRequest));

            }
            catch (Exception ex) {
               Debugger.Break();
            }

         }
         catch (Exception) {
            Debugger.Break();
         }

         return Results.Json(data: applianceStatus);

      }).WithName("GetApplianceStatus").WithOpenApi();

      group.MapPost("/{id}/on", (string id) => {
         return SetPower(id, true);
      }).WithName("PatchApplianceByIdOn").WithOpenApi();

      group.MapPost("/{id}/off", (string id) => {
         return SetPower(id, false);
      }).WithName("PatchApplianceByIdOff").WithOpenApi();

      group.MapPost("/{id}/temperature/{temperature}", (string id, double temperature) => {
         return SetTemperature(id, temperature);
      }).WithName("PatchApplianceByIdTemperature").WithOpenApi();

   }

   private static IResult SetPower(string id, bool setPowerStateTo) {
      try {

         LoginCommand loginCommand = new();

         RestClient apiClient = new(loginCommand.url);

         RestRequest loginRequest = new(loginCommand.action);
         loginRequest.AddJsonBody(loginCommand);

         LoginResult? loginResult = apiClient.Post<LoginResult>(loginRequest);

         if (loginResult != null && !string.IsNullOrWhiteSpace(loginResult.idToken)) {

            try {

               PowerCommand powerCommand = new(id, setPowerStateTo);

               apiClient = new(powerCommand.url);

               RestRequest powerRequest = new(powerCommand.action);
               powerRequest.AddQueryParameter("key", powerCommand.key);
               powerRequest.AddQueryParameter("auth", loginResult.idToken);
               powerRequest.AddJsonBody(powerCommand);

               try {

                  PowerResult? powerResult = apiClient.Patch<PowerResult>(powerRequest);

                  if (powerResult != null && powerResult.power == setPowerStateTo) {
                     return Results.Json(data: new PowerResult(id, powerResult.power), contentType: "application/json", statusCode: StatusCodes.Status200OK);
                  }
                  else {
                     return Results.Problem(detail: "Unknown reason not changed", statusCode: StatusCodes.Status304NotModified);
                  }

               }
               catch (Exception ex) {
                  Trace.WriteLine(ex.Message);
                  Debugger.Break();
                  return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
               }

            }
            catch (Exception ex) {
               Trace.WriteLine(ex.Message);
               Debugger.Break();
               return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }

         }
         else
            return Results.Problem(detail: "Unable to authenticate and authorise user", statusCode: StatusCodes.Status500InternalServerError);

      }
      catch (Exception ex) {
         Trace.WriteLine(ex.Message);
         Debugger.Break();
         return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
      }
   }

   private static IResult SetTemperature(string id, double setTemperatureTo) {

      try {

         LoginCommand loginCommand = new();

         RestClient apiClient = new(loginCommand.url);

         RestRequest loginRequest = new(loginCommand.action);
         loginRequest.AddJsonBody(loginCommand);

         LoginResult? loginResult = apiClient.Post<LoginResult>(loginRequest);

         if (loginResult != null && !string.IsNullOrWhiteSpace(loginResult.idToken)) {

            try {

               TemperatureCommand temperatureCommand = new(id, setTemperatureTo);

               apiClient = new(temperatureCommand.url);

               RestRequest temperatureRequest = new(temperatureCommand.action);
               temperatureRequest.AddQueryParameter("key", temperatureCommand.key);
               temperatureRequest.AddQueryParameter("auth", loginResult.idToken);
               temperatureRequest.AddJsonBody(temperatureCommand);

               try {

                  TemperatureResult? temperatureResult = apiClient.Patch<TemperatureResult>(temperatureRequest);

                  if (temperatureResult != null && temperatureResult.temp == setTemperatureTo) {
                     return Results.Json(data: new Temperature(id, temperatureResult.temp), contentType: "application/json", statusCode: StatusCodes.Status200OK);
                  }
                  else {
                     return Results.Problem(detail: "Unknown reason not changed", statusCode: StatusCodes.Status304NotModified);
                  }

               }
               catch (Exception ex) {
                  Trace.WriteLine(ex.Message);
                  Debugger.Break();
                  return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
               }

            }
            catch (Exception ex) {
               Trace.WriteLine(ex.Message);
               Debugger.Break();
               return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }

         }
         else
            return Results.Problem(detail: "Unable to authenticate and authorise user", statusCode: StatusCodes.Status500InternalServerError);

      }
      catch (Exception ex) {
         Trace.WriteLine(ex.Message);
         Debugger.Break();
         return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
      }
   }

}

public class ApplianceStatusCommand {

   public ApplianceStatusCommand() { }

   public ApplianceStatusCommand(string id) => (applianceId) = (id);

   [JsonIgnore]
   public string url => $@"https://elife-prod.firebaseio.com";

   [JsonIgnore]
   public string action => $@"/devices/{applianceId}/data.json";

   [JsonIgnore]
   public string applianceId { get; set; } = string.Empty;

   public bool power { get; set; }

}

public class ApplianceStatus {

   public ApplianceStatus() { }

   public ApplianceStatus(string? applianceId, Appliance? appliance) => (id, Appliance) = (applianceId, appliance);

   [JsonIgnore]
   public Appliance? Appliance { get; set; } = null;

   public string? id { get; set; } = null;
   public string? name => Regex.Replace(Appliance == null ? "" : Appliance.name, "\\([^)]*\\)", "").Trim();
   public bool? power => Appliance?.power;

   public string? status => Appliance?.status; // ice
   public string? mode => Appliance?.mode; // manual

   public float? target_temp => Appliance?.temp; // target temperature
   public float? actual_temp => Appliance?.temp_probe; // actual temperature


   public float? ice_set_temp => Appliance?.ice; // 7
   public float? eco_set_temp => Appliance?.eco; // 16
   public float? comfort_set_temp => Appliance?.comfort; // 19

}