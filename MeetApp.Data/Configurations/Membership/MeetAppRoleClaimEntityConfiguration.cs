using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppRoleClaimEntityConfiguration : IEntityTypeConfiguration<MeetAppRoleClaim>
    {
        public void Configure(EntityTypeBuilder<MeetAppRoleClaim> builder)
        {
            builder.ToTable("RoleClaims", "Membership");
        }
    }
}
