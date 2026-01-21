using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YasamPsikologProject.WebUi.Models.DTOs;
using YasamPsikologProject.WebUi.Services;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class WorkingHourController : Controller
    {
        private readonly IApiWorkingHourService _workingHourService;
        private readonly IApiPsychologistService _psychologistService;
        private readonly IApiAppointmentService _appointmentService;
        private readonly ILogger<WorkingHourController> _logger;

        public WorkingHourController(
            IApiWorkingHourService workingHourService,
            IApiPsychologistService psychologistService,
            IApiAppointmentService appointmentService,
            ILogger<WorkingHourController> logger)
        {
            _workingHourService = workingHourService;
            _psychologistService = psychologistService;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? psychologistId)
        {
            ViewData["PageTitle"] = "Çalışma Saatleri Yönetimi";

            try
            {
                // Psikolog listesini dropdown için getir
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    var psychologistList = psychologistsResponse.Data.Select(p => new
                    {
                        Id = p.Id,
                        FullName = $"{p.User?.FirstName} {p.User?.LastName}"
                    }).ToList();

                    ViewBag.Psychologists = new SelectList(
                        psychologistList,
                        "Id",
                        "FullName",
                        psychologistId);

                    // Psikolog isimlerini ID ile eşleştirmek için dictionary oluştur
                    ViewBag.PsychologistNames = psychologistList.ToDictionary(p => p.Id, p => p.FullName);
                    
                    // Takvim için de psikolog listesi
                    ViewBag.PsychologistList = psychologistsResponse.Data;
                }

                if (psychologistId.HasValue)
                {
                    var response = await _workingHourService.GetAllByPsychologistAsync(psychologistId.Value);
                    if (response.Success && response.Data != null)
                    {
                        ViewBag.SelectedPsychologistId = psychologistId.Value;
                        return View(response.Data);
                    }
                    TempData["ErrorMessage"] = response.Message;
                    return View(new List<WorkingHourDto>());
                }
                else
                {
                    // Tüm psikologların çalışma saatlerini getir
                    var allWorkingHours = new List<WorkingHourDto>();
                    if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                    {
                        foreach (var psychologist in psychologistsResponse.Data)
                        {
                            var response = await _workingHourService.GetAllByPsychologistAsync(psychologist.Id);
                            if (response.Success && response.Data != null)
                            {
                                allWorkingHours.AddRange(response.Data);
                            }
                        }
                    }
                    return View(allWorkingHours);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saatleri listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<WorkingHourDto>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(int? psychologistId)
        {
            ViewData["PageTitle"] = "Yeni Çalışma Saati Ekle";

            // Psikolog listesini dropdown için getir
            var psychologistsResponse = await _psychologistService.GetAllAsync();
            if (psychologistsResponse.Success && psychologistsResponse.Data != null)
            {
                ViewBag.Psychologists = new SelectList(
                    psychologistsResponse.Data.Select(p => new
                    {
                        Id = p.Id,
                        FullName = $"{p.User?.FirstName} {p.User?.LastName}"
                    }),
                    "Id",
                    "FullName",
                    psychologistId);
                
                // Takvim için de psikolog listesi
                ViewBag.PsychologistList = psychologistsResponse.Data;
            }

            // Haftanın günleri - WeekDay enum string isimleri kullanılıyor (API bunu bekliyor)
            ViewBag.DaysOfWeek = new SelectList(new[]
            {
                new { Value = "Monday", Text = "Pazartesi" },
                new { Value = "Tuesday", Text = "Salı" },
                new { Value = "Wednesday", Text = "Çarşamba" },
                new { Value = "Thursday", Text = "Perşembe" },
                new { Value = "Friday", Text = "Cuma" },
                new { Value = "Saturday", Text = "Cumartesi" },
                new { Value = "Sunday", Text = "Pazar" }
            }, "Value", "Text");

            var model = new WorkingHourDto
            {
                PsychologistId = psychologistId ?? 0,
                IsAvailable = true,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            };

            return View(model);
        }
        
        [HttpGet]
        [Route("GetCalendarEvents")]
        public async Task<IActionResult> GetCalendarEvents()
        {
            try
            {
                // Tüm randevuları getir
                var appointmentsResponse = await _appointmentService.GetAllAsync();
                if (!appointmentsResponse.Success || appointmentsResponse.Data == null)
                {
                    return Json(new List<object>());
                }

                var events = appointmentsResponse.Data.Select(a => new
                {
                    id = a.Id,
                    title = $"{a.Client?.User?.FirstName} {a.Client?.User?.LastName}",
                    start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = a.AppointmentEndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    backgroundColor = a.Psychologist?.CalendarColor ?? "#3788d8",
                    borderColor = a.Psychologist?.CalendarColor ?? "#3788d8",
                    textColor = "#fff",
                    extendedProps = new
                    {
                        psychologist = $"{a.Psychologist?.User?.FirstName} {a.Psychologist?.User?.LastName}",
                        psychologistId = a.PsychologistId,
                        client = $"{a.Client?.User?.FirstName} {a.Client?.User?.LastName}",
                        status = a.Status,
                        notes = a.Notes,
                        cancellationReason = a.CancellationReason
                    }
                }).ToList();

                return Json(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Takvim eventleri alınırken hata");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkingHourDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                _logger.LogWarning("Model validation failed: {Errors}", errors);
                TempData["ErrorMessage"] = $"Form hataları: {errors}";

                // Dropdown'ları yeniden doldur
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    ViewBag.Psychologists = new SelectList(
                        psychologistsResponse.Data.Select(p => new
                        {
                            Id = p.Id,
                            FullName = $"{p.User?.FirstName} {p.User?.LastName}"
                        }),
                        "Id",
                        "FullName",
                        model.PsychologistId);
                }

                ViewBag.DaysOfWeek = new SelectList(new[]
                {
                    new { Value = "Monday", Text = "Pazartesi" },
                    new { Value = "Tuesday", Text = "Salı" },
                    new { Value = "Wednesday", Text = "Çarşamba" },
                    new { Value = "Thursday", Text = "Perşembe" },
                    new { Value = "Friday", Text = "Cuma" },
                    new { Value = "Saturday", Text = "Cumartesi" },
                    new { Value = "Sunday", Text = "Pazar" }
                }, "Value", "Text", model.DayOfWeek);

                return View(model);
            }

            try
            {
                var response = await _workingHourService.CreateAsync(model);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Çalışma saati başarıyla eklendi.";
                    return RedirectToAction(nameof(Index), new { psychologistId = model.PsychologistId });
                }

                TempData["ErrorMessage"] = response.Message ?? "Çalışma saati eklenirken hata oluştu.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati eklenirken hata");
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu.";
                return View(model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Çalışma Saati Düzenle";

            try
            {
                var response = await _workingHourService.GetByIdAsync(id);

                if (!response.Success || response.Data == null)
                {
                    TempData["ErrorMessage"] = "Çalışma saati bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                // Psikolog listesini dropdown için getir
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    ViewBag.Psychologists = new SelectList(
                        psychologistsResponse.Data.Select(p => new
                        {
                            Id = p.Id,
                            FullName = $"{p.User?.FirstName} {p.User?.LastName}"
                        }),
                        "Id",
                        "FullName",
                        response.Data.PsychologistId);
                }

                ViewBag.DaysOfWeek = new SelectList(new[]
        {
            new { Value = "Monday", Text = "Pazartesi" },
            new { Value = "Tuesday", Text = "Salı" },
            new { Value = "Wednesday", Text = "Çarşamba" },
            new { Value = "Thursday", Text = "Perşembe" },
            new { Value = "Friday", Text = "Cuma" },
            new { Value = "Saturday", Text = "Cumartesi" },
            new { Value = "Sunday", Text = "Pazar" }
        }, "Value", "Text", response.Data.DayOfWeek);

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati getirilirken hata: {Id}", id);
                TempData["ErrorMessage"] = "Çalışma saati yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkingHourDto model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID uyuşmazlığı.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Form hataları: {errors}";

                // Dropdown'ları yeniden doldur
                var psychologistsResponse = await _psychologistService.GetAllAsync();
                if (psychologistsResponse.Success && psychologistsResponse.Data != null)
                {
                    ViewBag.Psychologists = new SelectList(
                        psychologistsResponse.Data.Select(p => new
                        {
                            Id = p.Id,
                            FullName = $"{p.User?.FirstName} {p.User?.LastName}"
                        }),
                        "Id",
                        "FullName",
                        model.PsychologistId);
                }

                ViewBag.DaysOfWeek = new SelectList(new[]
                {
                    new { Value = "Monday", Text = "Pazartesi" },
                    new { Value = "Tuesday", Text = "Salı" },
                    new { Value = "Wednesday", Text = "Çarşamba" },
                    new { Value = "Thursday", Text = "Perşembe" },
                    new { Value = "Friday", Text = "Cuma" },
                    new { Value = "Saturday", Text = "Cumartesi" },
                    new { Value = "Sunday", Text = "Pazar" }
                }, "Value", "Text", model.DayOfWeek);

                return View(model);
            }

            try
            {
                var response = await _workingHourService.UpdateAsync(id, model);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Çalışma saati başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index), new { psychologistId = model.PsychologistId });
                }

                TempData["ErrorMessage"] = response.Message ?? "Çalışma saati güncellenirken hata oluştu.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati güncellenirken hata: {Id}", id);
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu.";
                return View(model);
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int psychologistId)
        {
            try
            {
                var response = await _workingHourService.DeleteAsync(id);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Çalışma saati başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Çalışma saati silinirken hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Çalışma saati silinirken hata: {Id}", id);
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index), new { psychologistId });
        }
    }
}
