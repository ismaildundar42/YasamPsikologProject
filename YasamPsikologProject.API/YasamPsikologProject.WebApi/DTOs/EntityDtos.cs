namespace YasamPsikologProject.WebApi.DTOs
{
    public class CreatePsychologistDto
    {
        // User bilgileri
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Psychologist bilgileri
        public string LicenseNumber { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string? CalendarColor { get; set; }
        public int ConsultationFee { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdatePsychologistDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string? CalendarColor { get; set; }
        public int ConsultationFee { get; set; }
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
}
