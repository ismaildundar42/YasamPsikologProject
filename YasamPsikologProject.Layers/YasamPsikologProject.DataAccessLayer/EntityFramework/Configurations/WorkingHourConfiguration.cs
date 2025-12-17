using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class WorkingHourConfiguration : IEntityTypeConfiguration<WorkingHour>
    {
        public void Configure(EntityTypeBuilder<WorkingHour> builder)
        {
            builder.ToTable("WorkingHours");

            builder.HasKey(w => w.Id);

            builder.HasIndex(w => new { w.PsychologistId, w.DayOfWeek })
                .HasDatabaseName("IX_WorkingHours_Psychologist_Day");

            builder.Property(w => w.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(w => w.RowVersion)
                .IsRowVersion();
        }
    }
}
