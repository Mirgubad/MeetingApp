using MediatR;

namespace MeetApp.Business.Modules.AccountModule.Commands.SigninCommand
{
    public class SigninRequest : IRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
