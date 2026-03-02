using JobScoreServer.Data;
using JobScoreServer.DTOs;
using JobScoreServer.Models;
using JobScoreServer.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JobScoreServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly DBContext _dbcontext;

        public AuthService(IConfiguration config, ITokenService tokenService, DBContext dbcontext)
        {
            _config = config;
            _tokenService = tokenService;
            _dbcontext = dbcontext;
        }

        public async Task<string> Login(string email, string password)
        {
            // checking if user exists
            var user = await _dbcontext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            // password verification
            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return _tokenService.CreateToken(user);
        }

        public async Task<string> Register(RegisterRequest request)
        {
            if (request?.password != request?.passwordConfirm)
            {
                throw new Exception("Passwords do not match");
            }

            // checking if user is already registered
            var existingUser = await _dbcontext.Users
                .FirstOrDefaultAsync(u => u.Email == request.email);

            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            var newUser = new User
            {
                FirstName = request!.firstName,
                LastName = request.lastName,
                Email = request.email,
                PasswordHash = PasswordHasher.HashPassword(request.password),
                IsAdmin = false 
            };

            try
            {
                _dbcontext.Users.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                return _tokenService.CreateToken(newUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register user: {ex.Message}", ex);
            }
        }
    }
}