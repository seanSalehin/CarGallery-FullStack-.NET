using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models;
using Gateway_API_Client;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {


        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthService(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }


        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }


        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            if (loginRequestDTO == null ||
                string.IsNullOrWhiteSpace(loginRequestDTO.Email) ||
                string.IsNullOrWhiteSpace(loginRequestDTO.Password))
                return null;

            // normalize
            var email = loginRequestDTO.Email.Trim().ToLower();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (user == null)
                return null;

            if (user.Password != loginRequestDTO.Password)
                return null;

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                UserDTO = _mapper.Map<UserDTO>(user),
                Token = token
            };
        }


        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO registararionRequestDTO)
        {
            try
            {
                if (await IsEmailExistAsync(registararionRequestDTO.Email))
                {
                    //email already exist 
                    throw new InvalidOperationException($"User with email '{registararionRequestDTO.Email}' already exist");
                }
                User user = new()
                {
                    Email = registararionRequestDTO.Email,
                    Name = registararionRequestDTO.Name,
                    Password = registararionRequestDTO.Password,
                    Role = string.IsNullOrEmpty(registararionRequestDTO.Role) ? "Customer" : registararionRequestDTO.Role,
                    CreatedDate = DateTime.Now,
                };
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                //handle any other unexpected errors
                throw new InvalidOperationException("An unexpected error ouccured during user registration", ex);
            }
        }


        private string GenerateJwtToken(User user)
        {
            var secret = _configuration["JwtSettings:Secret"];

            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JwtSettings:Secret is missing in appsettings.json");

            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Name, user.Name ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                 }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
