using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CampManager.Repositories;

namespace CampManager.Controllers
{
    [Route("api/counselors")]
    [ApiController]
    public class CounselorsController : ControllerBase
    {
        private readonly ICounselorRepository _counselorRepository;

        public CounselorsController(ICounselorRepository counselorRepository)
        {
            _counselorRepository = counselorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCounselors()
        {
            var counselors = await _counselorRepository.GetAllCounselorsAsync();
            return Ok(counselors);
        }
    }
}
