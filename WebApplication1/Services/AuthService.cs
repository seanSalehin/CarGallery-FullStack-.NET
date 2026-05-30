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
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {


        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthService(ApplicationDbContext db, IMapper mapper, IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<bool> IsEmailExistAsync(string email)
        {
            var normalized = email.Trim().ToUpper();
            return await _db.Users.AnyAsync(u => u.NormalizedEmail == normalized);
        }


        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null ||
              string.IsNullOrWhiteSpace(loginRequestDTO.Email) ||
              string.IsNullOrWhiteSpace(loginRequestDTO.Password))
                    return null;

                var email = loginRequestDTO.Email.Trim().ToLower();

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
                if (user == null)   
                    return null;

                var ok = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if (!ok) 
                    return null;

                var roles = await _userManager.GetRolesAsync(user);

                var token = GenerateJwtToken(user, roles);

                var dto = _mapper.Map<UserDTO>(user);
                dto.Role = roles.FirstOrDefault() ?? "Customer";

                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user),
                    Token = token
                };
            }
            catch(Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException("Registration failed: " + msg, ex);
            }
          
        }


        public async Task<UserDTO?> RegisterAsync(RegisterationRequestDTO dto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = dto.Email,
                    UserName = dto.Email,
                    Name = dto.Name,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );

                var role = string.IsNullOrWhiteSpace(dto.Role) ? "Customer" : dto.Role.Trim();

                if (role != "Customer" && role != "Admin")
                    role = "Customer";

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                        throw new InvalidOperationException(
                            string.Join(", ", roleResult.Errors.Select(e => e.Description))
                        );
                }

                var addRole = await _userManager.AddToRoleAsync(user, role);
                if (!addRole.Succeeded)
                    throw new InvalidOperationException(
                        string.Join(", ", addRole.Errors.Select(e => e.Description))
                    );

                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = role;
                return userDTO;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException("Registration failed: " + msg, ex);
            }
        }



        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var secret = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JwtSettings:Secret is missing in appsettings.json");

            var key = Encoding.UTF8.GetBytes(secret);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Name, user.UserName ?? "")
                };

            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
