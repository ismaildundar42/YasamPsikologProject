using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Client>> GetByPsychologistAsync(int psychologistId);
    }
}
