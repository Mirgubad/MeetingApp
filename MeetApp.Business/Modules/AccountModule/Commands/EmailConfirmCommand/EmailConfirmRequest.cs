using MediatR;

namespace MeetApp.Business.Modules.AccountModule.Commands.EmailConfirmCommand
{
    public class EmailConfirmRequest : IRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
