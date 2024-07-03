using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppUserClaimEntityConfiguration : IEntityTypeConfiguration<MeetAppUserClaim>
    {
        public void Configure(EntityTypeBuilder<MeetAppUserClaim> builder)
        {
            builder.ToTable("UserClaims", "Membership");
        }
    }
}
