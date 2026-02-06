using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IApiSystemSettingService _settingService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(
            IApiSystemSettingService settingService,
            ILogger<SettingsController> logger)
        {
            _settingService = settingService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            ViewData["PageTitle"] = "Sistem Ayarları";

            try
            {
                var response = await _settingService.GetAllAsync();
                if (response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                TempData["ErrorMessage"] = response.Message;
                return View(new List<SystemSettingDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ayarlar listelenirken hata");
                TempData["ErrorMessage"] = "Veriler yüklenirken hata oluştu.";
                return View(new List<SystemSettingDto>());
            }
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] SystemSettingDto setting)
        {
            try
            {
                var response = await _settingService.UpdateAsync(setting);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ayar güncellenirken hata");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
