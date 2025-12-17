using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class Client : BaseEntity
    {
        // User ile One-to-One ilişki (Her danışan bir kullanıcıdır)
        [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        // Atanmış psikolog (opsiyonel - sonradan atanabilir)
        public int? AssignedPsychologistId { get; set; }

        [ForeignKey(nameof(AssignedPsychologistId))]
        public virtual Psychologist? AssignedPsychologist { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        // KVKK Onayı (zorunlu)
        [Required(ErrorMessage = "KVKK onayı zorunludur")]
        public bool KvkkConsentGiven { get; set; } = false;
        
        public DateTime? KvkkConsentDate { get; set; }

        // Tercih edilen bildirim yöntemi
        public NotificationType? PreferredNotificationMethod { get; set; }

        // WhatsApp bildirimi onayı
        public bool WhatsAppNotificationEnabled { get; set; } = true;

        // SMS bildirimi onayı
        public bool SmsNotificationEnabled { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
