using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    /// <summary>
    /// Silinmiş Psikolog Arşivi
    /// Psikolog silindiğinde tüm bilgileri buraya kopyalanır
    /// Böylece geçmiş randevularda psikolog bilgileri görünür
    /// </summary>
    [Table("PsychologistArchive")]
    public class PsychologistArchive : BaseEntity
    {
        // Orijinal Psikolog ID'si
        [Required]
        public int OriginalPsychologistId { get; set; }

        // Kullanıcı bilgileri (kopyalanan)
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Psikolog özellikleri
        [Required]
        [MaxLength(7)]
        public string CalendarColor { get; set; } = "#3788D8";

        public bool AutoApproveAppointments { get; set; }

        // Arşiv bilgileri
        [Required]
        public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(500)]
        public string ArchivedReason { get; set; } = "Psikolog silindi";

        [MaxLength(100)]
        public string? ArchivedByUser { get; set; }

        // Orijinal oluşturma tarihi
        public DateTime OriginalCreatedAt { get; set; }
    }
}
