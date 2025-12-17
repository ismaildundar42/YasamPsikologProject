using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IPsychologistRepository : IRepository<Psychologist>
    {
        Task<Psychologist?> GetByUserIdAsync(int userId);
        Task<Psychologist?> GetByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<Psychologist>> GetActiveWithWorkingHoursAsync();
    }
}
