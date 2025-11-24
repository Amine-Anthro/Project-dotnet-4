using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Projet_Dotnet_4.Models;
using Projet_Dotnet_4.Repositories;
using Projet_Dotnet_4.Data;
using Projet_Dotnet_4.DTO.Auth;

namespace Projet_Dotnet_4.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? Get(UserDTO userLogin)
        {
            return _context.Users.FirstOrDefault(
                u => u.Username.Equals(userLogin.Username, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
