using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class BreakTimeConfiguration : IEntityTypeConfiguration<BreakTime>
    {
        public void Configure(EntityTypeBuilder<BreakTime> builder)
        {
            builder.ToTable("BreakTimes");

            builder.HasKey(b => b.Id);

            builder.HasIndex(b => b.WorkingHourId)
                .HasDatabaseName("IX_BreakTimes_WorkingHour");

            builder.Property(b => b.RowVersion)
                .IsRowVersion();

            // WorkingHour ile ilişki
            builder.HasOne(b => b.WorkingHour)
                .WithMany(w => w.BreakTimes)
                .HasForeignKey(b => b.WorkingHourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
