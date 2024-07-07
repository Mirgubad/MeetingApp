namespace MeetApp.Infrastructure.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MeetingId { get; set; }
        public string Email { get; set; }
        public bool IsAccepted { get; set; }
    }
}
