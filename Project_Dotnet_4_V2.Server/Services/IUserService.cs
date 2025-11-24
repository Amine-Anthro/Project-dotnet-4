using Projet_Dotnet_4.Models;

namespace Projet_Dotnet_4.Services
{
    public interface IUserService
    {
        User GetByUsername(string username);
        bool UsernameExists(string username);
        bool EmailExists(string email);
        void Create(User user);
        public User Get(UserLogin userLogin);
    }
}
