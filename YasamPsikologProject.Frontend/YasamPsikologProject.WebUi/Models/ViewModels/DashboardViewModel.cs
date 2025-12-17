using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int TotalPsychologists { get; set; }
        public int TotalClients { get; set; }
        
        public List<AppointmentDto> RecentAppointments { get; set; } = new();
        public List<AppointmentDto> TodayAppointmentsList { get; set; } = new();
    }
}
