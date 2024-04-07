using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using TheConciergeOrg.Heating.Data;

namespace TheConciergeOrg.Heating.Pages;

public class IndexModel : PageModel {

   private readonly ScheduleDatabaseContext _context;

   public IndexModel(ScheduleDatabaseContext context) {

      _context = context;

   }

   public IList<Data.Task> Tasks { get; set; }

   public void OnGet() {

      _context.Tasks.ExecuteDelete();
      //_context.Tasks.Add(new Data.Task() { CreatedOn = DateTime.UtcNow, ActionOn = DateTime.UtcNow.AddHours(1), Appliance = new() { name = "Living Room" } });

      _context.SaveChanges();

      Tasks = _context.Tasks.ToList();

   }

}