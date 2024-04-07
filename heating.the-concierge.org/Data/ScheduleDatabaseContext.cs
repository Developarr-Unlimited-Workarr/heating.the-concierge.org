using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using TheConciergeOrg.Heating.Models;

namespace TheConciergeOrg.Heating.Data;

public class ScheduleDatabaseContext : DbContext {

   public ScheduleDatabaseContext(DbContextOptions<ScheduleDatabaseContext> options) : base(options) {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder) {

      var applianceConverter = new ValueConverter<Appliance?, string>(
          v => JsonConvert.SerializeObject(v),
          v => JsonConvert.DeserializeObject<Appliance>(v));

      var actionConverter = new ValueConverter<Action?, string>(
          v => JsonConvert.SerializeObject(v),
          v => JsonConvert.DeserializeObject<Action>(v));

      modelBuilder
          .Entity<Task>()
          .Property(e => e.Appliance)
          .HasConversion(applianceConverter);

      modelBuilder
          .Entity<Task>()
          .Property(e => e.Action)
          .HasConversion(actionConverter);

   }

   public DbSet<Task> Tasks { get; set; }

}

public class Task {

   [Key]
   public int Id { get; set; }

   public DateTime CreatedOn { get; set; }

   public Appliance? Appliance { get; set; } = null;

   public Action? Action { get; set; } = null;

   public DateTime ActionOn { get; set; }

}

public class Action {

   public bool? PowerOn { get; set; } = null;

   public bool? PowerOff { get; set; } = null;

   public bool? SetAuto { get; set; } = null;

   public bool? SetManual { get; set; } = null;

   public int? SetTemperatureTo { get; set; } = null;

}