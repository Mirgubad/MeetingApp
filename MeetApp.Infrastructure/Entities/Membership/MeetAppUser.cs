using Microsoft.AspNetCore.Identity;

namespace MeetApp.Infrastructure.Entities.Membership
{
    public class MeetAppUser : IdentityUser<int>
    {
        public string Fullname { get; set; }
    }
}
