using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class AppointmentNotification : BaseEntity
    {
        // Hangi randevuya ait
        [Required(ErrorMessage = "Randevu seçimi zorunludur")]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment Appointment { get; set; } = null!;

        // Bildirim tipi
        [Required(ErrorMessage = "Bildirim tipi zorunludur")]
        public NotificationType NotificationType { get; set; }

        // Alıcı bilgisi (telefon veya email)
        [Required(ErrorMessage = "Alıcı bilgisi zorunludur")]
        [MaxLength(255)]
        public string RecipientContact { get; set; } = null!;

        [MaxLength(20)]
        public string RecipientPhoneNumber { get; set; } = null!;

        [MaxLength(255)]
        public string RecipientEmail { get; set; } = null!;

        // Mesaj içeriği
        [Required(ErrorMessage = "Mesaj içeriği zorunludur")]
        [MaxLength(2000)]
        public string Message { get; set; } = null!;

        // Gönderildi mi?
        public bool IsSent { get; set; } = false;

        public DateTime? SentAt { get; set; }

        // Hata durumunda
        public bool HasError { get; set; } = false;

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        // Bildirim amacı (Randevu onayı, hatırlatma, iptal vb.)
        [MaxLength(50)]
        public string? NotificationPurpose { get; set; }
    }
}
