using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<BreakTime> BreakTimes { get; set; }
        public DbSet<UnavailableTime> UnavailableTimes { get; set; }
        public DbSet<AppointmentNotification> AppointmentNotifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<PsychologistArchive> PsychologistArchive { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration'ları uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Soft delete için global query filter
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Psychologist>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<WorkingHour>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<BreakTime>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<UnavailableTime>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<AppointmentNotification>().HasQueryFilter(n => !n.IsDeleted);
            modelBuilder.Entity<AuditLog>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<SystemSetting>().HasQueryFilter(s => !s.IsDeleted);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EntityLayer.Abstract.BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (EntityLayer.Abstract.BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
