using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YasamPsikologProject.EntityLayer.Abstract;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class AuditLog : BaseEntity
    {
        // Hangi kullanıcı işlem yaptı
        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        // İşlem tipi (Create, Update, Delete, Login, Logout vb.)
        [Required(ErrorMessage = "İşlem tipi zorunludur")]
        [MaxLength(50)]
        public string Action { get; set; } = null!;

        // Hangi entity'de işlem yapıldı
        [Required(ErrorMessage = "Entity adı zorunludur")]
        [MaxLength(100)]
        public string EntityName { get; set; } = null!;

        // Entity'nin ID'si
        public int? EntityId { get; set; }

        // İşlem açıklaması
        [MaxLength(1000)]
        public string? Description { get; set; }

        // IP adresi
        [MaxLength(50)]
        public string? IpAddress { get; set; }
    }
}
