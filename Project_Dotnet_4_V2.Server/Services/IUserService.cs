using Project_Dotnet_4_V2.DTO.Auth;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Services
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task<User?> GetByUsername(string username);
        Task<bool> UsernameExists(string username);
        Task<bool> EmailExists(string email);
    }
}
