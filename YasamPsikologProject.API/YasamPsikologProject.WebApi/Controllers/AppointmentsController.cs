using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Randevu bulunamadı." });

            return Ok(appointment);
        }

        [HttpGet("psychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var appointments = await _appointmentService.GetByPsychologistAsync(psychologistId, startDate, endDate);
            return Ok(appointments);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClient(int clientId)
        {
            var appointments = await _appointmentService.GetByClientAsync(clientId);
            return Ok(appointments);
        }

        [HttpGet("upcoming/{psychologistId}")]
        public async Task<IActionResult> GetUpcoming(int psychologistId, [FromQuery] int days = 7)
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(psychologistId, days);
            return Ok(appointments);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var appointments = await _appointmentService.GetPendingAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Appointment appointment)
        {
            try
            {
                var createdAppointment = await _appointmentService.CreateAsync(appointment);
                return CreatedAtAction(nameof(GetById), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var updatedAppointment = await _appointmentService.UpdateAsync(appointment);
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AppointmentStatusUpdateDto dto)
        {
            try
            {
                var appointment = await _appointmentService.UpdateStatusAsync(id, dto.Status, dto.Reason);
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelAppointmentDto dto)
        {
            try
            {
                await _appointmentService.CancelAsync(id, dto.Reason);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-conflict")]
        public async Task<IActionResult> CheckConflict([FromQuery] int psychologistId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? excludeAppointmentId = null)
        {
            var hasConflict = await _appointmentService.HasConflictAsync(psychologistId, startDate, endDate, excludeAppointmentId);
            return Ok(new { hasConflict });
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int psychologistId, [FromQuery] DateTime date, [FromQuery] AppointmentDuration duration)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(psychologistId, date, duration);
            return Ok(slots);
        }
    }

    public class AppointmentStatusUpdateDto
    {
        public AppointmentStatus Status { get; set; }
        public string? Reason { get; set; }
    }

    public class CancelAppointmentDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
