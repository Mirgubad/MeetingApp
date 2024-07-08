namespace MeetApp.Business.Modules.MeetingModule.Commands.MeetingJoinCommand
{
    public class MeetingJoinDto
    {
        public string MeetingId { get; set; }
        public string UserFullname { get; set; }
        public string UserEmail { get; set; }
        public object Nickname { get; set; }
    }
}
