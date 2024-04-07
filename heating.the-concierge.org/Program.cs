using Microsoft.EntityFrameworkCore;

using TheConciergeOrg.Heating.Controllers;
using TheConciergeOrg.Heating.Data;
using TheConciergeOrg.Heating.Services;

namespace TheConciergeOrg.Heating;

public class Program {

   public static void Main(string[] args) {

      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddDbContext<ScheduleDatabaseContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("ScheduleDatabaseContext")));

      builder.Services.AddSingleton<AuthenticationService>();
      builder.Services.AddHostedService<AuthenticationService>(provider => provider.GetService<AuthenticationService>());

      builder.Services.AddRazorPages();

      builder.Services.AddEndpointsApiExplorer();

      builder.Services.AddSwaggerGen();

      var app = builder.Build();

      using (var scope = app.Services.CreateScope()) {
         var services = scope.ServiceProvider;
         var context = services.GetRequiredService<ScheduleDatabaseContext>();
         context.Database.EnsureCreated();
      }


      if (!app.Environment.IsDevelopment()) {
         app.UseExceptionHandler("/Error");
         app.UseHsts();
      }

      app.UseSwagger();
      app.UseSwaggerUI();

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.MapRazorPages();

      app.MapAuthenticationEndPoints();

      app.MapApplianceEndpoints();

      app.Run();

   }

}