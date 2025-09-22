using CodeCrewShop.Data;
using CodeCrewShop.Models.User;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeCrewShop.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        //DB Context
        private readonly CodeCrewShopContext _context;
        public UserRepository(CodeCrewShopContext context) => _context = context;

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            //Fast Action for Database create.
            await  _context.Database.EnsureCreatedAsync();
           return await _context.User.ToListAsync();
        }
        public async Task<Users?> GetByIdAsync(int id) =>
            await _context.User.FindAsync(id);

        public async Task<Users?> GetByEmailAsync(string email) =>
            await _context.User.FirstOrDefaultAsync(u => u.Email == email);

        public async Task AddAsync(Users user) =>
            await _context.User.AddAsync(user);

        public void Update(Users user) => _context.User.Update(user);

        public void Delete(Users user) => _context.User.Remove(user);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) => _config = config;

        public string CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: GetExpiry(),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpiry()
        {
            var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;
            return DateTime.UtcNow.AddMinutes(minutes);
        }
    }
}
