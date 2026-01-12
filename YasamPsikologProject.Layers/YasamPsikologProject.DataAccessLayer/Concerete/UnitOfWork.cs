using Microsoft.EntityFrameworkCore.Storage;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.DataAccessLayer.Repositories;

namespace YasamPsikologProject.DataAccessLayer.Concerete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            UserRepository = new UserRepository(_context);
            PsychologistRepository = new PsychologistRepository(_context);
            ClientRepository = new ClientRepository(_context);
            AppointmentRepository = new AppointmentRepository(_context);
            WorkingHourRepository = new WorkingHourRepository(_context);
            BreakTimeRepository = new BreakTimeRepository(_context);
            UnavailableTimeRepository = new UnavailableTimeRepository(_context);
            AppointmentNotificationRepository = new AppointmentNotificationRepository(_context);
            AuditLogRepository = new AuditLogRepository(_context);
            SystemSettingRepository = new SystemSettingRepository(_context);
        }

        public IUserRepository UserRepository { get; private set; }
        public IPsychologistRepository PsychologistRepository { get; private set; }
        public IClientRepository ClientRepository { get; private set; }
        public IAppointmentRepository AppointmentRepository { get; private set; }
        public IWorkingHourRepository WorkingHourRepository { get; private set; }
        public IBreakTimeRepository BreakTimeRepository { get; private set; }
        public IUnavailableTimeRepository UnavailableTimeRepository { get; private set; }
        public IAppointmentNotificationRepository AppointmentNotificationRepository { get; private set; }
        public IAuditLogRepository AuditLogRepository { get; private set; }
        public ISystemSettingRepository SystemSettingRepository { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
