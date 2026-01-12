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

            // Çalışma saatleri kontrolü - buffer süresini almak için önce al
            var appointmentDayOfWeek = appointment.AppointmentDate.DayOfWeek == DayOfWeek.Sunday
                ? WeekDay.Sunday
                : (WeekDay)((int)appointment.AppointmentDate.DayOfWeek);
            
            var workingHour = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(
                appointment.PsychologistId, 
                appointmentDayOfWeek);

            if (workingHour == null || !workingHour.IsAvailable)
                throw new Exception("Psikolog bu günde çalışmamaktadır.");

            // Buffer süresini WorkingHour'dan al
            appointment.BreakDuration = workingHour.BufferDuration;

            // Randevu bitiş saatini hesapla (süre + buffer süresi)
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

            // Randevu, çalışma saatleri içinde mi kontrol et
            var appointmentTime = appointment.AppointmentDate.TimeOfDay;
            if (appointmentTime < workingHour.StartTime || appointment.AppointmentEndDate.TimeOfDay > workingHour.EndTime)
                throw new Exception("Randevu saatleri çalışma saatleri dışındadır.");

            // Mola saatleri kontrolü - SADECE mola zamanı (mola bitince hemen randevu başlayabilir)
            bool isInBreakTime = workingHour.BreakTimes.Any(b =>
            {
                // Randevunun herhangi bir kısmı mola zamanına denk geliyorsa
                return (appointment.AppointmentDate.TimeOfDay >= b.StartTime && appointment.AppointmentDate.TimeOfDay < b.EndTime) ||
                       (appointment.AppointmentEndDate.TimeOfDay > b.StartTime && appointment.AppointmentEndDate.TimeOfDay <= b.EndTime) ||
                       (appointment.AppointmentDate.TimeOfDay <= b.StartTime && appointment.AppointmentEndDate.TimeOfDay > b.StartTime);
            });

            if (isInBreakTime)
                throw new Exception("Randevu saatleri psikologun mola saatlerine denk gelmektedir.");

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

            // Çalışma saatlerinden buffer süresini al
            var appointmentDayOfWeek = appointment.AppointmentDate.DayOfWeek == DayOfWeek.Sunday
                ? WeekDay.Sunday
                : (WeekDay)((int)appointment.AppointmentDate.DayOfWeek);
            
            var workingHour = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(
                appointment.PsychologistId, 
                appointmentDayOfWeek);

            if (workingHour != null)
            {
                appointment.BreakDuration = workingHour.BufferDuration;
            }

            // Randevu bitiş saatini hesapla (süre + buffer süresi)
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

            // Randevu onaylandığında, danışanın atanmış psikoloğunu güncelle
            if (status == AppointmentStatus.Confirmed && appointment.ClientId > 0)
            {
                var client = await _unitOfWork.ClientRepository.GetByIdAsync(appointment.ClientId);
                if (client != null && client.AssignedPsychologistId != appointment.PsychologistId)
                {
                    client.AssignedPsychologistId = appointment.PsychologistId;
                    _unitOfWork.ClientRepository.Update(client);
                }
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
            int bufferDuration = workingHour.BufferDuration; // Psikologun buffer süresi
            int slotInterval = 1; // Her dakika kontrol et ama sadece uygun zamanları göster
            
            var currentTime = date.Date.Add(workingHour.StartTime);
            var endTime = date.Date.Add(workingHour.EndTime);

            while (currentTime.AddMinutes(slotDuration) <= endTime)
            {
                // Sadece 5'in katı saatleri göster (örn: 10:00, 10:05, 10:10, 10:15...)
                if (currentTime.Minute % 5 != 0)
                {
                    currentTime = currentTime.AddMinutes(slotInterval);
                    continue;
                }
                
                // Slot kontrolü için sadece randevu süresi (buffer AppointmentEndDate'te zaten var)
                var slotEnd = currentTime.AddMinutes(slotDuration);
                
                // Slot bitiş saati çalışma saatini geçmemeli
                if (slotEnd > endTime)
                    break;
                
                // Mola saatleri kontrolü - SADECE mola zamanı (mola bitince hemen randevu başlayabilir)
                bool isInBreakTime = workingHour.BreakTimes.Any(b =>
                {
                    var breakStart = date.Date.Add(b.StartTime);
                    var breakEnd = date.Date.Add(b.EndTime);
                    
                    // Randevunun herhangi bir kısmı mola zamanına denk geliyorsa
                    return (currentTime >= breakStart && currentTime < breakEnd) ||
                           (slotEnd > breakStart && slotEnd <= breakEnd) ||
                           (currentTime <= breakStart && slotEnd > breakStart);
                });

                // Mola zamanı değilse diğer kontrollere geç
                if (!isInBreakTime)
                {
                    // Çakışma kontrolü - AppointmentEndDate'e GÜVENME, DİNAMİK HESAPLA!
                    // Mevcut randevuların gerçek bitiş saatini hesapla (buffer dahil)
                    bool hasConflict = appointments.Any(a => 
                    {
                        if (a.Status == AppointmentStatus.Cancelled)
                            return false;
                        
                        // Gerçek bitiş saati = Başlangıç + Duration + BreakDuration
                        var actualEndDate = a.AppointmentDate.AddMinutes((int)a.Duration + a.BreakDuration);
                        
                        // Yeni slot başlangıcı, mevcut randevunun bitişinden ÖNCE mi?
                        return currentTime < actualEndDate;
                    });

                    if (!hasConflict)
                    {
                        if (!await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(
                            psychologistId, currentTime, slotEnd))
                        {
                            availableSlots.Add(currentTime);
                        }
                    }
                }

                // Sabit aralıklarla ilerle (10 dakika) - böylece her olası başlangıç saatini kontrol ederiz
                currentTime = currentTime.AddMinutes(slotInterval);
            }

            return availableSlots;
        }
    }
}
