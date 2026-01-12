using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IPsychologistRepository : IRepository<Psychologist>
    {
        Task<Psychologist?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Psychologist>> GetActiveWithWorkingHoursAsync();
    }
}
