using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IAppointmentNotificationRepository : IRepository<AppointmentNotification>
    {
        Task<IEnumerable<AppointmentNotification>> GetByAppointmentAsync(int appointmentId);
        Task<IEnumerable<AppointmentNotification>> GetPendingNotificationsAsync();
    }
}
