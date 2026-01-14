using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Appointment>> GetByClientAsync(int clientId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int psychologistId, int days = 7);
        Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync();
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<Appointment> UpdateStatusAsync(int id, AppointmentStatus status, string? reason = null);
        Task CancelAsync(int id, string reason);
        Task<bool> HasConflictAsync(int psychologistId, DateTime startDate, DateTime endDate, int? excludeAppointmentId = null);
        Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int psychologistId, DateTime date, AppointmentDuration duration, string? clientEmail = null, string? clientPhone = null);
    }
}
