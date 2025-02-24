using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CampManager.Repositories;
using System.Security.Claims;

namespace CampManager.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICounselorRepository _counselorRepository;

        public ProfileController(IUserRepository userRepository, ICounselorRepository counselorRepository)
        {
            _userRepository = userRepository;
            _counselorRepository = counselorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub")
                                  ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                    return Unauthorized(new { message = "Не удалось определить идентификатор пользователя." });

                var userId = int.Parse(userIdClaim.Value);

                var user = await _userRepository.GetUserWithCounselorAsync(userId);
                if (user == null)
                    return NotFound(new { message = "Пользователь не найден" });

                if (user.Role == "Counselor")
                {
                    if (user.Counselor == null)
                        return NotFound(new { message = "Данные вожатого не найдены" });

                    return Ok(new
                    {
                        name = user.Counselor.Name,
                        surname = user.Counselor.Surname,
                        patronymic = user.Counselor.Patronymic,
                        phoneNumber = user.Counselor.PhoneNumber,
                        role = user.Role
                    });
                }
                else
                {
                    return Ok(new
                    {
                        name = user.Username,
                        surname = "-",
                        patronymic = "-",
                        phoneNumber = "-",
                        role = user.Role
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка сервера при получении профиля", error = ex.Message });
            }
        }
    }
}
