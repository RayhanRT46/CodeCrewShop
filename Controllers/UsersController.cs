using CodeCrewShop.Models.User;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCrewShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;

        public UserController(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _tokenService = tokenService;
        }

        // 🔹 Admin-only to see all users
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _uow.Users.GetAllAsync();
            var result = users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Phone,
                u.Role,
                u.IsActive,
                u.CreatedAt
            });
            return Ok(result);
        }

        // 🔹 Customer registration
        [HttpPost("register-customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterRequest req)
        {
            return await RegisterUser(req, UserRoles.Customer);
        }

        // 🔹 Seller registration
        [HttpPost("register-seller")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterRequest req)
        {
            return await RegisterUser(req, UserRoles.Seller);
        }

        // 🔹 Admin registration (⚠️ should be limited)
        [HttpPost("register-admin")]
        [Authorize(Roles = UserRoles.Admin)] // only admins can create admins
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest req)
        {
            return await RegisterUser(req, UserRoles.Admin);
        }

        // 🔹 Shared logic for registration
        private async Task<IActionResult> RegisterUser(RegisterRequest req, string role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _uow.Users.GetByEmailAsync(req.Email);
            if (existing != null) return BadRequest("Email already exists");

            var user = new Users
            {
                FullName = req.FullName,
                Email = req.Email,
                Phone = req.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Users.AddAsync(user);
            await _uow.CompleteAsync();

            var token = _tokenService.CreateToken(user);
            return Ok(new
            {
                message = $"{role} registered successfully",
                token,
                expiresAt = _tokenService.GetExpiry()
            });
        }

        // 🔹 Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _uow.Users.GetByEmailAsync(model.Email);
            if (user == null || !user.IsActive) return Unauthorized("Invalid credentials");

            var isValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isValid) return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                token,
                expiresAt = _tokenService.GetExpiry(),
                user = new { user.Id, user.FullName, user.Email, user.Role }
            });
        }
    }
}
