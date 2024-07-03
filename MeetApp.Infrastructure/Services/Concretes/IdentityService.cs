using MeetApp.Infrastructure.Services.Abstracts;

namespace MeetApp.Infrastructure.Services.Concretes
{
    public class IdentityService : IIdentityService
    {
        public int GetPrincipialId()
        {
            return 6;
        }
    }
}
