using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class BreakTime : BaseEntity
    {
        // Hangi çalışma saatine ait
        [Required(ErrorMessage = "Çalışma saati seçimi zorunludur")]
        public int WorkingHourId { get; set; }

        [ForeignKey(nameof(WorkingHourId))]
        public virtual WorkingHour WorkingHour { get; set; } = null!;

        // Mola başlangıç saati
        [Required(ErrorMessage = "Mola başlangıç saati zorunludur")]
        public TimeSpan StartTime { get; set; }

        // Mola bitiş saati
        [Required(ErrorMessage = "Mola bitiş saati zorunludur")]
        public TimeSpan EndTime { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
