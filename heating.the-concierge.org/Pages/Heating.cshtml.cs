using Microsoft.AspNetCore.Mvc.RazorPages;

using TheConciergeOrg.Heating.Models;

namespace TheConciergeOrg.Heating.Pages;

public class HeatingModel : PageModel {

   public UserDetails? LoggedInUser { get; set; }

   public List<Appliance>? Appliances { get; set; }

   public HeatingModel() {
   }

   public void OnGet() {

      LoggedInUser = Models.Heating.Authenticate().Result;

      if (LoggedInUser != null) {

         List<Installation>? Installations = Models.Heating.Installations(LoggedInUser).Result;

         if (Installations != null && Installations.Count > 0) {

            foreach (Installation installation in Installations) {

               if (installation.Appliances != null && Installations.Count > 0) {

                  if (Appliances == null) Appliances = new();

                  Appliances.AddRange(installation.Appliances);

               }

            }

         }

      }

   }

}