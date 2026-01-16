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
            // GEÇMİŞ TARİH/SAAT KONTROLÜ - Geçmiş saatlere randevu alınamaz!
            // Türkiye saat dilimini kullan (UTC+3)
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var currentTimeInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyTimeZone);
            var minimumAppointmentTime = currentTimeInTurkey.AddMinutes(5);
            
            // appointment.AppointmentDate UTC olarak geliyor, Türkiye saatine çevir
            var appointmentDateInTurkey = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, turkeyTimeZone);
            
            if (appointmentDateInTurkey <= minimumAppointmentTime)
            {
                throw new Exception($"Geçmiş tarihe veya saate randevu oluşturamazsınız. Randevu en erken {minimumAppointmentTime:dd.MM.yyyy HH:mm} için oluşturulabilir. (Şu an: {currentTimeInTurkey:HH:mm})");
            }

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

            // Psikolog için çakışma kontrolü
            if (await _unitOfWork.AppointmentRepository.HasConflictAsync(
                appointment.PsychologistId, 
                appointment.AppointmentDate, 
                appointment.AppointmentEndDate))
            {
                throw new Exception("Bu saatte psikolog için zaten bir randevu bulunmaktadır.");
            }

            // Danışan için çakışma kontrolü - aynı danışanın çakışan randevusu olamaz
            if (await _unitOfWork.AppointmentRepository.HasClientConflictAsync(
                appointment.ClientId, 
                appointment.AppointmentDate, 
                appointment.AppointmentEndDate))
            {
                throw new Exception("Bu saatte zaten bir randevunuz bulunmaktadır. Lütfen başka bir saat seçiniz.");
            }

            // MAKSIMUM GÜNLÜK DANIŞAN SAYISI KONTROLÜ
            if (workingHour.MaxDailyPatients.HasValue && workingHour.MaxDailyPatients.Value > 0)
            {
                // O gün için bekleyen + onaylanmış randevuları say
                var appointmentDate = appointment.AppointmentDate.Date;
                var startOfDay = new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, 0, 0, 0, DateTimeKind.Utc);
                var endOfDay = startOfDay.AddDays(1);

                var existingAppointments = await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(
                    appointment.PsychologistId,
                    startOfDay,
                    endOfDay);

                var dailyAppointmentCount = existingAppointments
                    .Count(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed);

                if (dailyAppointmentCount >= workingHour.MaxDailyPatients.Value)
                {
                    throw new Exception($"Psikolog bu gün için maksimum danışan sayısına ulaşmıştır ({workingHour.MaxDailyPatients.Value} danışan). Lütfen başka bir gün seçiniz.");
                }
            }

            // Randevu, çalışma saatleri içinde mi kontrol et (buffer dahil bitiş saati)
            var appointmentTime = appointment.AppointmentDate.TimeOfDay;
            var appointmentEndTime = appointment.AppointmentEndDate.TimeOfDay;
            
            if (appointmentTime < workingHour.StartTime)
                throw new Exception("Randevu başlangıç saati çalışma saatlerinden önce.");
            
            if (appointmentEndTime > workingHour.EndTime)
                throw new Exception("Randevu bitiş saati (buffer dahil) çalışma saatlerinden sonra.");

            // Mola saatleri kontrolü - Randevunun HERHANGİ BİR KISMI mola zamanına denk gelmemeli
            // Ancak randevu tam mola başlangıcında bitebilir (AppointmentEndDate == BreakStartTime)
            bool isInBreakTime = workingHour.BreakTimes.Any(b =>
            {
                // Çakışma kontrolü: Randevu [start, end] ile mola [breakStart, breakEnd] çakışıyor mu?
                // Çakışma YOK ise: end <= breakStart VEYA start >= breakEnd
                // Çakışma VAR ise: start < breakEnd VE end > breakStart
                return appointment.AppointmentDate.TimeOfDay < b.EndTime && 
                       appointment.AppointmentEndDate.TimeOfDay > b.StartTime;
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

            // GEÇMİŞ TARİH/SAAT KONTROLÜ - Geçmiş saatlere taşınamaz!
            // Türkiye saat dilimini kullan (UTC+3)
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var currentTimeInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyTimeZone);
            var minimumAppointmentTime = currentTimeInTurkey.AddMinutes(5);
            
            // appointment.AppointmentDate UTC olarak geliyor, Türkiye saatine çevir
            var appointmentDateInTurkey = TimeZoneInfo.ConvertTimeFromUtc(appointment.AppointmentDate, turkeyTimeZone);
            
            if (appointmentDateInTurkey <= minimumAppointmentTime)
            {
                throw new Exception($"Randevu geçmiş tarihe veya saate taşınamaz. Randevu en erken {minimumAppointmentTime:dd.MM.yyyy HH:mm} için ayarlanabilir. (Şu an: {currentTimeInTurkey:HH:mm})");
            }

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

            // Çalışma saatleri kontrolü
            if (workingHour != null)
            {
                var appointmentTime = appointment.AppointmentDate.TimeOfDay;
                var appointmentEndTime = appointment.AppointmentEndDate.TimeOfDay;
                
                if (appointmentTime < workingHour.StartTime)
                    throw new Exception("Randevu başlangıç saati çalışma saatlerinden önce.");
                
                if (appointmentEndTime > workingHour.EndTime)
                    throw new Exception("Randevu bitiş saati (buffer dahil) çalışma saatlerinden sonra.");
                
                // Mola saatleri kontrolü
                bool isInBreakTime = workingHour.BreakTimes.Any(b =>
                {
                    return appointment.AppointmentDate.TimeOfDay < b.EndTime && 
                           appointment.AppointmentEndDate.TimeOfDay > b.StartTime;
                });

                if (isInBreakTime)
                    throw new Exception("Randevu saatleri psikologun mola saatlerine denk gelmektedir.");
            }
            
            // Psikolog için çakışma kontrolü (kendi randevusu hariç)
            if (await _unitOfWork.AppointmentRepository.HasConflictAsync(
                appointment.PsychologistId,
                appointment.AppointmentDate,
                appointment.AppointmentEndDate,
                appointment.Id))
            {
                throw new Exception("Bu saatte psikolog için zaten bir randevu bulunmaktadır.");
            }

            // Danışan için çakışma kontrolü (kendi randevusu hariç)
            if (await _unitOfWork.AppointmentRepository.HasClientConflictAsync(
                appointment.ClientId,
                appointment.AppointmentDate,
                appointment.AppointmentEndDate,
                appointment.Id))
            {
                throw new Exception("Bu saatte zaten bir randevunuz bulunmaktadır. Lütfen başka bir saat seçiniz.");
            }

            // MAKSIMUM GÜNLÜK DANIŞAN SAYISI KONTROLÜ (tarih değiştiğinde kontrol et)
            if (workingHour != null && workingHour.MaxDailyPatients.HasValue && workingHour.MaxDailyPatients.Value > 0)
            {
                // Sadece tarih değişmişse kontrol et
                if (existing.AppointmentDate.Date != appointment.AppointmentDate.Date)
                {
                    var appointmentDate = appointment.AppointmentDate.Date;
                    var startOfDay = new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, 0, 0, 0, DateTimeKind.Utc);
                    var endOfDay = startOfDay.AddDays(1);

                    var existingAppointments = await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(
                        appointment.PsychologistId,
                        startOfDay,
                        endOfDay);

                    var dailyAppointmentCount = existingAppointments
                        .Where(a => a.Id != appointment.Id) // Kendi randevusunu hariç tut
                        .Count(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed);

                    if (dailyAppointmentCount >= workingHour.MaxDailyPatients.Value)
                    {
                        throw new Exception($"Psikolog bu gün için maksimum danışan sayısına ulaşmıştır ({workingHour.MaxDailyPatients.Value} danışan). Lütfen başka bir gün seçiniz.");
                    }
                }
            }
            
            // İzin günü kontrolü
            if (await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(
                appointment.PsychologistId,
                appointment.AppointmentDate,
                appointment.AppointmentEndDate))
            {
                throw new Exception("Psikolog bu tarihte müsait değildir.");
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

            var oldStatus = appointment.Status;
            
            // İptal edilen randevuyu tekrar onaylarken ÇAKIŞMA KONTROLÜ yap!
            if (oldStatus == AppointmentStatus.Cancelled && 
                (status == AppointmentStatus.Confirmed || status == AppointmentStatus.Pending))
            {
                // Psikolog için çakışma kontrolü
                if (await _unitOfWork.AppointmentRepository.HasConflictAsync(
                    appointment.PsychologistId,
                    appointment.AppointmentDate,
                    appointment.AppointmentEndDate,
                    appointment.Id))
                {
                    throw new Exception("Bu saatte artık psikolog için başka bir randevu bulunmaktadır. İptal edilen randevu tekrar aktif edilemiyor.");
                }

                // Danışan için çakışma kontrolü
                if (await _unitOfWork.AppointmentRepository.HasClientConflictAsync(
                    appointment.ClientId,
                    appointment.AppointmentDate,
                    appointment.AppointmentEndDate,
                    appointment.Id))
                {
                    throw new Exception("Bu saatte artık bir randevunuz bulunmaktadır. İptal edilen randevu tekrar aktif edilemiyor.");
                }
            }

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

        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int psychologistId, DateTime date, AppointmentDuration duration, string? clientEmail = null, string? clientPhone = null)
        {
            var availableSlots = new List<DateTime>();
            
            // Türkiye saat dilimini kullan (UTC+3)
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var currentTimeInTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyTimeZone);
            
            // GEÇMİŞ TARİH KONTROLÜ - Geçmiş günler için müsait saat döndürme
            if (date.Date < currentTimeInTurkey.Date)
            {
                return availableSlots; // Boş liste döndür
            }
            
            // Client ID'yi bul (eğer email/telefon varsa)
            int? clientId = null;
            if (!string.IsNullOrWhiteSpace(clientEmail) || !string.IsNullOrWhiteSpace(clientPhone))
            {
                var clients = await _unitOfWork.ClientRepository.GetAllAsync();
                
                // Önce email'e göre ara
                var client = clients.FirstOrDefault(c => 
                    !string.IsNullOrWhiteSpace(clientEmail) && 
                    c.User?.Email?.Equals(clientEmail, StringComparison.OrdinalIgnoreCase) == true);
                
                // Email yoksa telefona göre ara
                if (client == null && !string.IsNullOrWhiteSpace(clientPhone))
                {
                    client = clients.FirstOrDefault(c => 
                        c.User?.PhoneNumber?.Equals(clientPhone, StringComparison.OrdinalIgnoreCase) == true);
                }
                
                if (client != null)
                {
                    clientId = client.Id;
                }
            }
            
            // C# DayOfWeek: Sunday=0, Monday=1, ... Saturday=6
            // Our WeekDay: Monday=1, Tuesday=2, ... Sunday=7
            var dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday 
                ? WeekDay.Sunday 
                : (WeekDay)((int)date.DayOfWeek);

            var workingHour = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(psychologistId, dayOfWeek);
            if (workingHour == null || !workingHour.IsAvailable)
                return availableSlots;
            
            // MAKSİMUM DANIŞAN SAYISI KONTROLÜ - Sınıra ulaşıldıysa o gün için hiç slot gösterme
            if (workingHour.MaxDailyPatients.HasValue)
            {
                var dayStart = date.Date;
                var dayEnd = date.Date.AddDays(1);
                var existingAppointments = await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(psychologistId, dayStart, dayEnd);
                
                var dailyAppointmentCount = existingAppointments.Count(a => 
                    a.Status == AppointmentStatus.Pending || 
                    a.Status == AppointmentStatus.Confirmed);
                
                if (dailyAppointmentCount >= workingHour.MaxDailyPatients.Value)
                {
                    // Maksimum sayıya ulaşıldı, boş liste döndür
                    return availableSlots;
                }
            }

            var appointments = await _unitOfWork.AppointmentRepository.GetByPsychologistAsync(
                psychologistId, 
                date.Date, 
                date.Date.AddDays(1));
            
            // Eğer clientId varsa, o client'ın randevularını da al
            List<Appointment> clientAppointments = new List<Appointment>();
            if (clientId.HasValue)
            {
                var clientAppts = await _unitOfWork.AppointmentRepository.GetByClientAsync(clientId.Value);
                clientAppointments = clientAppts
                    .Where(a => a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
                    .ToList();
            }

            int slotDuration = (int)duration;
            int bufferDuration = workingHour.BufferDuration; // Psikologun buffer süresi
            int slotInterval = 5; // 5 dakika aralıklarla kontrol et
            
            var currentTime = date.Date.Add(workingHour.StartTime);
            var endTime = date.Date.Add(workingHour.EndTime);
            
            // Eğer bugünse ve başlangıç saati geçmişte kalıyorsa, şu andan itibaren başla
            // En az 5 dakika sonrası için müsait saat göster (Türkiye saati kullan)
            var minimumTime = currentTimeInTurkey.AddMinutes(5);
            
            if (date.Date == currentTimeInTurkey.Date && currentTime < minimumTime)
            {
                // Şimdiki zamandan en az 5 dakika sonraki ilk 5'in katı dakikayı bul
                currentTime = minimumTime;
                int remainder = currentTime.Minute % 5;
                if (remainder != 0)
                {
                    currentTime = currentTime.AddMinutes(5 - remainder);
                }
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
            }

            // While döngüsü: Buffer dahil bitiş saati çalışma saatini geçmemeli
            while (currentTime.AddMinutes(slotDuration + bufferDuration) <= endTime)
            {
                
                // Slot bitiş saati = Randevu süresi + Buffer süresi (gerçek bitiş zamanı)
                var slotEnd = currentTime.AddMinutes(slotDuration);
                var slotEndWithBuffer = currentTime.AddMinutes(slotDuration + bufferDuration);
                
                // Mola saatleri kontrolü
                // ÖNEMLİ: Eğer randevu tam mola başında bitiyorsa (slotEnd == breakStart), 
                // buffer'a gerek yok çünkü psikolog zaten molada olacak
                bool isInBreakTime = workingHour.BreakTimes.Any(b =>
                {
                    var breakStart = date.Date.Add(b.StartTime);
                    var breakEnd = date.Date.Add(b.EndTime);
                    
                    // Randevunun kendisi (buffer hariç) mola ile çakışıyor mu?
                    bool appointmentOverlapsBreak = currentTime < breakEnd && slotEnd > breakStart;
                    
                    // Eğer randevu tam mola başında bitiyorsa, bu kabul edilebilir
                    if (slotEnd == breakStart)
                        return false;
                    
                    // Buffer'lı kontrolü sadece randevu molaya denk gelmiyorsa yap
                    return appointmentOverlapsBreak || (currentTime < breakEnd && slotEndWithBuffer > breakStart);
                });

                // Mola zamanı değilse diğer kontrollere geç
                if (!isInBreakTime)
                {
                    // Psikolog için çakışma kontrolü - Buffer dahil bitiş saatini kullan!
                    bool hasConflict = appointments.Any(a => 
                    {
                        if (a.Status == AppointmentStatus.Cancelled)
                            return false;
                        
                        // Gerçek bitiş saati = Başlangıç + Duration + BreakDuration
                        var actualEndDate = a.AppointmentDate.AddMinutes((int)a.Duration + a.BreakDuration);
                        
                        // İki zaman dilimi çakışıyor mu kontrol et:
                        // Yeni slot (BUFFER DAHİL): [currentTime, slotEndWithBuffer]
                        // Mevcut randevu: [appointmentDate, actualEndDate]
                        return currentTime < actualEndDate && slotEndWithBuffer > a.AppointmentDate;
                    });
                    
                    // Client için çakışma kontrolü (eğer clientId varsa)
                    bool hasClientConflict = false;
                    if (clientId.HasValue && clientAppointments.Any())
                    {
                        hasClientConflict = clientAppointments.Any(a =>
                        {
                            // Gerçek bitiş saati = Başlangıç + Duration + BreakDuration
                            var actualEndDate = a.AppointmentDate.AddMinutes((int)a.Duration + a.BreakDuration);
                            
                            // İki zaman dilimi çakışıyor mu kontrol et
                            return currentTime < actualEndDate && slotEndWithBuffer > a.AppointmentDate;
                        });
                    }

                    if (!hasConflict && !hasClientConflict)
                    {
                        // İzin günü kontrolü - buffer dahil bitiş saatini kullan
                        if (!await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(
                            psychologistId, currentTime, slotEndWithBuffer))
                        {
                            availableSlots.Add(currentTime);
                        }
                    }
                }

                // 5 dakika aralıklarla ilerle
                currentTime = currentTime.AddMinutes(slotInterval);
            }

            return availableSlots;
        }
    }
}
