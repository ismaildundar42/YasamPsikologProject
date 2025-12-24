using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Models.ViewModels
{
    public class PsychologistDashboardViewModel
    {
        // İstatistikler
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int TotalClients { get; set; }

        // Listeler
        public List<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
        public List<AppointmentDto> TodayAppointmentsList { get; set; } = new List<AppointmentDto>();

        // Psikolog Bilgileri
        public PsychologistDto? PsychologistInfo { get; set; }
    }
}
