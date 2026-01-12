using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IBreakTimeRepository : IRepository<BreakTime>
    {
        Task<IEnumerable<BreakTime>> GetByPsychologistAsync(int psychologistId);
    }
}
