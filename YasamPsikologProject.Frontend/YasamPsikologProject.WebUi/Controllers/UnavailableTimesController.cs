using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Controllers
{
    [Route("Admin/[controller]")]
    public class UnavailableTimesController : Controller
    {
        private readonly IApiUnavailableTimeService _unavailableTimeService;
        private readonly ILogger<UnavailableTimesController> _logger;

        public UnavailableTimesController(
            IApiUnavailableTimeService unavailableTimeService,
            ILogger<UnavailableTimesController> logger)
        {
            _unavailableTimeService = unavailableTimeService;
            _logger = logger;
        }

        [HttpGet("GetByPsychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId)
        {
            try
            {
                var response = await _unavailableTimeService.GetByPsychologistAsync(psychologistId);
                return Json(new { success = response.Success, data = response.Data, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unavailable times for psychologist {PsychologistId}", psychologistId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] UnavailableTimeDto model)
        {
            try
            {
                var response = await _unavailableTimeService.CreateAsync(model);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unavailable time");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _unavailableTimeService.DeleteAsync(id);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unavailable time {Id}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
