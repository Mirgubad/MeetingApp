using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppUserRoleEntityConfiguration : IEntityTypeConfiguration<MeetAppUserRole>
    {
        public void Configure(EntityTypeBuilder<MeetAppUserRole> builder)
        {
            builder.ToTable("UserRoles", "Membership");
        }
    }
}
