using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _unitOfWork.UserRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _unitOfWork.UserRepository.GetByPhoneAsync(phone);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _unitOfWork.UserRepository.GetAllAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            if (await _unitOfWork.UserRepository.EmailExistsAsync(user.Email))
                throw new Exception("Bu email adresi zaten kullanılıyor.");

            if (await _unitOfWork.UserRepository.PhoneExistsAsync(user.PhoneNumber))
                throw new Exception("Bu telefon numarası zaten kullanılıyor.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new Exception("Şifre boş olamaz.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (await _unitOfWork.UserRepository.EmailExistsAsync(user.Email, user.Id))
                throw new Exception("Bu email adresi başka bir kullanıcı tarafından kullanılıyor.");

            if (await _unitOfWork.UserRepository.PhoneExistsAsync(user.PhoneNumber, user.Id))
                throw new Exception("Bu telefon numarası başka bir kullanıcı tarafından kullanılıyor.");

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await _unitOfWork.UserRepository.EmailExistsAsync(email, excludeUserId);
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeUserId = null)
        {
            return await _unitOfWork.UserRepository.PhoneExistsAsync(phone, excludeUserId);
        }
    }
}
