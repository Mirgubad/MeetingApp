using MediatR;
using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace MeetApp.Business.Modules.AccountModule.Commands.SigninCommand
{
    internal class SigninRequestHandler : IRequestHandler<SigninRequest>
    {
        private readonly SignInManager<MeetAppUser> _signInManager;
        private readonly UserManager<MeetAppUser> _userManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        public SigninRequestHandler(
            SignInManager<MeetAppUser> signInManager,
            UserManager<MeetAppUser> userManager,
            IActionContextAccessor actionContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _actionContextAccessor = actionContextAccessor;
        }
        public async Task Handle(SigninRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new Exception($"{request.Email} user not found!");

            var checkResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (!checkResult.Succeeded)
                throw new Exception($"Username or Password is incorrect!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await _actionContextAccessor.ActionContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                });
        }
    }
}
