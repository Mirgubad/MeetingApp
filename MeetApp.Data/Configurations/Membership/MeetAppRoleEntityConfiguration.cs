using MeetApp.Infrastructure.Entities.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations.Membership
{
    internal class MeetAppRoleEntityConfiguration : IEntityTypeConfiguration<MeetAppRole>
    {
        public void Configure(EntityTypeBuilder<MeetAppRole> builder)
        {
            builder.ToTable("Roles", "Membership");
        }
    }
}
