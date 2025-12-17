using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    public class PsychologistController : Controller
    {
        private readonly IPsychologistService _psychologistService;
        private readonly ILogger<PsychologistController> _logger;

        public PsychologistController(
            IPsychologistService psychologistService,
            ILogger<PsychologistController> logger)
        {
            _psychologistService = psychologistService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Psikolog Yönetimi";
            
            try
            {
                var response = await _psychologistService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }
                
                TempData["ErrorMessage"] = response.Message;
                return View(new List<PsychologistDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikologlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<PsychologistDto>());
            }
        }

        public IActionResult Create()
        {
            ViewData["PageTitle"] = "Yeni Psikolog";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PsychologistDto model)
        {
            try
            {
                var response = await _psychologistService.CreateAsync(model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Psikolog başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog oluşturulurken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Psikolog Düzenle";
            
            try
            {
                var response = await _psychologistService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog düzenlenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PsychologistDto model)
        {
            try
            {
                var response = await _psychologistService.UpdateAsync(id, model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Psikolog başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog güncellenirken hata");
                TempData["ErrorMessage"] = "İşlem sırasında hata oluştu.";
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Psikolog Detayları";
            
            try
            {
                var response = await _psychologistService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Psikolog bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _psychologistService.DeleteAsync(id);
                if (response.Success)
                {
                    return Json(new { success = true, message = "Psikolog başarıyla silindi." });
                }

                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Psikolog silinirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }
    }
}
