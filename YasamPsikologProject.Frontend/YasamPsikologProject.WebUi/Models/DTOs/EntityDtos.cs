using System.ComponentModel.DataAnnotations;

namespace YasamPsikologProject.WebUi.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = null!;
        
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = null!;
        
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Telefon alanı zorunludur")]
        [RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve 11 haneli olmalıdır (örn: 05551234567)")]
        public string PhoneNumber { get; set; } = null!;
        
        public string? Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PsychologistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Renk kodu hex formatında olmalıdır (örn: #4CAF50)")]
        public string? CalendarColor { get; set; }
        
        public bool IsOnlineConsultationAvailable { get; set; } = true;
        public bool IsInPersonConsultationAvailable { get; set; } = true;
        public bool AutoApproveAppointments { get; set; } = false;
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ClientDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int? AssignedPsychologistId { get; set; }
        public PsychologistDto? AssignedPsychologist { get; set; }
        
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
        public string? Address { get; set; }
        
        [StringLength(2000, ErrorMessage = "Notlar en fazla 2000 karakter olabilir")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "KVKK onayı zorunludur")]
        public bool KvkkConsent { get; set; }
        
        public DateTime? KvkkConsentDate { get; set; }
        public string? PreferredNotificationMethod { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PsychologistId { get; set; }
        public PsychologistDto? Psychologist { get; set; }
        public int ClientId { get; set; }
        public ClientDto? Client { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentEndDate { get; set; }
        public int Duration { get; set; }
        public int BreakDuration { get; set; } = 10;
        public string Status { get; set; } = null!;
        public bool IsOnline { get; set; } = true;
        public string? ClientNotes { get; set; }
        public string? PsychologistNotes { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public bool ReminderSent { get; set; }
        public string? MeetingLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Computed properties for display
        public string ClientName => Client != null ? $"{Client.User?.FirstName} {Client.User?.LastName}" : "N/A";
        public string PsychologistName => Psychologist != null ? $"{Psychologist.User?.FirstName} {Psychologist.User?.LastName}" : "N/A";
    }

    public class WorkingHourDto
    {
        public int Id { get; set; }
        public int PsychologistId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public string? Notes { get; set; }
    }

    public class UnavailableTimeDto
    {
        public int Id { get; set; }
        public int PsychologistId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Reason { get; set; } = null!;
        public bool IsAllDay { get; set; }
    }

    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Role { get; set; }
        public int? PsychologistId { get; set; }
        public int? ClientId { get; set; }
    }
}
