using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("/{roomId}")]
        public IActionResult Call(String roomId)
        {
            ViewBag.roomId = roomId;
            ViewBag.userName = "Mirqubad Akbarov";
            return View();
        }


    }
}
