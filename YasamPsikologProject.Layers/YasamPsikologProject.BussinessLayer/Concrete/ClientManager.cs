using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class ClientManager : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _unitOfWork.ClientRepository.GetAllAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ClientRepository.GetByIdAsync(id);
        }

        public async Task<Client?> GetByUserIdAsync(int userId)
        {
            return await _unitOfWork.ClientRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Client>> GetByPsychologistAsync(int psychologistId)
        {
            return await _unitOfWork.ClientRepository.GetByPsychologistAsync(psychologistId);
        }

        public async Task<Client> CreateAsync(Client client)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(client.UserId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (user.Role != EntityLayer.Enums.UserRole.Client)
                throw new Exception("Kullanıcı danışan rolüne sahip değil.");

            var existing = await _unitOfWork.ClientRepository.GetByUserIdAsync(client.UserId);
            if (existing != null)
                throw new Exception("Bu kullanıcı için zaten danışan kaydı bulunmaktadır.");

            if (client.AssignedPsychologistId.HasValue)
            {
                var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(client.AssignedPsychologistId.Value);
                if (psychologist == null)
                    throw new Exception("Atanan psikolog bulunamadı.");
            }

            await _unitOfWork.ClientRepository.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateAsync(Client client)
        {
            var existing = await _unitOfWork.ClientRepository.GetByIdAsync(client.Id);
            if (existing == null)
                throw new Exception("Danışan bulunamadı.");

            if (client.AssignedPsychologistId.HasValue)
            {
                var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(client.AssignedPsychologistId.Value);
                if (psychologist == null)
                    throw new Exception("Atanan psikolog bulunamadı.");
            }

            _unitOfWork.ClientRepository.Update(client);
            await _unitOfWork.SaveChangesAsync();
            return client;
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _unitOfWork.ClientRepository.GetByIdAsync(id);
            if (client == null)
                throw new Exception("Danışan bulunamadı.");

            _unitOfWork.ClientRepository.Delete(client);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
