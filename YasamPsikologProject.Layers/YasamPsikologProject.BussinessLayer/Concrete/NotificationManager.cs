using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SendAppointmentConfirmationAsync(Appointment appointment)
        {
            var client = appointment.Client ?? await _unitOfWork.ClientRepository.GetByIdAsync(appointment.ClientId);
            var psychologist = appointment.Psychologist ?? await _unitOfWork.PsychologistRepository.GetByIdAsync(appointment.PsychologistId);

            if (client?.User == null || psychologist?.User == null)
                return;

            string message = $"Sayın {client.User.FirstName} {client.User.LastName}, " +
                           $"{appointment.AppointmentDate:dd.MM.yyyy HH:mm} tarihinde " +
                           $"{psychologist.User.FirstName} {psychologist.User.LastName} ile randevunuz oluşturulmuştur.";

            var notification = new AppointmentNotification
            {
                AppointmentId = appointment.Id,
                NotificationType = client.PreferredNotificationMethod ?? NotificationType.SMS,
                RecipientContact = client.User.PhoneNumber,
                RecipientPhoneNumber = client.User.PhoneNumber,
                RecipientEmail = client.User.Email,
                Message = message,
                IsSent = false
            };

            await _unitOfWork.AppointmentNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            bool sent = false;
            if (notification.NotificationType == NotificationType.WhatsApp)
            {
                sent = await SendWhatsAppAsync(notification.RecipientPhoneNumber, message);
            }
            else
            {
                sent = await SendSmsAsync(notification.RecipientPhoneNumber, message);
            }

            notification.IsSent = sent;
            notification.SentAt = sent ? DateTime.UtcNow : null;
            _unitOfWork.AppointmentNotificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendAppointmentReminderAsync(Appointment appointment)
        {
            var client = appointment.Client ?? await _unitOfWork.ClientRepository.GetByIdAsync(appointment.ClientId);
            var psychologist = appointment.Psychologist ?? await _unitOfWork.PsychologistRepository.GetByIdAsync(appointment.PsychologistId);

            if (client?.User == null || psychologist?.User == null)
                return;

            string message = $"Sayın {client.User.FirstName} {client.User.LastName}, " +
                           $"{appointment.AppointmentDate:dd.MM.yyyy HH:mm} tarihindeki randevunuzu hatırlatmak isteriz. " +
                           $"Psikolog: {psychologist.User.FirstName} {psychologist.User.LastName}";

            var notification = new AppointmentNotification
            {
                AppointmentId = appointment.Id,
                NotificationType = client.PreferredNotificationMethod ?? NotificationType.SMS,
                RecipientContact = client.User.PhoneNumber,
                RecipientPhoneNumber = client.User.PhoneNumber,
                RecipientEmail = client.User.Email,
                Message = message,
                IsSent = false
            };

            await _unitOfWork.AppointmentNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            bool sent = false;
            if (notification.NotificationType == NotificationType.WhatsApp)
            {
                sent = await SendWhatsAppAsync(notification.RecipientPhoneNumber, message);
            }
            else
            {
                sent = await SendSmsAsync(notification.RecipientPhoneNumber, message);
            }

            notification.IsSent = sent;
            notification.SentAt = sent ? DateTime.UtcNow : null;
            appointment.ReminderSent = sent;
            
            _unitOfWork.AppointmentNotificationRepository.Update(notification);
            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendAppointmentCancellationAsync(Appointment appointment, string reason)
        {
            var client = appointment.Client ?? await _unitOfWork.ClientRepository.GetByIdAsync(appointment.ClientId);
            var psychologist = appointment.Psychologist ?? await _unitOfWork.PsychologistRepository.GetByIdAsync(appointment.PsychologistId);

            if (client?.User == null || psychologist?.User == null)
                return;

            string message = $"Sayın {client.User.FirstName} {client.User.LastName}, " +
                           $"{appointment.AppointmentDate:dd.MM.yyyy HH:mm} tarihindeki randevunuz iptal edilmiştir. " +
                           $"Sebep: {reason}";

            var notification = new AppointmentNotification
            {
                AppointmentId = appointment.Id,
                NotificationType = client.PreferredNotificationMethod ?? NotificationType.SMS,
                RecipientContact = client.User.PhoneNumber,
                RecipientPhoneNumber = client.User.PhoneNumber,
                RecipientEmail = client.User.Email,
                Message = message,
                IsSent = false
            };

            await _unitOfWork.AppointmentNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            bool sent = false;
            if (notification.NotificationType == NotificationType.WhatsApp)
            {
                sent = await SendWhatsAppAsync(notification.RecipientPhoneNumber, message);
            }
            else
            {
                sent = await SendSmsAsync(notification.RecipientPhoneNumber, message);
            }

            notification.IsSent = sent;
            notification.SentAt = sent ? DateTime.UtcNow : null;
            _unitOfWork.AppointmentNotificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> SendWhatsAppAsync(string phoneNumber, string message)
        {
            // TODO: WhatsApp API entegrasyonu yapılacak (Twilio, MessageBird vb.)
            await Task.Delay(100);
            return false; // Geçici olarak false dönüyoruz
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            // TODO: SMS API entegrasyonu yapılacak (Netgsm, İleti Merkezi vb.)
            await Task.Delay(100);
            return false; // Geçici olarak false dönüyoruz
        }
    }
}
