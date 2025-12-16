using System.ComponentModel.DataAnnotations;
using YasamPsikologProject.EntityLayer.Abstract;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.EntityLayer.Concrete
{
    public class User : BaseEntity
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [MaxLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [MaxLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "Kullanıcı rolü zorunludur")]
        public UserRole Role { get; set; }

        public Gender Gender { get; set; } = Gender.NotSpecified;

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // JWT Refresh Token
        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        public virtual Psychologist? Psychologist { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
