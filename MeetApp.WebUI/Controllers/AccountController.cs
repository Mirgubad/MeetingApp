using MediatR;
using MeetApp.Business.Modules.AccountModule.Commands.EmailConfirmCommand;
using MeetApp.Business.Modules.AccountModule.Commands.RegisterCommand;
using MeetApp.Business.Modules.AccountModule.Commands.SigninCommand;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetApp.WebUI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("/signin.html")]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        [Route("/signin.html")]
        public async Task<IActionResult> Signin(SigninRequest request)
        {
            await _mediator.Send(request);
            return RedirectToAction(nameof(Index), "/");
        }


        [Route("/signup.html")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [Route("/signup.html")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            await _mediator.Send(request);
            return RedirectToAction(nameof(Signin));
        }

        [Route("/email-confirm")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmRequest request)
        {
            await _mediator.Send(request);
            return RedirectToAction(nameof(Signin));
        }

        [Route("/logout.html")]
        public async Task<IActionResult> Logout()
        {
            await Request.HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Signin));
        }

    }
}
