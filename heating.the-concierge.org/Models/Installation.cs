using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheConciergeOrg.Heating.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Installation {

   private object? _zones;

   public string Id { get; set; } = string.Empty;
   [JsonPropertyName("userid")] public string UserId { get; set; } = string.Empty;
   [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

   [JsonPropertyName("power")] public bool Power { get; set; }

   [JsonPropertyName("cost_limit")] public float CostLimit { get; set; }
   [JsonPropertyName("currency")] public string Currency { get; set; } = string.Empty;

   [JsonPropertyName("image_path")] public string ImageUrl { get; set; } = string.Empty;

   [JsonPropertyName("location")] public string Location { get; set; } = string.Empty;
   [JsonPropertyName("latitude")] public float Latitude { get; set; }
   [JsonPropertyName("longitude")] public float Longitude { get; set; }
   [JsonPropertyName("timeZone")] public int TimeZone { get; set; }
   [JsonPropertyName("timeZoneId")] public string TimeZoneId { get; set; } = string.Empty;

   [JsonPropertyName("zones")]
   public object zones {

      get => _zones; set {

         _zones = value;

         try {

            if (_zones != null) {

               string? zoneAsString = zones.ToString();

               if (!string.IsNullOrWhiteSpace(zoneAsString)) {
                  Dictionary<string, object>? asObjects = JsonSerializer.Deserialize<Dictionary<string, object>>(zoneAsString, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                  if (asObjects != null && asObjects.Count > 0) {

                     Zones = new();

                     foreach (var asObject in asObjects) {

                        try {

                           if (asObject.Value != null) {

                              string? valueAsString = asObject.Value.ToString();

                              if (valueAsString != null && !string.IsNullOrWhiteSpace(valueAsString)) {

                                 try {

                                    Zone? zone = JsonSerializer.Deserialize<Zone>(valueAsString, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                                    if (zone != null) {

                                       Zones.Add(zone);

                                    }

                                 }
                                 catch (Exception ex) {
                                    Trace.WriteLine(ex.Message);
                                    Debugger.Break();
                                 }

                              }

                           }

                        }
                        catch (Exception ex) {
                           Trace.WriteLine(ex.Message);
                           Debugger.Break();
                        }

                     }

                  }

               }

            }

         }
         catch (Exception ex) {
            Trace.WriteLine(ex.Message);
            Debugger.Break();
         }

      }

   }


   public List<Zone>? Zones { get; set; } = null;

   public List<Appliance>? Appliances { get; set; }

   public override string ToString() {
      return $@"{Name}, {Location} [{Latitude}, {Longitude}], {TimeZoneId} [{TimeZone}], {Zones.Count} zones defined";
   }
   private string GetDebuggerDisplay() {
      return ToString();
   }
}
