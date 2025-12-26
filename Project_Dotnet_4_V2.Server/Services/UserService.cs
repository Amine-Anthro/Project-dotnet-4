
using Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Create(User user)
        {
            return await _userRepository.CreateAsync(user);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<bool> UsernameExists(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null;
        }

        public async Task<bool> EmailExists(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }
    }
}
