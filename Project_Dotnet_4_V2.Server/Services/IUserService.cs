using Projet_Dotnet_4.DTO.Auth;
using Projet_Dotnet_4.Models;

namespace Projet_Dotnet_4.Services
{
    public interface IUserService
    {
        Task<User> Create(User user);
        Task<User?> GetByUsername(string username);
        Task<bool> UsernameExists(string username);
        Task<bool> EmailExists(string email);
    }
}
