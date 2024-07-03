using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppUserLoginEntityConfiguration : IEntityTypeConfiguration<MeetAppUserLogin>
    {
        public void Configure(EntityTypeBuilder<MeetAppUserLogin> builder)
        {
            builder.ToTable("UserLogins", "Membership");
        }
    }
}
