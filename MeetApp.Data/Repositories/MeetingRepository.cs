using MeetApp.Infrastructure.Commons.Concretes;
using MeetApp.Infrastructure.Entities;
using MeetApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeetApp.Data.Repositories
{
    public class MeetingRepository : Repository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(DbContext db) : base(db)
        {
        }
    }
}
