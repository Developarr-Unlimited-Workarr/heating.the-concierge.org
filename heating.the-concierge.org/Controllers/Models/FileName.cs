using System.Text.Json.Serialization;


namespace TheConciergeOrg.Heating.Controllers;


public class PowerCommand {

   public PowerCommand() { }

   public PowerCommand(string id, bool toState) => (applianceId, power) = (id, toState);

   [JsonIgnore]
   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   [JsonIgnore]
   public string url => $@"https://elife-prod.firebaseio.com";

   [JsonIgnore]
   public string action => $@"/devices/{applianceId}/data.json";

   [JsonIgnore]
   public string applianceId { get; set; } = string.Empty;

   public bool power { get; set; }

}

public class PowerResult {

   public PowerResult() { }

   public PowerResult(string id, bool state) => (applianceId, power) = (id, state);

   public string applianceId { get; set; } = string.Empty;
   public bool power { get; set; } = false;

}





public class TemperatureCommand {

   public TemperatureCommand() { }

   public TemperatureCommand(string id, double temperature) => (applianceId, temp) = (id, temperature);

   [JsonIgnore]
   public string key = "AIzaSyBi1DFJlBr9Cezf2BwfaT-PRPYmi3X3pdA";

   [JsonIgnore]
   public string url => $@"https://elife-prod.firebaseio.com";

   [JsonIgnore]
   public string action => $@"/devices/{applianceId}/data.json";

   [JsonIgnore]
   public string applianceId { get; set; } = string.Empty;

   public double temp { get; set; }
   public string mode { get; set; } = "manual";
   public bool power { get; set; } = true;

}

public class TemperatureResult {

   public double temp { get; set; }
   public string mode { get; set; }
   public bool power { get; set; }

}

public class Temperature {

   public Temperature() { }

   public Temperature(string id, double state) => (applianceId, temperature) = (id, state);

   public string applianceId { get; set; } = string.Empty;
   public double temperature { get; set; } = 0.0;

}
