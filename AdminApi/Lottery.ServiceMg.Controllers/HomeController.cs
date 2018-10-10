using Lottery.AdminApi.Model;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Lottery.ServiceMg.Controllers
{
    [Area("mg")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
