namespace MeetApp.Infrastructure.Entities
{
    public class Meeting
    {
        public string Id { get; set; }
        public int OrganizerId { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
