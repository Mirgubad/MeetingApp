using MeetApp.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetApp.Data.Configurations
{
    public class ParticipantEntityConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable("Participants");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                   .HasColumnType("int")
                   .IsRequired();

            builder.Property(p => p.MeetingId)
                   .IsRequired()
                   .HasColumnType("varchar")
                   .HasMaxLength(50); // Adjust length as necessary

            builder.Property(p => p.Email).HasColumnType("varchar").HasMaxLength(150);
            builder.Property(m => m.IsAccepted)
                 .HasDefaultValue(false);

            // Configure foreign key relationship to Meeting
            builder.HasOne<Meeting>()
                   .WithMany()
                   .HasForeignKey(p => p.MeetingId)
                   .IsRequired();
        }
    }
}
