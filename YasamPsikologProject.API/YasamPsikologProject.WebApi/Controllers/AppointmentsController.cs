using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;
using YasamPsikologProject.WebApi.DTOs;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? psychologistId, [FromQuery] string? status)
        {
            var appointments = await _appointmentService.GetAllAsync();
            
            // Psikolog filtreleme
            if (psychologistId.HasValue && psychologistId.Value > 0)
            {
                appointments = appointments.Where(a => a.PsychologistId == psychologistId.Value).ToList();
            }
            
            // Durum filtreleme
            if (!string.IsNullOrEmpty(status))
            {
                var statusList = status.Split(',', StringSplitOptions.RemoveEmptyEntries);
                appointments = appointments.Where(a => statusList.Contains(a.Status.ToString(), StringComparer.OrdinalIgnoreCase)).ToList();
            }
            
            var appointmentDtos = appointments.Select(a => new
            {
                a.Id,
                a.PsychologistId,
                Psychologist = a.Psychologist != null ? new
                {
                    a.Psychologist.Id,
                    a.Psychologist.UserId,
                    User = a.Psychologist.User != null ? new
                    {
                        a.Psychologist.User.Id,
                        a.Psychologist.User.FirstName,
                        a.Psychologist.User.LastName,
                        a.Psychologist.User.Email,
                        a.Psychologist.User.PhoneNumber
                    } : null,
                    a.Psychologist.CalendarColor
                } : null,
                a.ClientId,
                Client = a.Client != null ? new
                {
                    a.Client.Id,
                    a.Client.UserId,
                    User = a.Client.User != null ? new
                    {
                        a.Client.User.Id,
                        a.Client.User.FirstName,
                        a.Client.User.LastName,
                        a.Client.User.Email,
                        a.Client.User.PhoneNumber
                    } : null
                } : null,
                a.AppointmentDate,
                a.AppointmentEndDate,
                Duration = (int)a.Duration,
                Status = a.Status.ToString(),
                Notes = a.PsychologistNotes,
                a.CancellationReason,
                a.CancelledAt,
                a.ReminderSent,
                a.MeetingLink
            });
            return Ok(appointmentDtos);
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
            
            var appointmentDtos = appointments.Select(a => new
            {
                a.Id,
                a.PsychologistId,
                Psychologist = a.Psychologist != null ? new
                {
                    a.Psychologist.Id,
                    a.Psychologist.UserId,
                    User = a.Psychologist.User != null ? new
                    {
                        a.Psychologist.User.Id,
                        a.Psychologist.User.FirstName,
                        a.Psychologist.User.LastName,
                        a.Psychologist.User.Email,
                        a.Psychologist.User.PhoneNumber
                    } : null,
                    a.Psychologist.CalendarColor
                } : null,
                a.ClientId,
                Client = a.Client != null ? new
                {
                    a.Client.Id,
                    a.Client.UserId,
                    User = a.Client.User != null ? new
                    {
                        a.Client.User.Id,
                        a.Client.User.FirstName,
                        a.Client.User.LastName,
                        a.Client.User.Email,
                        a.Client.User.PhoneNumber
                    } : null
                } : null,
                a.AppointmentDate,
                a.AppointmentEndDate,
                Duration = (int)a.Duration,
                a.IsOnline,
                Status = a.Status.ToString(), // Enum'u string'e çevir
                ClientNotes = a.ClientNotes,
                PsychologistNotes = a.PsychologistNotes,
                Notes = a.PsychologistNotes,
                a.CancellationReason,
                a.CancelledAt,
                a.ReminderSent,
                a.MeetingLink,
                a.CreatedAt,
                a.UpdatedAt
            }).ToList();
            
            return Ok(appointmentDtos);
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
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                _logger.LogInformation("Creating appointment for Psychologist: {PsychologistId}, Client: {ClientId}, Date: {Date}, Duration: {Duration}", 
                    dto.PsychologistId, dto.ClientId, dto.AppointmentDate, dto.Duration);

                // Duration ve Status enum dönüşümü
                AppointmentDuration duration = dto.Duration switch
                {
                    50 => AppointmentDuration.Duration50,
                    90 => AppointmentDuration.Duration90,
                    120 => AppointmentDuration.Duration120,
                    _ => AppointmentDuration.Duration50
                };

                AppointmentStatus status = dto.Status?.ToLower() switch
                {
                    "confirmed" => AppointmentStatus.Confirmed,
                    "cancelled" => AppointmentStatus.Cancelled,
                    "completed" => AppointmentStatus.Completed,
                    _ => AppointmentStatus.Pending
                };

                var appointment = new Appointment
                {
                    PsychologistId = dto.PsychologistId,
                    ClientId = dto.ClientId,
                    AppointmentDate = dto.AppointmentDate,
                    Duration = duration,
                    Status = status,
                    PsychologistNotes = dto.Notes
                    // BreakDuration ve AppointmentEndDate AppointmentManager tarafından set edilecek
                };

                var createdAppointment = await _appointmentService.CreateAsync(appointment);
                return CreatedAtAction(nameof(GetById), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                _logger.LogInformation("Updating appointment {Id}", id);

                // Mevcut randevuyu al
                var existingAppointment = await _appointmentService.GetByIdAsync(id);
                if (existingAppointment == null)
                    return NotFound(new { message = "Randevu bulunamadı." });

                // Duration enum dönüşümü
                AppointmentDuration duration = dto.Duration switch
                {
                    50 => AppointmentDuration.Duration50,
                    90 => AppointmentDuration.Duration90,
                    120 => AppointmentDuration.Duration120,
                    _ => AppointmentDuration.Duration50
                };

                // Status enum dönüşümü
                AppointmentStatus status = dto.Status?.ToLower() switch
                {
                    "confirmed" => AppointmentStatus.Confirmed,
                    "cancelled" => AppointmentStatus.Cancelled,
                    "completed" => AppointmentStatus.Completed,
                    "pending" => AppointmentStatus.Pending,
                    _ => AppointmentStatus.Pending
                };

                // Güncelleme
                existingAppointment.PsychologistId = dto.PsychologistId;
                existingAppointment.ClientId = dto.ClientId;
                existingAppointment.AppointmentDate = dto.AppointmentDate;
                existingAppointment.Duration = duration;
                existingAppointment.Status = status;
                existingAppointment.PsychologistNotes = dto.Notes;
                // BreakDuration ve AppointmentEndDate AppointmentManager.UpdateAsync tarafından set edilecek

                var updatedAppointment = await _appointmentService.UpdateAsync(existingAppointment);
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AppointmentStatusUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("API UpdateStatus: Updating appointment {Id} status to {Status}, reason: {Reason}", 
                    id, dto.Status, dto.Reason);
                
                // String'i enum'a çevir
                if (!Enum.TryParse<AppointmentStatus>(dto.Status, true, out var statusEnum))
                {
                    _logger.LogWarning("API UpdateStatus: Invalid status value: {Status}", dto.Status);
                    return BadRequest(new { success = false, message = $"Geçersiz durum değeri: {dto.Status}" });
                }
                
                var appointment = await _appointmentService.UpdateStatusAsync(id, statusEnum, dto.Reason);
                
                if (appointment == null)
                {
                    _logger.LogWarning("API UpdateStatus: Appointment {Id} not found", id);
                    return NotFound(new { success = false, message = "Randevu bulunamadı." });
                }
                
                _logger.LogInformation("API UpdateStatus: Successfully updated appointment {Id}", id);
                return Ok(new { 
                    success = true, 
                    message = "Randevu durumu güncellendi.",
                    data = appointment 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API UpdateStatus: Error updating appointment {Id} status", id);
                return BadRequest(new { success = false, message = ex.Message });
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
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int psychologistId, [FromQuery] DateTime date, [FromQuery] int duration, [FromQuery] string? clientEmail = null, [FromQuery] string? clientPhone = null)
        {
            try
            {
                _logger.LogInformation($"GetAvailableSlots called: psychologistId={psychologistId}, date={date:yyyy-MM-dd}, duration={duration}, clientEmail={clientEmail}, clientPhone={clientPhone}");
                
                AppointmentDuration durationEnum;
                
                if (duration == 50)
                    durationEnum = AppointmentDuration.Duration50;
                else if (duration == 90)
                    durationEnum = AppointmentDuration.Duration90;
                else if (duration == 120)
                    durationEnum = AppointmentDuration.Duration120;
                else
                {
                    _logger.LogWarning($"Invalid duration: {duration}");
                    return BadRequest(new { message = "Geçersiz süre değeri. Lütfen 50, 90 veya 120 seçiniz." });
                }

                var slots = await _appointmentService.GetAvailableSlotsAsync(psychologistId, date, durationEnum, clientEmail, clientPhone);
                var slotsList = slots.ToList();
                
                _logger.LogInformation($"Found {slotsList.Count} available slots");
                
                // Always return OK with the list (empty or not)
                return Ok(slotsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAvailableSlots");
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }

    public class AppointmentStatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class CancelAppointmentDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
