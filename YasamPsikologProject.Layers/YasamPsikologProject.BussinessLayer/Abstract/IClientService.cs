using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Client>> GetByPsychologistAsync(int psychologistId);
        Task<Client> CreateAsync(Client client);
        Task<Client> UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
