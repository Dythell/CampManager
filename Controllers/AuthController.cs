using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly ICounselorRepository _counselorRepository;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUserRepository userRepository,
            ICounselorRepository counselorRepository,
            JwtService jwtService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _counselorRepository = counselorRepository;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(request.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                // Блокируем неподтверждённых админов
                if ((user.Role == "Admin" || user.Role == "GAdmin") && !user.IsConfirmed)
                    return Unauthorized(new { message = "Учётная запись администратора не подтверждена" });

                // Если вожатый, подтянем его данные
                CounselorDTO? counselorData = null;
                if (user.Role == "Counselor")
                {
                    var counselor = await _counselorRepository.GetCounselorByUserIdAsync(user.User_Id);
                    if (counselor != null)
                    {
                        counselorData = new CounselorDTO(
                            counselor.Surname,
                            counselor.Name,
                            counselor.Patronymic,
                            counselor.PhoneNumber
                        );
                    }
                }

                var token = _jwtService.GenerateToken(user.User_Id, user.Username, user.Role);
                var userDTO = new UserDTO(user.User_Id, user.Username, user.Role, counselorData);

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
                // Проверка уникальности
                if (await _userRepository.UserExistsAsync(request.Username))
                    return BadRequest(new { message = "Пользователь уже существует" });

                // Для админов флаг подтверждения сбрасывается, для остальных — сразу true
                bool isAdmin = request.Role == "Admin" || request.Role == "GAdmin";
                bool isConfirmed = !isAdmin;

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role ?? "Counselor",
                    IsConfirmed = isConfirmed
                };

                await _userRepository.AddUserAsync(user);
                await _userRepository.SaveChangesAsync();

                // Если регистрируется вожатый — сразу создаём профиль
                if (user.Role == "Counselor")
                {
                    var counselor = new Counselor
                    {
                        User_Id = user.User_Id,
                        Surname = request.Surname!,
                        Name = request.Name!,
                        Patronymic = request.Patronymic!,
                        PhoneNumber = request.PhoneNumber!
                    };
                    await _counselorRepository.AddCounselorAsync(counselor);
                    await _counselorRepository.SaveChangesAsync();
                }

                // (Опционально) уведомляем админов о новой регистрации админа... 

                return Ok(new { message = "Регистрация прошла успешно" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при регистрации", error = ex.Message });
            }
        }
    }
}
