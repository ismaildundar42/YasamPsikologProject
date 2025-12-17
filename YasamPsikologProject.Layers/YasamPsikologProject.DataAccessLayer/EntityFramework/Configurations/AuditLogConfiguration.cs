using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(a => a.CreatedAt)
                .HasDatabaseName("IX_AuditLogs_CreatedAt");

            builder.HasIndex(a => new { a.EntityName, a.EntityId })
                .HasDatabaseName("IX_AuditLogs_Entity");

            builder.Property(a => a.RowVersion)
                .IsRowVersion();
        }
    }
}
