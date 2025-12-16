using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class WorkingHour : BaseEntity
    {
        // Hangi psikoloğa ait
        [Required(ErrorMessage = "Psikolog seçimi zorunludur")]
        public int PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist Psychologist { get; set; } = null!;

        // Haftanın günü
        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        public WeekDay DayOfWeek { get; set; }

        // Başlangıç saati (örn: 09:00)
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        public TimeSpan StartTime { get; set; }

        // Bitiş saati (örn: 17:00)
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        public TimeSpan EndTime { get; set; }

        // Bu gün aktif mi? (geçici olarak kapatma için)
        public bool IsAvailable { get; set; } = true;

        // Öğle arası gibi molalar için (opsiyonel)
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
