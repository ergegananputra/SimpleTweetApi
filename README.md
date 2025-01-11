## SimpleTweetApi
SimpleTweetApi is a mock-up of Twitter or X built with ASP.NET Core. It includes features such as tweeting, user management, liking tweets, and reporting tweets.

### Features
-Tweeting: Users can create, update, and delete tweets.
-User Management: Users can register, log in, and manage their profiles.
-Liking Tweets: Users can like and unlike tweets.
-Reporting Tweets: Users can report tweets for various reasons.

### Project Structure
```bash
.
├── Controllers/
│   ├── MasterFlagController.cs
│   ├── TweetController.cs
│   └── WeatherForecastController.cs
├── Database/
│   └── ApplicationDbContext.cs
├── Enum/
│   └── FlagType.cs
├── Extensions/
│   └── MigrationExtensions.cs
├── Middlewares/
│   └── AdminMiddleware.cs
├── Migrations/
│   ├── 20250102081239_InitialCreate.cs
│   ├── 20250102081239_InitialCreate.Designer.cs
│   ├── 20250102090528_TweetTable.cs
│   ├── 20250102090528_TweetTable.Designer.cs
│   ├── 20250110084237_TweetLikesTable.cs
│   ├── 20250110084237_TweetLikesTable.Designer.cs
│   ├── 20250110091156_FlagTableAndTweetFlagsTableRelation.cs
│   ├── 20250110091156_FlagTableAndTweetFlagsTableRelation.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── App/
│   │   ├── Flag.cs
│   │   ├── Tweet.cs
│   │   ├── TweetFlags.cs
│   │   └── TweetLikes.cs
│   ├── Auth/
│   │   └── User.cs
│   └── BaseModel.cs
├── Resources/
│   ├── Requests/
│   │   ├── FlagPostForm.cs
│   │   └── TweetPostForm.cs
│   └── Responses/
│       ├── BasePagination.cs
│       └── BaseResponse.cs
├── Services/
│   ├── FlagService.cs
│   └── TweetCoreService.cs
├── .gitattributes
├── .gitignore
├── appsettings.Development.json
├── appsettings.json
├── Program.cs
├── SimpleTweetApi.csproj
├── SimpleTweetApi.sln
└── WeatherForecast.cs
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL

### Setup
1. Clone the repository:
```bash
git clone https://github.com/ergegananputra/SimpleTweetApi.git
cd SimpleTweetApi
```
2. Update the connection string in appsettings.Development.json:
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Username=postgres;Database=net8_tweet"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
3. Apply migrations
```bash
dotnet ef database update
```
4. Run the application
```bash
dotnet run
```
