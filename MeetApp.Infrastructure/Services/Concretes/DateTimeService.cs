using MeetApp.Infrastructure.Services.Abstracts;

namespace MeetApp.Infrastructure.Services.Concretes
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime ExecutingTime
        {
            get
            {
                return DateTime.Now;

            }
        }
    }


    public class UtcDateTimeService : IDateTimeService
    {
        public DateTime ExecutingTime
        {
            get
            {
                return DateTime.UtcNow;

            }
        }
    }
}
