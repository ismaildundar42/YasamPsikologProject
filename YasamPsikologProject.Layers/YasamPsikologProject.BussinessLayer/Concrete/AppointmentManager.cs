using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class AppointmentManager : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AppointmentManager(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _unitOfWork.AppointmentRepository.GetAllAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(psychologistId, startDate, endDate);
        }

        public async Task<IEnumerable<Appointment>> GetByClientAsync(int clientId)
        {
            return await _unitOfWork.AppointmentRepository.GetByClientAsync(clientId);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int psychologistId, int days = 7)
        {
            return await _unitOfWork.AppointmentRepository.GetUpcomingAppointmentsAsync(psychologistId, days);
        }

        public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _unitOfWork.AppointmentRepository.GetPendingAppointmentsAsync();
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            // Client kontrolü
            var client = await _unitOfWork.ClientRepository.GetByIdAsync(appointment.ClientId);
            if (client == null)
                throw new Exception("Danışan bulunamadı.");

            // Psychologist kontrolü
            var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(appointment.PsychologistId);
            if (psychologist == null)
                throw new Exception("Psikolog bulunamadı.");

            // Randevu bitiş saatini hesapla (süre + ara süre)
            int durationMinutes = (int)appointment.Duration;
            int totalMinutes = durationMinutes + appointment.BreakDuration;
            appointment.AppointmentEndDate = appointment.AppointmentDate.AddMinutes(totalMinutes);

            // Çakışma kontrolü
            if (await _unitOfWork.AppointmentRepository.HasConflictAsync(
                appointment.PsychologistId, 
                appointment.AppointmentDate, 
                appointment.AppointmentEndDate))
            {
                throw new Exception("Bu saatte zaten bir randevu bulunmaktadır.");
            }

            // Çalışma saatleri kontrolü
            var appointmentDayOfWeek = appointment.AppointmentDate.DayOfWeek == DayOfWeek.Sunday
                ? WeekDay.Sunday
                : (WeekDay)((int)appointment.AppointmentDate.DayOfWeek);
            
            var workingHour = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(
                appointment.PsychologistId, 
                appointmentDayOfWeek);

            if (workingHour == null || !workingHour.IsAvailable)
                throw new Exception("Psikolog bu günde çalışmamaktadır.");

            var appointmentTime = appointment.AppointmentDate.TimeOfDay;
            if (appointmentTime < workingHour.StartTime || appointment.AppointmentEndDate.TimeOfDay > workingHour.EndTime)
                throw new Exception("Randevu saatleri çalışma saatleri dışındadır.");

            // İzin günü kontrolü
            if (await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(
                appointment.PsychologistId,
                appointment.AppointmentDate,
                appointment.AppointmentEndDate))
            {
                throw new Exception("Psikolog bu tarihte müsait değildir.");
            }

            appointment.Status = AppointmentStatus.Pending;
            await _unitOfWork.AppointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // Bildirim gönder
            try
            {
                await _notificationService.SendAppointmentConfirmationAsync(appointment);
            }
            catch
            {
                // Bildirim hatası randevu oluşturmayı engellemez
            }

            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            var existing = await _unitOfWork.AppointmentRepository.GetByIdAsync(appointment.Id);
            if (existing == null)
                throw new Exception("Randevu bulunamadı.");

            // Randevu bitiş saatini hesapla (süre + ara süre)
            int durationMinutes = (int)appointment.Duration;
            int totalMinutes = durationMinutes + appointment.BreakDuration;
            appointment.AppointmentEndDate = appointment.AppointmentDate.AddMinutes(totalMinutes);

            // Çakışma kontrolü (kendi randevusu hariç)
            if (await _unitOfWork.AppointmentRepository.HasConflictAsync(
                appointment.PsychologistId,
                appointment.AppointmentDate,
                appointment.AppointmentEndDate,
                appointment.Id))
            {
                throw new Exception("Bu saatte zaten bir randevu bulunmaktadır.");
            }

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateStatusAsync(int id, AppointmentStatus status, string? reason = null)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Randevu bulunamadı.");

            appointment.Status = status;
            if (status == AppointmentStatus.Cancelled && !string.IsNullOrWhiteSpace(reason))
            {
                appointment.CancellationReason = reason;
                appointment.CancelledAt = DateTime.UtcNow;
            }

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
            return appointment;
        }

        public async Task CancelAsync(int id, string reason)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Randevu bulunamadı.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = reason;
            appointment.CancelledAt = DateTime.UtcNow;

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            // İptal bildirimi gönder
            try
            {
                await _notificationService.SendAppointmentCancellationAsync(appointment, reason);
            }
            catch
            {
                // Bildirim hatası işlemi engellemez
            }
        }

        public async Task<bool> HasConflictAsync(int psychologistId, DateTime startDate, DateTime endDate, int? excludeAppointmentId = null)
        {
            return await _unitOfWork.AppointmentRepository.HasConflictAsync(psychologistId, startDate, endDate, excludeAppointmentId);
        }

        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int psychologistId, DateTime date, AppointmentDuration duration)
        {
            var availableSlots = new List<DateTime>();
            // C# DayOfWeek: Sunday=0, Monday=1, ... Saturday=6
            // Our WeekDay: Monday=1, Tuesday=2, ... Sunday=7
            var dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday 
                ? WeekDay.Sunday 
                : (WeekDay)((int)date.DayOfWeek);

            var workingHour = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(psychologistId, dayOfWeek);
            if (workingHour == null || !workingHour.IsAvailable)
                return availableSlots;

            var appointments = await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(
                psychologistId, 
                date.Date, 
                date.Date.AddDays(1));

            int slotDuration = (int)duration;
            int breakDuration = 10; // Varsayılan ara süre
            int totalSlotDuration = slotDuration + breakDuration; // Randevu + ara süre
            
            var currentTime = date.Date.Add(workingHour.StartTime);
            var endTime = date.Date.Add(workingHour.EndTime);

            while (currentTime.AddMinutes(slotDuration) <= endTime)
            {
                var slotEnd = currentTime.AddMinutes(slotDuration);
                var slotEndWithBreak = currentTime.AddMinutes(totalSlotDuration);
                
                // Çakışma kontrolü - ara süre dahil
                bool hasConflict = appointments.Any(a => 
                    a.Status != AppointmentStatus.Cancelled &&
                    ((currentTime >= a.AppointmentDate && currentTime < a.AppointmentEndDate) ||
                     (slotEndWithBreak > a.AppointmentDate && slotEndWithBreak <= a.AppointmentEndDate) ||
                     (currentTime <= a.AppointmentDate && slotEndWithBreak >= a.AppointmentEndDate)));

                if (!hasConflict)
                {
                    if (!await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(
                        psychologistId, currentTime, slotEnd))
                    {
                        availableSlots.Add(currentTime);
                    }
                }

                // Bir sonraki slot için ara süre dahil ilerle
                currentTime = currentTime.AddMinutes(totalSlotDuration);
            }

            return availableSlots;
        }
    }
}
