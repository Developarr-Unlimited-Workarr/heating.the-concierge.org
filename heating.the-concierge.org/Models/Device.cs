using System.Text.Json.Serialization;

namespace TheConciergeOrg.Heating.Models;

public class Device {
   public string id { get; set; }
   public bool something { get; set; }
}

public class Appliance {

   [JsonIgnore]
   public int OrderBy { get; set; }
   public string id { get; set; }
   public bool adelanto_enable { get; set; }
   public bool block_local { get; set; }
   public bool block_remote { get; set; }
   public bool check_updates_now { get; set; }
   public bool error_heating { get; set; }
   public bool error_low_wifi { get; set; }
   public bool error_temperature_probe { get; set; }
   public bool factory_test { get; set; }
   public bool ice_mode { get; set; }
   public bool legionella { get; set; }
   public bool pilot_mode { get; set; }
   public bool pir_mode { get; set; }
   public bool pir_status { get; set; }
   public bool power { get; set; }
   public bool test_factory { get; set; }
   public bool two_hours { get; set; }
   public bool user_mode { get; set; }
   public bool windows_open_mode { get; set; }
   public bool windows_open_status { get; set; }
   public float comfort_minus { get; set; }
   public float comfort { get; set; }
   public float eco { get; set; }
   public float ice { get; set; }
   public float kwh_price { get; set; }
   public float temp { get; set; }
   public float temp_calc { get; set; }
   public float temp_probe { get; set; }
   public float um_max_temp { get; set; }
   public float um_min_temp { get; set; }
   public int backlight { get; set; }
   public int backlight_on { get; set; }
   public int evolution { get; set; }
   public int gmt { get; set; }
   public int legionella_conf { get; set; }
   public int mgmt_modules { get; set; }
   public int nominal_power { get; set; }
   public int schedule_day { get; set; }
   public int schedule_hour { get; set; }
   public decimal last_sync_datetime_app { get; set; }
   public long last_sync_datetime_device { get; set; }
   public long last_sync_datetime_tariff { get; set; }
   public long pir_datetime { get; set; }
   public string check_updates_time { get; set; }
   public string color { get; set; }
   public string kwh_tariff_id { get; set; }
   public string mode { get; set; }
   public string name { get; set; }
   public string product_version { get; set; }
   public string status { get; set; }
   public string type { get; set; }
   public string um_password { get; set; }
   public string[] schedule { get; set; }
}
