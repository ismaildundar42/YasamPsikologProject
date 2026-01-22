using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        AppDbContext Context { get; } // Arşiv için direkt Context erişimi
        
        IUserRepository UserRepository { get; }
        IPsychologistRepository PsychologistRepository { get; }
        IClientRepository ClientRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IWorkingHourRepository WorkingHourRepository { get; }
        IBreakTimeRepository BreakTimeRepository { get; }
        IUnavailableTimeRepository UnavailableTimeRepository { get; }
        IAppointmentNotificationRepository AppointmentNotificationRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }
        ISystemSettingRepository SystemSettingRepository { get; }
        IPsychologistArchiveRepository PsychologistArchiveRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
