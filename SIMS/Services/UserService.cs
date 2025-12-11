using SIMS.Interfaces;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public async Task<Users?> LoginUser(string username, string password)
        {
            // phuong thuc kiem tra viec dang nhap he thong
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null) return null;

            // mk sau nay ma hoa - giai mai de kiem tra mk
            return user.Password.Equals(password) ? user : null;
        }
    }
}
