using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheConciergeOrg.Heating.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Zone {

   [JsonIgnore] public int OrderBy { get; set; }
   [JsonPropertyName("id")] public string Id { get; set; }
   [JsonPropertyName("name")]
   public string? Name {
      get => name; set {
         name = value;
         if (name.Contains("Living Room", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 1;
         else if (name.Contains("Main Bedroom", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 2;
         else if (name.Contains("Annex", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 3;
         else if (name.Contains("Kitchen", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 4;
         else if (name.Contains("Snugg", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 5;
         else if (name.Contains("Hallway", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 6;
         else if (name.Contains("Wellbeing", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 7;
         else if (name.Contains("Store Room", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 8;
         else if (name.Contains("Guest Bedroom", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 9;
         else if (name.Contains("Shower Room", StringComparison.CurrentCultureIgnoreCase))
            OrderBy = 10;
         else OrderBy = 99;
      }
   }
   [JsonPropertyName("type")] public ApplianceTypes Type { get; set; }
   [JsonPropertyName("meters")] public float SurfaceAreaMeterSquared { get; set; }
   [JsonPropertyName("status")] public Statuses Status { get; set; }
   [JsonPropertyName("power")] public bool IsSwitchOn { get; set; }

   [JsonPropertyName("ice")] public float AntiFrostTemperature { get; set; }
   [JsonPropertyName("eco")] public float EconomyModeTemperature { get; set; }
   [JsonPropertyName("comfort")] public float ComfortTemperature { get; set; }
   [JsonPropertyName("temp")] public float TargetTemperature { get; set; }

   private object _devices;
   private string? name;

   public object devices {

      get => _devices; set {

         _devices = value;

         try {

            if (_devices != null) {

               string? deviceAsString = _devices.ToString();

               if (!string.IsNullOrWhiteSpace(deviceAsString)) {

                  Dictionary<string, bool>? asObjects = JsonSerializer.Deserialize<Dictionary<string, bool>>(deviceAsString, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                  if (asObjects != null && asObjects.Count > 0) {

                     Devices = new();

                     foreach (var asObject in asObjects) {

                        Devices.Add(new Device() { id = asObject.Key, something = asObject.Value });

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
   [JsonIgnore] public List<Device>? Devices { get; set; }


   [JsonPropertyName("mode")] public ControlModes Mode { get; set; }
   public string[] schedule { get; set; }


   [MonitoringDescription("Anti-frost mode: This will keep the heating product at a minimum temperature.")]
   [JsonPropertyName("ice_mode")] public bool AntiFrostModeEnabled { get; set; }

   [MonitoringDescription("Open windows mode: Activates the anti-frost mode when the room temerature drops at least 4 degrees C within a 30 minute period.")]
   [JsonPropertyName("windows_open_mode")] public bool WindowsOpenModeEnabled { get; set; }

   [MonitoringDescription("Lock from the product: Activate or deactivate the product control panel. It can be deactivated by pressing and holding + and - from the device.")]
   [JsonPropertyName("block_local")] public bool BlockLocalAccessEnabled { get; set; }

   [MonitoringDescription("Lock remote: Activate or deactivate the product control panel. It can only be removed from the App.")]
   [JsonPropertyName("block_remote")] public bool BlockRemoteAccessEnabled { get; set; }

   [MonitoringDescription("Presence Detect mode: If your device has this option, the temperature drops by 1 degree C every hour to a maximum of 4 degrees C if it does not detect users in the room.")]
   [JsonPropertyName("pir_mode")] public bool PresenceDetectionEnabled { get; set; }

   [MonitoringDescription("Pilot wire: Locks the heating product to be managed with another system. If your device has this option.")]
   [JsonPropertyName("pilot_mode")] public bool PilotWiteEnabled { get; set; }

   [MonitoringDescription("Hotel mode: Locks the menu and man/auto keys, allowing the temperature to be raised and lowered.")]
   [JsonPropertyName("user_mode")] public bool UserModeEnabled { get; set; }


   // Radiator Only
   [MonitoringDescription("Early start: In auto mode, it anticipates the start-up top to 2 hours to reach the temperature at the set time.")]
   [JsonPropertyName("adelanto_enable")] public bool EarlyStartModeEnable { get; set; }

   // Towel Rail Only
   [MonitoringDescription("2-Hours boost function: Activates the heating elements in your product to maximum power, continously for 2 hours.")]
   [JsonPropertyName("two_hours")] public bool EarlyStartModeEnabled { get; set; }


   [JsonPropertyName("presence_mode")] public bool PresenceDetectionModeEnabled { get; set; }
   [JsonPropertyName("windowsMode")] public bool WindowsMode { get; set; }
   [JsonPropertyName("windows_open_status")] public bool WindowsOpenStatus { get; set; }
   [JsonPropertyName("windowsStatus")] public bool WindowsStatus { get; set; }
   public bool final { get; set; }




   public string lastUpdate { get; set; }
   public string last_device_id_update { get; set; }
   public string path { get; set; }

   public override string ToString() {
      return $@"{Name}, {TargetTemperature} {Mode} [{AntiFrostTemperature}, {EconomyModeTemperature}, {ComfortTemperature}], {Devices.Count} devices defined";
   }
   private string GetDebuggerDisplay() {
      return ToString();
   }

}
