using MediatR;
using MeetApp.Infrastructure.Entities.Membership;
using MeetApp.Infrastructure.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Web;

namespace MeetApp.Business.Modules.AccountModule.Commands.RegisterCommand
{
    internal class RegisterRequestHandler : IRequestHandler<RegisterRequest, MeetAppUser>
    {
        private readonly UserManager<MeetAppUser> _userManager;
        private readonly RoleManager<MeetAppRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterRequestHandler(
            UserManager<MeetAppUser> userManager,
            RoleManager<MeetAppRole> roleManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<MeetAppUser> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                throw new Exception($"{request.Email} already taken!");
            }

            user = new MeetAppUser
            {
                Fullname = request.Fullname,
                UserName = request.Username,
                Email = request.Email,
                EmailConfirmed = false
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in identityResult.Errors)
                {
                    sb.AppendLine($"{error.Code} : {error.Description}");
                }
                throw new Exception(sb.ToString());
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = HttpUtility.UrlEncode(token);

            string url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/email-confirm?token={token}&email={user.Email}";

            string message = $"Welcome to AZ Meet! Please confirm your email by clicking <a href='{url}'>here</a>";

            bool isEmailSent = await _emailService.SendMailAsync(request.Email, "AZ Meet Registration", message);

            if (!isEmailSent) throw new Exception("Email didn't send");

            return user;
        }
    }
}
