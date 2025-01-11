## SimpleTweetApi
SimpleTweetApi is a mock-up of Twitter or X built with ASP.NET Core. It includes features such as tweeting, user management, liking tweets, and reporting tweets.

### Features
-Tweeting: Users can create, update, and delete tweets.
-User Management: Users can register, log in, and manage their profiles.
-Liking Tweets: Users can like and unlike tweets.
-Reporting Tweets: Users can report tweets for various reasons.

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
