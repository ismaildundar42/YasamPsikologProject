using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class PsychologistConfiguration : IEntityTypeConfiguration<Psychologist>
    {
        public void Configure(EntityTypeBuilder<Psychologist> builder)
        {
            builder.ToTable("Psychologists");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Psychologists_UserId");

            builder.Property(p => p.CalendarColor)
                .IsRequired()
                .HasMaxLength(7)
                .HasDefaultValue("#3788D8");

            builder.Property(p => p.RowVersion)
                .IsRowVersion();

            // One-to-Many: Psychologist - Clients
            builder.HasMany(p => p.AssignedClients)
                .WithOne(c => c.AssignedPsychologist)
                .HasForeignKey(c => c.AssignedPsychologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Psychologist - Appointments
            builder.HasMany(p => p.Appointments)
                .WithOne(a => a.Psychologist)
                .HasForeignKey(a => a.PsychologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Psychologist - WorkingHours
            builder.HasMany(p => p.WorkingHours)
                .WithOne(w => w.Psychologist)
                .HasForeignKey(w => w.PsychologistId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Psychologist - UnavailableTimes
            builder.HasMany(p => p.UnavailableTimes)
                .WithOne(u => u.Psychologist)
                .HasForeignKey(u => u.PsychologistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
