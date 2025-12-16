using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class UnavailableTime : BaseEntity
    {
        // Hangi psikoloğa ait
        [Required(ErrorMessage = "Psikolog seçimi zorunludur")]
        public int PsychologistId { get; set; }

        [ForeignKey(nameof(PsychologistId))]
        public virtual Psychologist Psychologist { get; set; } = null!;

        // Müsait olmayan zaman aralığı
        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur")]
        public DateTime EndDateTime { get; set; }

        // Sebep
        [Required(ErrorMessage = "Sebep girilmesi zorunludur")]
        [MaxLength(500)]
        public string Reason { get; set; } = null!;

        // Tüm gün mü? (örn: izin, tatil)
        public bool IsAllDay { get; set; } = false;

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
