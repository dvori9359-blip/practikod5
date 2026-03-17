using GitHubPortfolio.Services.Configuration;
using GitHubPortfolio.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubPortfolio.Services
{
    public class GitHubCachingDecorator : IGitHubService
    {
        private readonly IGitHubService _decoratedService;
        private readonly IMemoryCache _cache;
        private readonly GitHubOptions _options;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        // מקבל באמצעות הזרקת תלויות את השירות שצריך לעטוף ואת ה-Cache
        public GitHubCachingDecorator(
            IGitHubService decoratedService,
            IMemoryCache cache,
            IOptions<GitHubOptions> options)
        {
            _decoratedService = decoratedService;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<IEnumerable<RepositoryData>> GetPortfolioRepositoriesAsync()
        {
            // מזהה ייחודי ל-Cache של הפורטפוליו האישי
            string cacheKey = $"PortfolioData_{_options.Username}";

            // נסה לשלוף מה-Cache
            if (_cache.TryGetValue(cacheKey, out IEnumerable<RepositoryData>? cachedData) && cachedData != null)
            {
                // נמצא ב-Cache, החזר מיד
                return cachedData;
            }

            // לא נמצא, קרא לשירות האמיתי
            var data = await _decoratedService.GetPortfolioRepositoriesAsync();

            // שמור ב-Cache ל-30 דקות
            _cache.Set(cacheKey, data, _cacheDuration);

            return data;
        }

        public Task<IEnumerable<RepositoryData>> SearchRepositoriesAsync(string repositoryName, string language, string username)
        {
            // אין צורך ב-Caching עבור פונקציית החיפוש הכללית
            // מפני שקריטריוני החיפוש אינסופיים (למעט לוגיקת Throttling, שאינה נדרשת כאן).
            return _decoratedService.SearchRepositoriesAsync(repositoryName, language, username);
        }
    }
}