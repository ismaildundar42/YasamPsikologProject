using YasamPsikologProject.DataAccessLayer.Abstract;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
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

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
