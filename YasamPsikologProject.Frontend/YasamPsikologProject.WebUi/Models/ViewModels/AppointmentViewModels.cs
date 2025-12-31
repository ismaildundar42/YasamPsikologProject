using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Models.ViewModels
{
    public class PsychologistListViewModel
    {
        public List<PsychologistDto> Psychologists { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SpecializationFilter { get; set; }
        public bool? OnlineOnly { get; set; }
        public List<string> Specializations { get; set; } = new();
    }

    public class AppointmentBookingViewModel
    {
        public PsychologistDto Psychologist { get; set; } = null!;
        public DateTime SelectedDate { get; set; }
        public List<DateTime> AvailableSlots { get; set; } = new();
        public int Duration { get; set; } = 50;
        public bool IsOnline { get; set; } = true;
        public List<int> AvailableDurations { get; set; } = new() { 50, 90, 120 };
        public List<string> WorkingDays { get; set; } = new();
    }

    public class AppointmentConfirmViewModel
    {
        public PsychologistDto Psychologist { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public int Duration { get; set; }
        public bool IsOnline { get; set; }
        public string? ClientNotes { get; set; }
        public bool KvkkConsent { get; set; }
        public decimal TotalFee { get; set; }
    }

    public class MyAppointmentsViewModel
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; } = new();
        public List<AppointmentDto> PastAppointments { get; set; } = new();
        public List<AppointmentDto> PendingAppointments { get; set; } = new();
        public List<AppointmentDto> CancelledAppointments { get; set; } = new();
    }

    public class AdminAppointmentsViewModel
    {
        public List<AppointmentDto> AllAppointments { get; set; } = new();
        public List<AppointmentDto> PendingAppointments { get; set; } = new();
        public List<AppointmentDto> TodayAppointments { get; set; } = new();
        public string? StatusFilter { get; set; }
        public int? PsychologistFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<PsychologistDto> Psychologists { get; set; } = new();
    }

    public class PsychologistAppointmentsViewModel
    {
        public List<AppointmentDto> TodayAppointments { get; set; } = new();
        public List<AppointmentDto> UpcomingAppointments { get; set; } = new();
        public List<AppointmentDto> PendingAppointments { get; set; } = new();
        public List<AppointmentDto> PastAppointments { get; set; } = new();
        public int PsychologistId { get; set; }
        public string PsychologistName { get; set; } = string.Empty;
    }
}
