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
        private readonly ILogger<WorkingHourController> _logger;

        public WorkingHourController(
            IApiWorkingHourService workingHourService,
            IApiPsychologistService psychologistService,
            ILogger<WorkingHourController> logger)
        {
            _workingHourService = workingHourService;
            _psychologistService = psychologistService;
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
                    ViewBag.Psychologists = new SelectList(
                        psychologistsResponse.Data,
                        "Id",
                        "User.FirstName",
                        psychologistId);
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
                }

                return View(new List<WorkingHourDto>());
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
            }

            // Haftanın günleri
            ViewBag.DaysOfWeek = new SelectList(new[]
            {
                new { Value = "Pazartesi", Text = "Pazartesi" },
                new { Value = "Salı", Text = "Salı" },
                new { Value = "Çarşamba", Text = "Çarşamba" },
                new { Value = "Perşembe", Text = "Perşembe" },
                new { Value = "Cuma", Text = "Cuma" },
                new { Value = "Cumartesi", Text = "Cumartesi" },
                new { Value = "Pazar", Text = "Pazar" }
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
                    new { Value = "Pazartesi", Text = "Pazartesi" },
                    new { Value = "Salı", Text = "Salı" },
                    new { Value = "Çarşamba", Text = "Çarşamba" },
                    new { Value = "Perşembe", Text = "Perşembe" },
                    new { Value = "Cuma", Text = "Cuma" },
                    new { Value = "Cumartesi", Text = "Cumartesi" },
                    new { Value = "Pazar", Text = "Pazar" }
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
                    new { Value = "Pazartesi", Text = "Pazartesi" },
                    new { Value = "Salı", Text = "Salı" },
                    new { Value = "Çarşamba", Text = "Çarşamba" },
                    new { Value = "Perşembe", Text = "Perşembe" },
                    new { Value = "Cuma", Text = "Cuma" },
                    new { Value = "Cumartesi", Text = "Cumartesi" },
                    new { Value = "Pazar", Text = "Pazar" }
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
                    new { Value = "Pazartesi", Text = "Pazartesi" },
                    new { Value = "Salı", Text = "Salı" },
                    new { Value = "Çarşamba", Text = "Çarşamba" },
                    new { Value = "Perşembe", Text = "Perşembe" },
                    new { Value = "Cuma", Text = "Cuma" },
                    new { Value = "Cumartesi", Text = "Cumartesi" },
                    new { Value = "Pazar", Text = "Pazar" }
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
