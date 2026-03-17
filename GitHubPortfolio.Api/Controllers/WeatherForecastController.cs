using GitHubPortfolio.Services;
using GitHubPortfolio.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace GitHubPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        // הזרקת השירות (שהוא למעשה ה-Decorator העוטף את השירות הבסיסי)
        public PortfolioController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        /// <summary>
        /// מחזיר את נתוני הפורטפוליו האישי (משתמש Caching)
        /// GET: api/Portfolio
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RepositoryData>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPortfolio()
        {
            var data = await _gitHubService.GetPortfolioRepositoriesAsync();
            return Ok(data);
        }

        /// <summary>
        /// מבצע חיפוש רפוזיטורים ציבורי (ללא Caching)
        /// GET: api/Portfolio/Search?repoName=XXX&language=YYY&username=ZZZ
        /// </summary>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(IEnumerable<RepositoryData>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchRepositories(
            [FromQuery] string? repoName,
            [FromQuery] string? language,
            [FromQuery] string? username)
        {
            // שולח את המחרוזות הריקות/Null לפונקציה, והשירות מטפל בהן
            var data = await _gitHubService.SearchRepositoriesAsync(repoName, language, username);
            return Ok(data);
        }
    }
}