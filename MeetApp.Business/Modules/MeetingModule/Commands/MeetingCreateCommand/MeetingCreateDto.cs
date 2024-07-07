namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingCreateCommand
{
    public class MeetingCreateDto
    {
        public string MeetingId { get; set; }
        public string UserFullname { get; set; }
        public string? NickName { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime? EndTime { get; set; }
    }
}
