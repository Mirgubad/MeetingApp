using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppUserTokenEntityConfiguration : IEntityTypeConfiguration<MeetAppUserToken>
    {
        public void Configure(EntityTypeBuilder<MeetAppUserToken> builder)
        {
            builder.ToTable("UserTokens", "Membership");
        }
    }
}
