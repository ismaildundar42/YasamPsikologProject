using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.WebUi.Services;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly IApiSystemSettingService _settingService;

        public SystemSettingsController(IApiSystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var response = await _settingService.GetByKeyAsync(key);
            if (response.Success && response.Data != null)
            {
                return Ok(response.Data);
            }
            return NotFound(new { message = "Ayar bulunamadı" });
        }
    }
}
