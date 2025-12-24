using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;
using YasamPsikologProject.WebApi.DTOs;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var workingHours = await _workingHourService.GetAllAsync();
                var dtos = workingHours.Select(w => new
                {
                    w.Id,
                    w.PsychologistId,
                    DayOfWeek = (int)w.DayOfWeek,
                    StartTime = w.StartTime.ToString(@"hh\:mm"),
                    EndTime = w.EndTime.ToString(@"hh\:mm"),
                    w.IsAvailable,
                    BreakStartTime = w.BreakStartTime?.ToString(@"hh\:mm"),
                    BreakEndTime = w.BreakEndTime?.ToString(@"hh\:mm"),
                    w.Notes
                });
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Çalışma saatleri listelenirken hata: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var workingHour = await _workingHourService.GetByIdAsync(id);
                if (workingHour == null)
                    return NotFound(new { message = "Çalışma saati bulunamadı." });

                var dto = new
                {
                    workingHour.Id,
                    workingHour.PsychologistId,
                    DayOfWeek = (int)workingHour.DayOfWeek,
                    StartTime = workingHour.StartTime.ToString(@"hh\:mm"),
                    EndTime = workingHour.EndTime.ToString(@"hh\:mm"),
                    workingHour.IsAvailable,
                    BreakStartTime = workingHour.BreakStartTime?.ToString(@"hh\:mm"),
                    BreakEndTime = workingHour.BreakEndTime?.ToString(@"hh\:mm"),
                    workingHour.Notes
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Çalışma saati getirilirken hata: {ex.Message}" });
            }
        }

        [HttpGet("psychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId)
        {
            try
            {
                var workingHours = await _workingHourService.GetByPsychologistAsync(psychologistId);
                var dtos = workingHours.Select(w => new
                {
                    w.Id,
                    w.PsychologistId,
                    DayOfWeek = (int)w.DayOfWeek,
                    StartTime = w.StartTime.ToString(@"hh\:mm"),
                    EndTime = w.EndTime.ToString(@"hh\:mm"),
                    w.IsAvailable,
                    BreakStartTime = w.BreakStartTime?.ToString(@"hh\:mm"),
                    BreakEndTime = w.BreakEndTime?.ToString(@"hh\:mm"),
                    w.Notes
                });
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Çalışma saatleri getirilirken hata: {ex.Message}" });
            }
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
        public async Task<IActionResult> Create([FromBody] CreateWorkingHourDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Geçersiz veri.", errors = errors });
            }

            try
            {
                var workingHour = new WorkingHour
                {
                    PsychologistId = dto.PsychologistId,
                    DayOfWeek = Enum.Parse<WeekDay>(dto.DayOfWeek),
                    StartTime = TimeSpan.Parse(dto.StartTime),
                    EndTime = TimeSpan.Parse(dto.EndTime),
                    IsAvailable = dto.IsAvailable,
                    BreakStartTime = !string.IsNullOrEmpty(dto.BreakStartTime) ? TimeSpan.Parse(dto.BreakStartTime) : null,
                    BreakEndTime = !string.IsNullOrEmpty(dto.BreakEndTime) ? TimeSpan.Parse(dto.BreakEndTime) : null,
                    Notes = dto.Notes
                };

                var created = await _workingHourService.CreateAsync(workingHour);
                
                var result = new
                {
                    created.Id,
                    created.PsychologistId,
                    DayOfWeek = (int)created.DayOfWeek,
                    StartTime = created.StartTime.ToString(@"hh\:mm"),
                    EndTime = created.EndTime.ToString(@"hh\:mm"),
                    created.IsAvailable,
                    BreakStartTime = created.BreakStartTime?.ToString(@"hh\:mm"),
                    BreakEndTime = created.BreakEndTime?.ToString(@"hh\:mm"),
                    created.Notes
                };
                
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkingHourDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Geçersiz veri.", errors = errors });
            }

            try
            {
                var existing = await _workingHourService.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Çalışma saati bulunamadı." });

                existing.DayOfWeek = Enum.Parse<WeekDay>(dto.DayOfWeek);
                existing.StartTime = TimeSpan.Parse(dto.StartTime);
                existing.EndTime = TimeSpan.Parse(dto.EndTime);
                existing.IsAvailable = dto.IsAvailable;
                existing.BreakStartTime = !string.IsNullOrEmpty(dto.BreakStartTime) ? TimeSpan.Parse(dto.BreakStartTime) : null;
                existing.BreakEndTime = !string.IsNullOrEmpty(dto.BreakEndTime) ? TimeSpan.Parse(dto.BreakEndTime) : null;
                existing.Notes = dto.Notes;

                var updated = await _workingHourService.UpdateAsync(existing);
                
                var result = new
                {
                    updated.Id,
                    updated.PsychologistId,
                    DayOfWeek = (int)updated.DayOfWeek,
                    StartTime = updated.StartTime.ToString(@"hh\:mm"),
                    EndTime = updated.EndTime.ToString(@"hh\:mm"),
                    updated.IsAvailable,
                    BreakStartTime = updated.BreakStartTime?.ToString(@"hh\:mm"),
                    BreakEndTime = updated.BreakEndTime?.ToString(@"hh\:mm"),
                    updated.Notes
                };
                
                return Ok(result);
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
