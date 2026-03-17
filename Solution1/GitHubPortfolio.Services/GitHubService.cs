using GitHubPortfolio.Services.Configuration;
using GitHubPortfolio.Services.Models;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubPortfolio.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _gitHubClient;
        private readonly string _username;

        public GitHubService(IOptions<GitHubOptions> options)
        {
            var githubOptions = options.Value;
            _username = githubOptions.Username;

            // יצירת ה-Client של Octokit עם שם המוצר וטוקן הזדהות
            _gitHubClient = new GitHubClient(new ProductHeaderValue("PortfolioApp-by-Client"))
            {
                Credentials = new Credentials(githubOptions.PersonalAccessToken)
            };
        }

        public async Task<IEnumerable<RepositoryData>> GetPortfolioRepositoriesAsync()
        {
            // 1. שליפת רשימת ה-Repositories של המשתמש
            var repositories = await _gitHubClient.Repository.GetAllForUser(_username);

            var portfolioDataTasks = repositories.Select(async repo =>
            {
                // 2. שליפת שפות קוד (Languages)
                var languages = await _gitHubClient.Repository.GetAllLanguages(_username, repo.Name);
                var languageNames = languages.Select(l => l.Name).ToList();

                // 3. שליפת הקומיט האחרון לקבלת תאריך עדכון
                var latestCommit = (await _gitHubClient.Repository.Commit.GetAll(_username, repo.Name, new ApiOptions { PageSize = 1, PageCount = 1 })).FirstOrDefault();

                // 4. שליפת Pull Requests פתוחים
                var openPullRequests = await _gitHubClient.PullRequest.GetAllForRepository(_username, repo.Name, new PullRequestRequest { State = ItemStateFilter.Open });

                return new RepositoryData
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Description = repo.Description,
                    HtmlUrl = repo.HtmlUrl,
                    Homepage = repo.Homepage,
                    LastCommitDate = latestCommit?.Commit.Author.Date,
                    StargazersCount = repo.StargazersCount,
                    OpenPullRequestsCount = openPullRequests.Count,
                    Languages = languageNames
                };
            });

            // ממתינים לסיום כל משימות השליפה הנוספות
            return await Task.WhenAll(portfolioDataTasks);
        }

        public async Task<IEnumerable<RepositoryData>> SearchRepositoriesAsync(string repositoryName, string language, string username)
        {
            // 1. בניית מחרוזת החיפוש הראשית (Query)
            string fullQuery = repositoryName ?? string.Empty;

            if (!string.IsNullOrEmpty(language))
            {
                fullQuery += $" language:{language}";
            }

            if (!string.IsNullOrEmpty(username))
            {
                fullQuery += $" user:{username}";
            }

            // 2. יצירת האובייקט SearchRepositoriesRequest עם ה-Query המורכב בלבד.
            // הקצאת המיון הוסרה כדי להימנע משגיאת Read-Only.
            var request = new SearchRepositoriesRequest(fullQuery);

            // 3. ביצוע החיפוש הכללי
            var result = await _gitHubClient.Search.SearchRepo(request);

            return result.Items.Select(repo => new RepositoryData
            {
                Id = repo.Id,
                Name = repo.Name,
                Description = repo.Description,
                HtmlUrl = repo.HtmlUrl,
                Homepage = repo.Homepage,
                StargazersCount = repo.StargazersCount,
            });
        }
    }
}