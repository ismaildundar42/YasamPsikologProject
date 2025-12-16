using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class Appointment : BaseEntity
    {
        // Danışan bilgisi
        [Required(ErrorMessage = "Danışan seçimi zorunludur")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; } = null!;

        // Psikolog bilgisi
        [Required(ErrorMessage = "Psikolog seçimi zorunludur")]
        public int PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist Psychologist { get; set; } = null!;

        // Randevu tarihi ve saati
        [Required(ErrorMessage = "Randevu tarihi zorunludur")]
        public DateTime AppointmentDate { get; set; }

        // Randevu süresi (dakika)
        [Required(ErrorMessage = "Randevu süresi zorunludur")]
        public AppointmentDuration Duration { get; set; }

        // Randevu bitiş zamanı (hesaplanmış - ara süre dahil)
        [Required(ErrorMessage = "Bitiş zamanı zorunludur")]
        public DateTime AppointmentEndDate { get; set; }

        // Ara süre (dakika) - varsayılan 10, ayarlanabilir
        public int BreakDuration { get; set; } = 10;

        // Randevu durumu
        [Required(ErrorMessage = "Randevu durumu zorunludur")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // Online mı yüz yüze mi
        public bool IsOnline { get; set; } = true;

        // Danışan notları
        [MaxLength(1000)]
        public string? ClientNotes { get; set; }

        // Psikolog notları (gizli - sadece psikolog görür)
        [MaxLength(2000)]
        public string? PsychologistNotes { get; set; }

        // İptal bilgileri
        public DateTime? CancelledAt { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        // Hatırlatma gönderildi mi?
        public bool ReminderSent { get; set; } = false;

        // Online görüşme linki
        [MaxLength(500)]
        public string? MeetingLink { get; set; }

        // Navigation Properties
        public virtual ICollection<AppointmentNotification> Notifications { get; set; } = new List<AppointmentNotification>();
    }
}
