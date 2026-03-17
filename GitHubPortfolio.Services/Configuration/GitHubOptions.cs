namespace GitHubPortfolio.Services.Configuration
{
    public class GitHubOptions
    {
        // קבוע זה מייצג את שם המקטע (Section) בקובץ secrets.json
        public const string GitHub = "GitHub";

        public string Username { get; set; } = string.Empty;
        public string PersonalAccessToken { get; set; } = string.Empty;
    }
}