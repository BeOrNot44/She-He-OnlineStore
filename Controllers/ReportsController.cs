using Microsoft.AspNetCore.Mvc;

namespace She___He_Store.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Sales()
        {
            return View();
        }
        public IActionResult UserActivity()
        {
            return View();
        }
        public IActionResult WebsitePerformance()
        {
            return View();
        }

    }
}
