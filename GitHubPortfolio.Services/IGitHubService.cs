using GitHubPortfolio.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubPortfolio.Services
{
    public interface IGitHubService
    {
        // מחזירה את רשימת ה-Repositories של הפורטפוליו האישי
        Task<IEnumerable<RepositoryData>> GetPortfolioRepositoriesAsync();

        // מבצעת חיפוש ב-Public Repositories לפי פרמטרים
        Task<IEnumerable<RepositoryData>> SearchRepositoriesAsync(string repositoryName, string language, string username);
    }
}