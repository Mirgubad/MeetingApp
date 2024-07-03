using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppUserEntityConfiguration : IEntityTypeConfiguration<MeetAppUser>
    {
        public void Configure(EntityTypeBuilder<MeetAppUser> builder)
        {
            builder.ToTable("Users", "Membership");
        }
    }
}
