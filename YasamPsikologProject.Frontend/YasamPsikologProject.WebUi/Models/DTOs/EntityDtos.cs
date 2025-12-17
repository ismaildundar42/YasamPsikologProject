namespace YasamPsikologProject.WebUi.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PsychologistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string? Bio { get; set; }
        public string? CalendarColor { get; set; }
        public int ConsultationFee { get; set; }
        public int ConsultationDuration { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClientDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int? AssignedPsychologistId { get; set; }
        public PsychologistDto? AssignedPsychologist { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Notes { get; set; }
        public bool KvkkConsent { get; set; }
        public DateTime? KvkkConsentDate { get; set; }
        public string? PreferredNotificationMethod { get; set; }
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
        public string Duration { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public bool ReminderSent { get; set; }
        public string? MeetingLink { get; set; }
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
}
