using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace CampManager.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var token = _jwtService.GenerateToken(user.User_Id, user.Username, user.Role);
            var userDTO = new UserDTO(user.User_Id, user.Username, user.Role);

            return Ok(new { token, user = userDTO });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new { message = "Пользователь уже существует" });

            string fixedUsername = Encoding.UTF8.GetString(Encoding.Default.GetBytes(request.Username));

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role ?? "Counselor"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Регистрация успешна!" });
        }
    }
}
