using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IWorkingHourRepository : IRepository<WorkingHour>
    {
        Task<IEnumerable<WorkingHour>> GetByPsychologistAsync(int psychologistId);
        Task<WorkingHour?> GetByPsychologistAndDayAsync(int psychologistId, WeekDay day);
    }
}
