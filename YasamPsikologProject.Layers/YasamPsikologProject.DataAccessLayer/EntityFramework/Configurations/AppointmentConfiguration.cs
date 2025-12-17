using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.ClientId)
                .HasDatabaseName("IX_Appointments_ClientId");

            builder.HasIndex(a => a.PsychologistId)
                .HasDatabaseName("IX_Appointments_PsychologistId");

            builder.HasIndex(a => a.AppointmentDate)
                .HasDatabaseName("IX_Appointments_AppointmentDate");

            builder.HasIndex(a => a.Status)
                .HasDatabaseName("IX_Appointments_Status");

            // Çakışma kontrolü için composite index
            builder.HasIndex(a => new { a.PsychologistId, a.AppointmentDate, a.AppointmentEndDate })
                .HasDatabaseName("IX_Appointments_Overlap_Check");

            builder.Property(a => a.BreakDuration)
                .HasDefaultValue(10);

            builder.Property(a => a.RowVersion)
                .IsRowVersion();

            // One-to-Many: Appointment - Notifications
            builder.HasMany(a => a.Notifications)
                .WithOne(n => n.Appointment)
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
