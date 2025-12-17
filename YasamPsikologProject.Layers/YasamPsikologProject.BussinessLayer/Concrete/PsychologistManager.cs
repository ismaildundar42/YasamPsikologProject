using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class PsychologistManager : IPsychologistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PsychologistManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Psychologist?> GetByIdAsync(int id)
        {
            return await _unitOfWork.PsychologistRepository.GetByIdAsync(id);
        }

        public async Task<Psychologist?> GetByUserIdAsync(int userId)
        {
            return await _unitOfWork.PsychologistRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Psychologist>> GetAllActiveAsync()
        {
            return await _unitOfWork.PsychologistRepository.GetActiveWithWorkingHoursAsync();
        }

        public async Task<Psychologist> CreateAsync(Psychologist psychologist)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(psychologist.UserId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (user.Role != EntityLayer.Enums.UserRole.Psychologist)
                throw new Exception("Kullanıcı psikolog rolüne sahip değil.");

            var existing = await _unitOfWork.PsychologistRepository.GetByUserIdAsync(psychologist.UserId);
            if (existing != null)
                throw new Exception("Bu kullanıcı için zaten psikolog kaydı bulunmaktadır.");

            await _unitOfWork.PsychologistRepository.AddAsync(psychologist);
            await _unitOfWork.SaveChangesAsync();
            return psychologist;
        }

        public async Task<Psychologist> UpdateAsync(Psychologist psychologist)
        {
            var existing = await _unitOfWork.PsychologistRepository.GetByIdAsync(psychologist.Id);
            if (existing == null)
                throw new Exception("Psikolog bulunamadı.");

            _unitOfWork.PsychologistRepository.Update(psychologist);
            await _unitOfWork.SaveChangesAsync();
            return psychologist;
        }

        public async Task DeleteAsync(int id)
        {
            var psychologist = await _unitOfWork.PsychologistRepository.GetByIdAsync(id);
            if (psychologist == null)
                throw new Exception("Psikolog bulunamadı.");

            _unitOfWork.PsychologistRepository.Delete(psychologist);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
