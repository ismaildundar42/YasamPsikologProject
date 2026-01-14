using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<bool> HasConflictAsync(int psychologistId, DateTime startDate, DateTime endDate, int? excludeAppointmentId = null);
        Task<bool> HasClientConflictAsync(int clientId, DateTime startDate, DateTime endDate, int? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Appointment>> GetByClientAsync(int clientId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int psychologistId, int days = 7);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync();
    }
}
