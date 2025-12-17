using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IUnavailableTimeRepository : IRepository<UnavailableTime>
    {
        Task<IEnumerable<UnavailableTime>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> HasUnavailableTimeAsync(int psychologistId, DateTime startDate, DateTime endDate);
    }
}
