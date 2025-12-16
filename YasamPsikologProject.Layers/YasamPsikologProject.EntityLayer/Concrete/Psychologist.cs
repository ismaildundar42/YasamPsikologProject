using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class Psychologist : BaseEntity
    {
        // User ile One-to-One ilişki (Her psikolog bir kullanıcıdır)
        [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [Required(ErrorMessage = "Lisans numarası zorunludur")]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = null!;

        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(1000)]
        public string? Biography { get; set; }

        [Range(0, 50, ErrorMessage = "Deneyim yılı 0-50 arasında olmalıdır")]
        public int ExperienceYears { get; set; } = 0;

        [MaxLength(500)]
        public string? Education { get; set; }

        [MaxLength(500)]
        public string? Certifications { get; set; }

        // Google Calendar renk kodu (hex format: #FF5733)
        [MaxLength(7)]
        public string CalendarColor { get; set; } = "#3788D8";

        // Online/Yüz yüze seçenekleri
        public bool IsOnlineConsultationAvailable { get; set; } = true;
        public bool IsInPersonConsultationAvailable { get; set; } = true;

        // Otomatik randevu onayı (true ise admin onayına gerek yok)
        public bool AutoApproveAppointments { get; set; } = false;

        // Navigation Properties
        public virtual ICollection<Client> AssignedClients { get; set; } = new List<Client>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public virtual ICollection<UnavailableTime> UnavailableTimes { get; set; } = new List<UnavailableTime>();
    }
}
