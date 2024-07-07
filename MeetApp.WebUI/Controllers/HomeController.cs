using MediatR;
using MeetApp.Business.Modules.MeetingModule.Commands.MeetingCreateCommand;
using MeetApp.Business.Modules.MeetingModule.Commands.MeetingJoinCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetApp.WebUI.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        public HomeController(
            ILogger<HomeController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;

        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(MeetingCreateRequest request)
        {
            var response = await _mediator.Send(request);
            TempData["nickName"] = response.NickName;
            return RedirectToAction(nameof(Call), new { roomId = response.MeetingId });
        }

        [Authorize]
        [HttpGet("meet/new")]
        public async Task<IActionResult> CreateMeet(MeetingCreateRequest request)
        {
            var response = await _mediator.Send(request);
            return RedirectToAction(nameof(Call), new { roomId = response.MeetingId });
        }

        [Authorize]
        [HttpGet("meet/{roomId}")]
        public async Task<IActionResult> Call([FromRoute] string roomId, MeetingJoinRequest request)
        {
            request.MeetingId = roomId;

            TempData.Keep("nickName");
            var response = await _mediator.Send(request);
            response.Nickname = TempData["nickName"];
            string shortChatId = response.MeetingId.Substring(0, 6);
            ViewBag.ShortChatId = shortChatId;
            return View(response);
        }

    }
}
