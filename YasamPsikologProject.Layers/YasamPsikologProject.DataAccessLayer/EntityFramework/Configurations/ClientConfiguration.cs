using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Clients_UserId");

            builder.HasIndex(c => c.AssignedPsychologistId)
                .HasDatabaseName("IX_Clients_AssignedPsychologistId");

            builder.Property(c => c.KvkkConsentGiven)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.RowVersion)
                .IsRowVersion();

            // One-to-Many: Client - Appointments
            builder.HasMany(c => c.Appointments)
                .WithOne(a => a.Client)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
