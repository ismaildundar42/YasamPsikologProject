using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class WorkingHourManager : IWorkingHourService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkingHourManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkingHour>> GetAllAsync()
        {
            return await _unitOfWork.WorkingHourRepository.GetAllAsync();
        }

        public async Task<WorkingHour?> GetByIdAsync(int id)
        {
            return await _unitOfWork.WorkingHourRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WorkingHour>> GetByPsychologistAsync(int psychologistId)
        {
            return await _unitOfWork.WorkingHourRepository.GetByPsychologistAsync(psychologistId);
        }

        public async Task<WorkingHour> CreateAsync(WorkingHour workingHour)
        {
            var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(workingHour.PsychologistId);
            if (psychologist == null)
                throw new Exception("Psikolog bulunamadı.");

            var existing = await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(
                workingHour.PsychologistId, 
                workingHour.DayOfWeek);

            if (existing != null)
                throw new Exception("Bu gün için zaten çalışma saati tanımlanmış.");

            if (workingHour.StartTime >= workingHour.EndTime)
                throw new Exception("Bitiş saati başlangıç saatinden önce olamaz.");

            // Mola saatleri kontrolü - Molalar çalışma saatleri içinde olmalı
            if (workingHour.BreakTimes != null && workingHour.BreakTimes.Any())
            {
                foreach (var breakTime in workingHour.BreakTimes)
                {
                    if (breakTime.StartTime < workingHour.StartTime)
                        throw new Exception($"Mola başlangıç saati ({breakTime.StartTime:hh\\:mm}) çalışma başlangıç saatinden ({workingHour.StartTime:hh\\:mm}) önce olamaz.");
                    
                    if (breakTime.EndTime > workingHour.EndTime)
                        throw new Exception($"Mola bitiş saati ({breakTime.EndTime:hh\\:mm}) çalışma bitiş saatinden ({workingHour.EndTime:hh\\:mm}) sonra olamaz.");
                    
                    if (breakTime.StartTime >= breakTime.EndTime)
                        throw new Exception($"Mola bitiş saati ({breakTime.EndTime:hh\\:mm}) başlangıç saatinden ({breakTime.StartTime:hh\\:mm}) önce veya eşit olamaz.");
                }
            }

            await _unitOfWork.WorkingHourRepository.AddAsync(workingHour);
            await _unitOfWork.SaveChangesAsync();
            return workingHour;
        }

        public async Task<WorkingHour> UpdateAsync(WorkingHour workingHour)
        {
            var existing = await _unitOfWork.WorkingHourRepository.GetByIdAsync(workingHour.Id);
            if (existing == null)
                throw new Exception("Çalışma saati bulunamadı.");

            if (workingHour.StartTime >= workingHour.EndTime)
                throw new Exception("Bitiş saati başlangıç saatinden önce olamaz.");

            // Mola saatleri kontrolü - Molalar çalışma saatleri içinde olmalı
            if (workingHour.BreakTimes != null && workingHour.BreakTimes.Any())
            {
                foreach (var breakTime in workingHour.BreakTimes)
                {
                    if (breakTime.StartTime < workingHour.StartTime)
                        throw new Exception($"Mola başlangıç saati ({breakTime.StartTime:hh\\:mm}) çalışma başlangıç saatinden ({workingHour.StartTime:hh\\:mm}) önce olamaz.");
                    
                    if (breakTime.EndTime > workingHour.EndTime)
                        throw new Exception($"Mola bitiş saati ({breakTime.EndTime:hh\\:mm}) çalışma bitiş saatinden ({workingHour.EndTime:hh\\:mm}) sonra olamaz.");
                    
                    if (breakTime.StartTime >= breakTime.EndTime)
                        throw new Exception($"Mola bitiş saati ({breakTime.EndTime:hh\\:mm}) başlangıç saatinden ({breakTime.StartTime:hh\\:mm}) önce veya eşit olamaz.");
                }
            }

            _unitOfWork.WorkingHourRepository.Update(workingHour);
            await _unitOfWork.SaveChangesAsync();
            return workingHour;
        }

        public async Task DeleteAsync(int id)
        {
            var workingHour = await _unitOfWork.WorkingHourRepository.GetByIdAsync(id);
            if (workingHour == null)
                throw new Exception("Çalışma saati bulunamadı.");

            _unitOfWork.WorkingHourRepository.Delete(workingHour);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<WorkingHour?> GetByPsychologistAndDayAsync(int psychologistId, WeekDay day)
        {
            return await _unitOfWork.WorkingHourRepository.GetByPsychologistAndDayAsync(psychologistId, day);
        }
    }
}
