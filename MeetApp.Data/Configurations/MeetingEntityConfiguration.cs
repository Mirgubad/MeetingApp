using MeetApp.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations
{
    public class MeetingEntityConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.ToTable("Meetings");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                   .HasColumnType("varchar")
                   .HasDefaultValueSql("NEWID()")
                   .HasMaxLength(50);

            builder.Property(m => m.OrganizerId)
                   .IsRequired();


            builder.Property(m => m.IsClosed)
                   .HasDefaultValue(false);

            builder.Property(m => m.StartTime);
            builder.Property(m => m.EndTime);

            builder.HasMany<Participant>()
                   .WithOne()
                   .HasForeignKey(m => m.MeetingId);

        }
    }
}
