using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.ViewModels;
using YasamPsikologProject.WebUi.Models.DTOs;
using YasamPsikologProject.WebUi.Helpers;

namespace YasamPsikologProject.WebUi.Controllers
{
    public class PublicAppointmentController : Controller
    {
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiAppointmentService _appointmentService;
        private readonly IApiClientService _clientService;
        private readonly IApiWorkingHourService _workingHourService;
        private readonly ILogger<PublicAppointmentController> _logger;

        public PublicAppointmentController(
            IApiPsychologistService psychologistService,
            IApiAppointmentService appointmentService,
            IApiClientService clientService,
            IApiWorkingHourService workingHourService,
            ILogger<PublicAppointmentController> logger)
        {
            _psychologistService = psychologistService;
            _appointmentService = appointmentService;
            _clientService = clientService;
            _workingHourService = workingHourService;
            _logger = logger;
        }

        // GET: /PublicAppointment/Index - Psikolog Listesi
        [HttpGet]
        public async Task<IActionResult> Index(string? search, bool? onlineOnly)
        {
            ViewData["PageTitle"] = "Randevu Al";

            var model = new PsychologistListViewModel
            {
                SearchTerm = search,
                OnlineOnly = onlineOnly
            };

            try
            {
                var response = await _psychologistService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    var psychologists = response.Data.Where(p => p.IsActive).ToList();

                    // Filtreleme
                    if (!string.IsNullOrEmpty(search))
                    {
                        var searchLower = search.ToLower();
                        psychologists = psychologists.Where(p =>
                            (p.User?.FirstName?.ToLower().Contains(searchLower) ?? false) ||
                            (p.User?.LastName?.ToLower().Contains(searchLower) ?? false)
                        ).ToList();
                    }

                    if (onlineOnly == true)
                    {
                        psychologists = psychologists.Where(p => p.IsOnlineConsultationAvailable).ToList();
                    }

                    model.Psychologists = psychologists;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog listesi yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Psikologlar yüklenirken bir hata oluştu.";
            }

            return View(model);
        }

