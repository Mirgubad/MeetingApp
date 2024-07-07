using MediatR;

namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingCreateCommand
{
    public class MeetingCreateRequest : IRequest<MeetingCreateDto>
    {
        public string Id { get; set; }
        public string? NickName { get; set; }
        public int OrganizerId { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
