using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CampManager.Repositories;

namespace CampManager.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(request.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                var token = _jwtService.GenerateToken(user.User_Id, user.Username, user.Role);
                var userDTO = new UserDTO(user.User_Id, user.Username, user.Role);

                return Ok(new { token, user = userDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при входе", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                if (await _userRepository.UserExistsAsync(request.Username))
                    return BadRequest(new { message = "Пользователь уже существует" });

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role ?? "Counselor"
                };

                await _userRepository.AddUserAsync(user);
                await _userRepository.SaveChangesAsync();

                return Ok(new { message = "Регистрация успешна!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при регистрации", error = ex.Message });
            }
        }
    }
}
