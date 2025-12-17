using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.ToTable("SystemSettings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.Key)
                .IsUnique()
                .HasDatabaseName("IX_SystemSettings_Key");

            builder.HasIndex(s => s.Category)
                .HasDatabaseName("IX_SystemSettings_Category");

            builder.Property(s => s.RowVersion)
                .IsRowVersion();
        }
    }
}
