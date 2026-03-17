using System;
using System.Collections.Generic;

namespace GitHubPortfolio.Services.Models
{
    public class RepositoryData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlUrl { get; set; }
        // קישור לאתר ה-repo אם הוגדר
        public string? Homepage { get; set; }
        // תאריך הקומיט האחרון
        public DateTimeOffset? LastCommitDate { get; set; }
        // מספר כוכבים
        public int StargazersCount { get; set; }
        // מספר Pull Requests פתוחים
        public int OpenPullRequestsCount { get; set; }
        // רשימת שפות קוד
        public IReadOnlyList<string>? Languages { get; set; }
    }
}