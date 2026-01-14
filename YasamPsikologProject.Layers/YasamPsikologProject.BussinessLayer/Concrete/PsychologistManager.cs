using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class PsychologistManager : IPsychologistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PsychologistManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Psychologist?> GetByIdAsync(int id)
        {
            return await _unitOfWork.PsychologistRepository.GetByIdAsync(id);
        }

        public async Task<Psychologist?> GetByUserIdAsync(int userId)
        {
            return await _unitOfWork.PsychologistRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Psychologist>> GetAllActiveAsync()
        {
            return await _unitOfWork.PsychologistRepository.GetActiveWithWorkingHoursAsync();
        }

        public async Task<Psychologist> CreateAsync(Psychologist psychologist)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(psychologist.UserId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (user.Role != EntityLayer.Enums.UserRole.Psychologist)
                throw new Exception("Kullanıcı psikolog rolüne sahip değil.");

            var existing = await _unitOfWork.PsychologistRepository.GetByUserIdAsync(psychologist.UserId);
            if (existing != null)
                throw new Exception("Bu kullanıcı için zaten psikolog kaydı bulunmaktadır.");

            // Renk kontrolü
            if (await IsColorInUseAsync(psychologist.CalendarColor))
                throw new Exception("Bu renk başka bir psikolog tarafından kullanılmaktadır. Lütfen farklı bir renk seçiniz.");

            await _unitOfWork.PsychologistRepository.AddAsync(psychologist);
            await _unitOfWork.SaveChangesAsync();
            return psychologist;
        }

        public async Task<Psychologist> UpdateAsync(Psychologist psychologist)
        {
            var existing = await _unitOfWork.PsychologistRepository.GetByIdAsync(psychologist.Id);
            if (existing == null)
                throw new Exception("Psikolog bulunamadı.");

            // Renk kontrolü (kendi ID'si hariç)
            if (await IsColorInUseAsync(psychologist.CalendarColor, psychologist.Id))
                throw new Exception("Bu renk başka bir psikolog tarafından kullanılmaktadır. Lütfen farklı bir renk seçiniz.");

            _unitOfWork.PsychologistRepository.Update(psychologist);
            await _unitOfWork.SaveChangesAsync();
            return psychologist;
        }

        public async Task DeleteAsync(int id)
        {
            var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(id);
            if (psychologist == null)
                throw new Exception("Psikolog bulunamadı.");

            // İlişkili kayıtları kontrol et
            var appointments = await _unitOfWork.AppointmentRepository.GetAllAsync(a => a.PsychologistId == id);
            var appointmentsList = appointments.ToList();

            var activeAppointments = appointmentsList.Where(a => 
                a.Status != EntityLayer.Enums.AppointmentStatus.Cancelled && 
                a.AppointmentDate > DateTime.Now).ToList();

            // Eğer gelecekte aktif randevusu varsa uyar (isteğe bağlı)
            if (activeAppointments.Any())
            {
                throw new Exception($"Bu psikologun {activeAppointments.Count} adet gelecekteki aktif randevusu var. Önce bu randevuları iptal edin.");
            }

            // Psikolog'u soft delete yap
            _unitOfWork.PsychologistRepository.Delete(psychologist);

            // İlişkili tüm randevuları soft delete yap (geçmiş randevular dahil - arşiv için)
            foreach (var appointment in appointmentsList)
            {
                _unitOfWork.AppointmentRepository.Delete(appointment);
            }

            // İlişkili çalışma saatlerini soft delete yap
            var workingHours = await _unitOfWork.WorkingHourRepository.GetAllAsync(w => w.PsychologistId == id);
            var workingHoursList = workingHours.ToList();
            foreach (var workingHour in workingHoursList)
            {
                _unitOfWork.WorkingHourRepository.Delete(workingHour);
                
                // Bu çalışma saatine ait mola sürelerini de soft delete yap
                var breakTimes = await _unitOfWork.BreakTimeRepository.GetAllAsync(b => b.WorkingHourId == workingHour.Id);
                foreach (var breakTime in breakTimes)
                {
                    _unitOfWork.BreakTimeRepository.Delete(breakTime);
                }
            }

            // İlişkili uygun olmayan zamanları soft delete yap
            var unavailableTimes = await _unitOfWork.UnavailableTimeRepository.GetAllAsync(u => u.PsychologistId == id);
            foreach (var unavailableTime in unavailableTimes)
            {
                _unitOfWork.UnavailableTimeRepository.Delete(unavailableTime);
            }

            // NOT: Danışanları (Clients) silmiyoruz - başka psikologlarla randevuları olabilir
            // AssignedPsychologistId null yapılabilir ama bu opsiyonel

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsColorInUseAsync(string color, int? excludePsychologistId = null)
        {
            var psychologists = await _unitOfWork.PsychologistRepository.GetAllAsync();
            var psychologistsList = psychologists.Where(p => !p.DeletedAt.HasValue).ToList();

            if (excludePsychologistId.HasValue)
            {
                return psychologistsList.Any(p => 
                    p.Id != excludePsychologistId.Value && 
                    p.CalendarColor.Equals(color, StringComparison.OrdinalIgnoreCase));
            }

            return psychologistsList.Any(p => 
                p.CalendarColor.Equals(color, StringComparison.OrdinalIgnoreCase));
        }
    }
}