        // GET: /PublicAppointment/SelectDateTime - Tarih ve Saat Seçimi
        [HttpGet]
        public async Task<IActionResult> SelectDateTime(int psychologistId, int duration = 50)
        {
            ViewData["PageTitle"] = "Tarih ve Saat Seçimi";

            var model = new AppointmentBookingViewModel
            {
                Duration = duration
            };

            try
            {
                var response = await _psychologistService.GetByIdAsync(psychologistId);
                if (response.Success && response.Data != null)
                {
                    model.Psychologist = response.Data;
                    
                    // Çalışma saatlerini API'den çek
                    var workingHoursResponse = await _workingHourService.GetAllByPsychologistAsync(psychologistId);
                    if (workingHoursResponse.Success && workingHoursResponse.Data != null && workingHoursResponse.Data.Any())
                    {
                        // Aktif çalışma günlerini al
                        model.WorkingDays = workingHoursResponse.Data
                            .Where(wh => wh.IsAvailable)
                            .Select(wh => wh.DayOfWeek)
                            .Distinct()
                            .OrderBy(d => d)
                            .ToList();
                    }
                    
                    // Eğer çalışma günü yoksa, varsayılan olarak hafta içi günleri göster
                    if (model.WorkingDays == null || !model.WorkingDays.Any())
                    {
                        model.WorkingDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog bilgileri yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Psikolog bilgileri yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /PublicAppointment/GetAvailableSlots - AJAX endpoint
        [HttpGet]
        [Route("PublicAppointment/GetAvailableSlots")]
        [Route("Admin/PublicAppointment/GetAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(int psychologistId, string date, int duration)
        {
            try
            {
                _logger.LogInformation("GetAvailableSlots called: PsychologistId={PsychologistId}, Date={Date}, Duration={Duration}", 
                    psychologistId, date, duration);
                
                // Validate inputs
                if (psychologistId <= 0)
                {
                    _logger.LogWarning("Invalid psychologistId: {PsychologistId}", psychologistId);
                    return Json(new { success = false, message = "Geçersiz psikolog ID'si" });
                }
                
                if (string.IsNullOrEmpty(date))
                {
                    _logger.LogWarning("Date is null or empty");
                    return Json(new { success = false, message = "Tarih seçilmedi" });
                }
                
                DateTime appointmentDate;
                if (!DateTime.TryParse(date, out appointmentDate))
                {
                    _logger.LogWarning("Invalid date format: {Date}", date);
                    return Json(new { success = false, message = "Geçersiz tarih formatı" });
                }
                
                if (appointmentDate < DateTime.Today)
                {
                    _logger.LogWarning("Date is in the past: {Date:yyyy-MM-dd}", appointmentDate);
                    return Json(new { success = false, message = "Geçmiş bir tarih seçilemez" });
                }
                
                if (duration != 50 && duration != 90 && duration != 120)
                {
                    _logger.LogWarning("Invalid duration: {Duration}", duration);
                    return Json(new { success = false, message = "Geçersiz süre değeri" });
                }
                
                // API artık doğru formatı döndürüyor: {success: true/false, slots: [...] veya message: "..."}
                var response = await _appointmentService.GetAvailableSlotsAsync(psychologistId, appointmentDate, duration);
                
                _logger.LogInformation("GetAvailableSlots response: Success={Success}, DataCount={Count}", 
                    response.Success, response.Data?.Count ?? 0);
                
                if (response.Success && response.Data != null && response.Data.Count > 0)
                {
                    _logger.LogInformation("Found {Count} available slots", response.Data.Count);
                    
                    // Format the DateTime slots to match frontend expectations
                    var slots = response.Data.Select(s => new
                    {
                        time = s.ToString("HH:mm"),
                        value = s.ToString("yyyy-MM-ddTHH:mm:ss")
                    }).ToList();
                    
                    return Json(new { success = true, slots = slots });
                }
                else
                {
                    _logger.LogWarning("No available slots found for psychologist {PsychologistId} on {Date:yyyy-MM-dd}", 
                        psychologistId, appointmentDate);
                    
                    return Json(new 
                    { 
                        success = false,
                        message = response.Message ?? "Bu tarih için uygun saat bulunmamaktadır. Psikolog bu günde çalışmıyor olabilir veya tüm saatler dolu olabilir."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uygun saatler yüklenirken hata oluştu - PsychologistId: {PsychologistId}, Date: {Date}, Duration: {Duration}", 
                    psychologistId, date, duration);
                return Json(new 
                { 
                    success = false, 
                    message = "Uygun saatler yüklenirken bir hata oluştu: " + ex.Message 
                });
            }
        }

        // GET: /PublicAppointment/ConfirmAppointment - Randevu Özeti
        [HttpGet]
        public async Task<IActionResult> ConfirmAppointment(int psychologistId, DateTime appointmentDate, int duration, bool isOnline)
        {
            ViewData["PageTitle"] = "Randevu Onayı";

            var model = new AppointmentConfirmViewModel
            {
                AppointmentDate = appointmentDate,
                Duration = duration,
                IsOnline = isOnline
            };

            try
            {
                var response = await _psychologistService.GetByIdAsync(psychologistId);
                if (response.Success && response.Data != null)
                {
                    model.Psychologist = response.Data;
                    model.TotalFee = 0; // Artık ücret bilgisi yok
                }
                else
                {
                    TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog bilgileri yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Psikolog bilgileri yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: /PublicAppointment/CreateAppointment - Randevu Oluşturma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(
            int psychologistId,
            DateTime appointmentDate,
            int duration,
            bool isOnline,
            string? clientNotes,
            bool kvkkConsent,
            string clientFirstName,
            string clientLastName,
            string clientEmail,
            string clientPhone,
            string preferredNotification = "WhatsApp")
        {
            _logger.LogInformation("CreateAppointment called - PsychologistId: {PsychologistId}, Date: {Date}, Duration: {Duration}, IsOnline: {IsOnline}, KvkkConsent: {KvkkConsent}, Client: {FirstName} {LastName}, Email: {Email}, PreferredNotification: {PreferredNotification}",
                psychologistId, appointmentDate, duration, isOnline, kvkkConsent, clientFirstName, clientLastName, clientEmail, preferredNotification);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(clientFirstName) || string.IsNullOrWhiteSpace(clientLastName) || 
                string.IsNullOrWhiteSpace(clientEmail) || string.IsNullOrWhiteSpace(clientPhone))
            {
                _logger.LogWarning("Missing required client information");
                TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurunuz.";
                return RedirectToAction(nameof(ConfirmAppointment), new
                {
                    psychologistId = psychologistId,
                    appointmentDate = appointmentDate,
                    duration = duration,
                    isOnline = isOnline
                });
            }

            if (!kvkkConsent)
            {
                _logger.LogWarning("KVKK consent not provided");
                TempData["ErrorMessage"] = "KVKK onayı zorunludur.";
                return RedirectToAction(nameof(ConfirmAppointment), new
                {
                    psychologistId = psychologistId,
                    appointmentDate = appointmentDate,
                    duration = duration,
                    isOnline = isOnline
                });
            }

            try
            {
                ClientDto? client = null;
                
                // Try to find existing client by email
                var clientsResponse = await _clientService.GetAllAsync();
                if (clientsResponse.Success && clientsResponse.Data != null)
                {
                    client = clientsResponse.Data.FirstOrDefault(c => 
                        c.User?.Email?.Equals(clientEmail, StringComparison.OrdinalIgnoreCase) == true);
                }

                // If client doesn't exist, we need to create a new user and client
                if (client == null)
                {
                    _logger.LogInformation("Client not found with email {Email}, creating new user and client", clientEmail);
                    
                    // Create new user first
                    var newUser = new UserDto
                    {
                        FirstName = clientFirstName,
                        LastName = clientLastName,
                        Email = clientEmail,
                        PhoneNumber = clientPhone,
                        Role = "Client",
                        IsActive = true,
                        DateOfBirth = null
                    };

                    // Note: You'll need to add a CreateUserAsync method to your user service
                    // For now, we'll create the client with the assumption that the API handles user creation
                    // or we need to call the User API endpoint directly
                    
                    var newClient = new ClientDto
                    {
                        User = newUser,
                        AssignedPsychologistId = psychologistId, // Randevu talebinde bulunduğu psikoloğa ata
                        KvkkConsent = kvkkConsent,
                        KvkkConsentDate = DateTime.Now,
                        IsActive = true,
                        PreferredNotificationMethod = preferredNotification, // WhatsApp veya Email
                        Notes = $"Randevu talebi ile oluşturuldu - {DateTime.Now:yyyy-MM-dd HH:mm}"
                    };

                    var createClientResponse = await _clientService.CreateAsync(newClient);
                    
                    if (!createClientResponse.Success || createClientResponse.Data == null)
                    {
                        _logger.LogError("Failed to create new client: {Message}", createClientResponse.Message);
                        TempData["ErrorMessage"] = "Danışan kaydı oluşturulamadı. Lütfen tekrar deneyiniz.";
                        return RedirectToAction(nameof(ConfirmAppointment), new
                        {
                            psychologistId = psychologistId,
                            appointmentDate = appointmentDate,
                            duration = duration,
                            isOnline = isOnline
                        });
                    }
                    
                    client = createClientResponse.Data;
                    _logger.LogInformation("New client created with ID: {ClientId}", client.Id);
                }
                else
                {
                    _logger.LogInformation("Existing client found with ID: {ClientId}", client.Id);
                }

                // Create appointment with "Pending" status (frontend customer request)
                var appointmentDto = new AppointmentDto
                {
                    PsychologistId = psychologistId,
                    ClientId = client.Id,
                    AppointmentDate = appointmentDate,
                    Duration = duration,
                    IsOnline = isOnline,
                    ClientNotes = clientNotes,
                    Status = "Pending" // Frontend customer requests always start as Pending
                };

                var response = await _appointmentService.CreateAsync(appointmentDto);

                if (response.Success)
                {
                    _logger.LogInformation("Appointment created successfully with ID: {AppointmentId}", response.Data?.Id);
                    TempData["SuccessMessage"] = "Randevu talebiniz başarıyla oluşturuldu! Psikolog ve süper admin onayından sonra randevunuz kesinleşecektir. Tarafınıza bilgilendirme yapılacaktır.";
                    
                    // Redirect to a success page instead of MyAppointments (since user might not be logged in)
                    return RedirectToAction(nameof(AppointmentRequestSuccess), new { appointmentId = response.Data?.Id });
                }
                else
                {
                    _logger.LogError("Failed to create appointment: {Message}", response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Randevu oluşturulamadı.";
                    return RedirectToAction(nameof(ConfirmAppointment), new
                    {
                        psychologistId = psychologistId,
                        appointmentDate = appointmentDate,
                        duration = duration,
                        isOnline = isOnline
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu oluşturulurken hata oluştu");
                TempData["ErrorMessage"] = "Randevu oluşturulurken bir hata oluştu. Lütfen tekrar deneyiniz.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /PublicAppointment/MyAppointments - Kullanıcının Randevuları
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            ViewData["PageTitle"] = "Randevularım";

            var model = new MyAppointmentsViewModel();

            try
            {
                // Session'dan kullanıcı bilgilerini al
                var userId = HttpContext.Session.GetUserId();
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Randevularınızı görmek için giriş yapmalısınız.";
                    return RedirectToAction("Login", "Account");
                }

                // Client ID'yi bul
                var clientsResponse = await _clientService.GetAllAsync();
                var client = clientsResponse.Data?.FirstOrDefault(c => c.UserId == userId.Value);

                if (client == null)
                {
                    TempData["ErrorMessage"] = "Danışan kaydı bulunamadı.";
                    return View(model);
                }

                // Tüm randevuları çek
                var appointmentsResponse = await _appointmentService.GetAllAsync();
                if (appointmentsResponse.Success && appointmentsResponse.Data != null)
                {
                    var userAppointments = appointmentsResponse.Data
                        .Where(a => a.ClientId == client.Id)
                        .OrderByDescending(a => a.AppointmentDate)
                        .ToList();

                    var now = DateTime.Now;

                    model.UpcomingAppointments = userAppointments
                        .Where(a => a.AppointmentDate > now && a.Status != "Cancelled")
                        .ToList();

                    model.PastAppointments = userAppointments
                        .Where(a => a.AppointmentDate <= now || a.Status == "Completed")
                        .ToList();

                    model.PendingAppointments = userAppointments
                        .Where(a => a.Status == "Pending")
                        .ToList();

                    model.CancelledAppointments = userAppointments
                        .Where(a => a.Status == "Cancelled")
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevular yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Randevular yüklenirken bir hata oluştu.";
            }

            return View(model);
        }

        // GET: /PublicAppointment/AppointmentRequestSuccess - Randevu Talebi Başarılı
        [HttpGet]
        public IActionResult AppointmentRequestSuccess(int? appointmentId)
        {
            ViewData["PageTitle"] = "Randevu Talebi Başarılı";
            ViewBag.AppointmentId = appointmentId;
            return View();
        }

        // POST: /PublicAppointment/CancelAppointment - Randevu İptal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int id, string reason)
        {
            try
            {
                var response = await _appointmentService.CancelAsync(id, reason);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Randevunuz iptal edildi.";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Randevu iptal edilemedi.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu iptal edilirken hata oluştu");
                TempData["ErrorMessage"] = "Randevu iptal edilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(MyAppointments));
        }
    }
}
