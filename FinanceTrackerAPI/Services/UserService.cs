using FinanceTrackerAPI.Models;
using FinanceTrackerAPI.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace FinanceTrackerAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var hash = ComputeHash(password);
            return user.PasswordHash == hash ? user : null;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null) return false;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = ComputeHash(password)
            };
            await _userRepository.CreateAsync(user);
            return true;
        }

        private string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
