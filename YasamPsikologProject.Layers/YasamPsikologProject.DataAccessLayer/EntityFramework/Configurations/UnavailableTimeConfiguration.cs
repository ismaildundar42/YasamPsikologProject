using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class UnavailableTimeConfiguration : IEntityTypeConfiguration<UnavailableTime>
    {
        public void Configure(EntityTypeBuilder<UnavailableTime> builder)
        {
            builder.ToTable("UnavailableTimes");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.PsychologistId)
                .HasDatabaseName("IX_UnavailableTimes_PsychologistId");

            builder.HasIndex(u => new { u.PsychologistId, u.StartDateTime, u.EndDateTime })
                .HasDatabaseName("IX_UnavailableTimes_DateRange");

            builder.Property(u => u.RowVersion)
                .IsRowVersion();
        }
    }
}
