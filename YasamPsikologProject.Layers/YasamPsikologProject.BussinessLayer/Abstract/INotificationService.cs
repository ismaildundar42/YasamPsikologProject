using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface INotificationService
    {
        Task SendAppointmentConfirmationAsync(Appointment appointment);
        Task SendAppointmentReminderAsync(Appointment appointment);
        Task SendAppointmentCancellationAsync(Appointment appointment, string reason);
        Task<bool> SendWhatsAppAsync(string phoneNumber, string message);
        Task<bool> SendSmsAsync(string phoneNumber, string message);
    }
}
