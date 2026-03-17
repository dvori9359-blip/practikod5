using GitHubPortfolio.Services;
using GitHubPortfolio.Services.Configuration;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// הוספת שירותי Controller
builder.Services.AddControllers();

// הוספת תיעוד API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 1. הזרקת Options ---
// קורא את מקטע GitHub מתוך secrets.json וממפה אותו ל-GitHubOptions
builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection(GitHubOptions.GitHub));

// --- 2. הזרקת IMemoryCache ---
// נדרש עבור Decorator ה-Caching
builder.Services.AddMemoryCache();

// ... (קוד קודם ב-Program.cs)

// --- 3. הזרקת IGitHubService עם Decorator ---

// 1. רישום המימוש הבסיסי (השירות האמיתי) כמימוש של הממשק IGitHubService.
// אנו משתמשים ב-AddScoped, שיטת הרישום הנפוצה ביותר לשירותי בקשה (Per Request).
builder.Services.AddScoped<IGitHubService, GitHubService>();

// 2. רישום ה-Decorator:
// עכשיו, Scrutor מוצא את הרישום של IGitHubService, ומחליף אותו
// באובייקט GitHubCachingDecorator, אשר עוטף את המימוש המקורי.
builder.Services.Decorate<IGitHubService, GitHubCachingDecorator>();


var app = builder.Build();



// הגדרת Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();