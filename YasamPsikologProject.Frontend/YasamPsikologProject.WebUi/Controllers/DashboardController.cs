using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.ViewModels;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("[controller]")]
    public class DashboardController : Controller
    {
        private readonly IApiAppointmentService _appointmentService;
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiClientService _clientService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IApiAppointmentService appointmentService,
            IApiPsychologistService psychologistService,
            IApiClientService clientService,
            ILogger<DashboardController> logger)
        {
            _appointmentService = appointmentService;
            _psychologistService = psychologistService;
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Dashboard";

            var model = new DashboardViewModel();

            try
            {
                // Tüm randevuları çek
                var appointmentsResponse = await _appointmentService.GetAllAsync();
                if (appointmentsResponse.Success && appointmentsResponse.Data != null)
                {
                    var appointments = appointmentsResponse.Data;
                    var today = DateTime.Today;

                    model.TotalAppointments = appointments.Count;
                    model.TodayAppointments = appointments.Count(a => a.AppointmentDate.Date == today);
                    model.PendingAppointments = appointments.Count(a => a.Status == "Pending");
                    model.CompletedAppointments = appointments.Count(a => a.Status == "Completed");
                    
                    // Son 5 randevu
                    model.RecentAppointments = appointments
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(5)
                        .ToList();
                    
                    // Bugünkü randevular
                    model.TodayAppointmentsList = appointments
                        .Where(a => a.AppointmentDate.Date == today)
                        .OrderBy(a => a.AppointmentDate)
                        .ToList();
                }

                // Psikolog sayısı
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    model.TotalPsychologists = psychologistsResponse.Data.Count;
                }

                // Danışan sayısı
                var clientsResponse = await _clientService.GetAllAsync();
                if (clientsResponse.Success && clientsResponse.Data != null)
                {
                    model.TotalClients = clientsResponse.Data.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu.";
            }

            return View(model);
        }
    }
}
