using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class AppointmentNotificationConfiguration : IEntityTypeConfiguration<AppointmentNotification>
    {
        public void Configure(EntityTypeBuilder<AppointmentNotification> builder)
        {
            builder.ToTable("AppointmentNotifications");

            builder.HasKey(n => n.Id);

            builder.HasIndex(n => n.AppointmentId)
                .HasDatabaseName("IX_AppointmentNotifications_AppointmentId");

            builder.HasIndex(n => n.IsSent)
                .HasDatabaseName("IX_AppointmentNotifications_IsSent");

            builder.Property(n => n.RowVersion)
                .IsRowVersion();
        }
    }
}
