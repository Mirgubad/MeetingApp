using MediatR;
using MeetApp.Infrastructure.Entities.Membership;

namespace MeetApp.Business.Modules.AccountModule.Commands.RegisterCommand
{
    public class RegisterRequest : IRequest<MeetAppUser>
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
