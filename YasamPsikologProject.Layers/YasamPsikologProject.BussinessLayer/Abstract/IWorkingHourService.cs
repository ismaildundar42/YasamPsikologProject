using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface IWorkingHourService
    {
        Task<WorkingHour?> GetByIdAsync(int id);
        Task<IEnumerable<WorkingHour>> GetByPsychologistAsync(int psychologistId);
        Task<WorkingHour> CreateAsync(WorkingHour workingHour);
        Task<WorkingHour> UpdateAsync(WorkingHour workingHour);
        Task DeleteAsync(int id);
        Task<WorkingHour?> GetByPsychologistAndDayAsync(int psychologistId, WeekDay day);
    }
}
