using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingHoursController : ControllerBase
    {
        private readonly IWorkingHourService _workingHourService;

        public WorkingHoursController(IWorkingHourService workingHourService)
        {
            _workingHourService = workingHourService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workingHour = await _workingHourService.GetByIdAsync(id);
            if (workingHour == null)
                return NotFound(new { message = "Çalışma saati bulunamadı." });

            return Ok(workingHour);
        }

        [HttpGet("psychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId)
        {
            var workingHours = await _workingHourService.GetByPsychologistAsync(psychologistId);
            return Ok(workingHours);
        }

        [HttpGet("psychologist/{psychologistId}/day/{day}")]
        public async Task<IActionResult> GetByPsychologistAndDay(int psychologistId, WeekDay day)
        {
            var workingHour = await _workingHourService.GetByPsychologistAndDayAsync(psychologistId, day);
            if (workingHour == null)
                return NotFound(new { message = "Çalışma saati bulunamadı." });

            return Ok(workingHour);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkingHour workingHour)
        {
            try
            {
                var createdWorkingHour = await _workingHourService.CreateAsync(workingHour);
                return CreatedAtAction(nameof(GetById), new { id = createdWorkingHour.Id }, createdWorkingHour);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkingHour workingHour)
        {
            if (id != workingHour.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var updatedWorkingHour = await _workingHourService.UpdateAsync(workingHour);
                return Ok(updatedWorkingHour);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _workingHourService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
