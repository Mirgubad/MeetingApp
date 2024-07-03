using MediatR;
using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.AspNetCore.Identity;

namespace MeetApp.Business.Modules.AccountModule.Commands.EmailConfirmCommand
{
    internal class EmailConfirmRequestHandler : IRequestHandler<EmailConfirmRequest>
    {
        private readonly UserManager<MeetAppUser> _userManager;

        public EmailConfirmRequestHandler(UserManager<MeetAppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task Handle(EmailConfirmRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _userManager.ConfirmEmailAsync(user, request.Token);
        }
    }
}
