using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class ClientController : Controller
    {
        private readonly IApiClientService _clientService;
        private readonly IApiPsychologistService _psychologistService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IApiClientService clientService,
            IApiPsychologistService psychologistService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _psychologistService = psychologistService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Danışan Yönetimi";
            
            try
            {
                var response = await _clientService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }
                
                TempData["ErrorMessage"] = response.Message;
                return View(new List<ClientDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışanlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<ClientDto>());
            }
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["PageTitle"] = "Yeni Danışan";
            await LoadPsychologists();
            return View();
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                await LoadPsychologists();
                return View(model);
            }

            try
            {
                // User bilgilerini kontrol et
                if (model.User == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri eksik.";
                    await LoadPsychologists();
                    return View(model);
                }

                var response = await _clientService.CreateAsync(model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Danışan başarıyla oluşturuldu.";
                    return RedirectToAction("Index", "Client");
                }

                TempData["ErrorMessage"] = response.Message ?? "Danışan oluşturulamadı.";
                await LoadPsychologists();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan oluşturulurken hata: {Error}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                await LoadPsychologists();
                return View(model);
            }
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["PageTitle"] = "Danışan Düzenle";
            
            try
            {
                var response = await _clientService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    await LoadPsychologists();
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Danışan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan düzenlenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientDto model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem.";
                return RedirectToAction("Index", "Client");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                await LoadPsychologists();
                return View(model);
            }

            try
            {
                var response = await _clientService.UpdateAsync(id, model);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Danışan başarıyla güncellendi.";
                    return RedirectToAction("Index", "Client");
                }

                TempData["ErrorMessage"] = response.Message ?? "Danışan güncellenemedi.";
                await LoadPsychologists();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan güncellenirken hata: {Error}", ex.Message);
                TempData["ErrorMessage"] = $"İşlem sırasında hata oluştu: {ex.Message}";
                await LoadPsychologists();
                return View(model);
            }
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            ViewData["PageTitle"] = "Danışan Detayları";
            
            try
            {
                var response = await _clientService.GetByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = "Danışan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan detayları yüklenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _clientService.DeleteAsync(id);
                if (response.Success)
                {
                    return Json(new { success = true, message = "Danışan başarıyla silindi." });
                }

                return Json(new { success = false, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Danışan silinirken hata");
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        private async Task LoadPsychologists()
        {
            var response = await _psychologistService.GetAllAsync();
            if (response.Success && response.Data != null)
            {
                ViewBag.Psychologists = response.Data;
            }
            else
            {
                ViewBag.Psychologists = new List<PsychologistDto>();
            }
        }
    }
}
