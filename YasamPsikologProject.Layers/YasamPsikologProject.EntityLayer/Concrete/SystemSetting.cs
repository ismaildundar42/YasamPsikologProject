using System.ComponentModel.DataAnnotations;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class SystemSetting : BaseEntity
    {
        // Ayar anahtarı (unique)
        [Required(ErrorMessage = "Ayar anahtarı zorunludur")]
        [MaxLength(100)]
        public string Key { get; set; } = null!;

        // Ayar değeri
        [Required(ErrorMessage = "Ayar değeri zorunludur")]
        [MaxLength(1000)]
        public string Value { get; set; } = null!;

        // Ayar açıklaması
        [MaxLength(500)]
        public string? Description { get; set; }

        // Ayar kategorisi (Appointment, Notification, General vb.)
        [MaxLength(50)]
        public string? Category { get; set; }
    }
}
