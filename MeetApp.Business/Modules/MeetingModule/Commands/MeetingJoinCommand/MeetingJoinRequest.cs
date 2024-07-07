using MediatR;

namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingJoinCommand
{
    public class MeetingJoinRequest : IRequest<MeetingJoinDto>
    {
        public string MeetingId { get; set; }
    }
}
