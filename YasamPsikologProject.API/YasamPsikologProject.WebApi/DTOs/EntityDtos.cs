using System.ComponentModel.DataAnnotations;

namespace YasamPsikologProject.WebApi.DTOs
{
    public class CreatePsychologistDto
    {
        // User bilgileri
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Telefon alanı zorunludur")]
        [RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır")]
        public string PhoneNumber { get; set; } = null!;

        // Psychologist bilgileri
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Renk kodu hex formatında olmalıdır")]
        public string? CalendarColor { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdatePsychologistDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? CalendarColor { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateClientDto
    {
        // User bilgileri
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Client bilgileri
        public int? AssignedPsychologistId { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string PreferredNotificationMethod { get; set; } = "Email";
        public bool KvkkConsent { get; set; }
    }

    public class UpdateClientDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int? AssignedPsychologistId { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string PreferredNotificationMethod { get; set; } = "Email";
        public bool KvkkConsent { get; set; }
    }

    public class BreakTimeDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Mola başlangıç saati zorunludur")]
        public string StartTime { get; set; } = null!;

        [Required(ErrorMessage = "Mola bitiş saati zorunludur")]
        public string EndTime { get; set; } = null!;

        public string? Notes { get; set; }
    }

    public class CreateWorkingHourDto
    {
        [Required(ErrorMessage = "Psikolog seçimi zorunludur")]
        public int PsychologistId { get; set; }

        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        public string DayOfWeek { get; set; } = null!;

        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        public string StartTime { get; set; } = null!;

        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        public string EndTime { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;
        public int BufferDuration { get; set; } = 10;
        public List<BreakTimeDto>? BreakTimes { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateWorkingHourDto
    {
        [Required(ErrorMessage = "Gün seçimi zorunludur")]
        public string DayOfWeek { get; set; } = null!;

        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        public string StartTime { get; set; } = null!;

        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        public string EndTime { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;
        public int BufferDuration { get; set; } = 10;
        public List<BreakTimeDto>? BreakTimes { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateAppointmentDto
    {
        [Required]
        public int PsychologistId { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Range(15, 240)]
        public int Duration { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending";
        
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        [Required]
        public int PsychologistId { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Range(15, 240)]
        public int Duration { get; set; }

        public int BreakDuration { get; set; } = 10;
        
        [Required]
        public string Status { get; set; } = "Pending";
        
        public string? Notes { get; set; }
    }
}
