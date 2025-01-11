using Microsoft.EntityFrameworkCore;
using SimpleTweetApi.Database;
using SimpleTweetApi.Enum;
using SimpleTweetApi.Models.App;

namespace SimpleTweetApi.Services;

public class FlagService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FlagService> _logger;

    public FlagService(ApplicationDbContext context, ILogger<FlagService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Flag>> Flags(string? keyword = null, int page = 1, int limit = 20)
    {
        var query = _context.Flag
            .Where(t => t.DeletedAt == null)
            .OrderByDescending(t => t.Code)
            .AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(t => t.Code.Contains(keyword));
        }
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Flag?> Flag(string code)
    {
        var flag = await _context.Flag.FindAsync(code);
        if (flag == null || flag.DeletedAt != null)
        {
            return null;
        }
        return flag;
    }

    public async Task<IEnumerable<Flag>> FlagsByType(FlagType type, string? keyword = null, int page = 1, int limit = 20)
    {
        var query = _context.Flag
            .Where(t => t.DeletedAt == null && t.Code.StartsWith(type.ToString()))
            .OrderByDescending(t => t.Code)
            .AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(t => t.Code.Contains(keyword));
        }
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Flag> Create(FlagType type,string name, string description = "", string? icon = null)
    {
        var flag = new Flag
        {
            Code = $"{type.ToString()}_{name.Replace(' ', '_')}".ToUpper(),
            Name = name,
            Description = description,
            Icon = icon
        };


        if (await _context.Flag.FindAsync(flag.Code) != null)
        {
            throw new InvalidOperationException($"Flag with code '{flag.Code}' already exists.");
        }

        _context.Flag.Add(flag);
        await _context.SaveChangesAsync();
        return flag;
    }

    public async Task<Flag?> Update(string code, Flag flag)
    {
        var existingFlag = await _context.Flag.FindAsync(code);
        if (existingFlag == null || existingFlag.DeletedAt != null)
        {
            return null;
        }
        _context.Entry(existingFlag).CurrentValues.SetValues(flag);
        await _context.SaveChangesAsync();
        return flag;
    }

    public async Task<bool> Delete(string code)
    {
        var flag = await _context.Flag.FindAsync(code);
        if (flag == null || flag.DeletedAt != null)
        {
            return false;
        }
        flag.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Restore(string code)
    {
        var flag = await _context.Flag.FindAsync(code);
        if (flag == null || flag.DeletedAt == null)
        {
            return false;
        }
        flag.DeletedAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> GenerateBasicFlagCore()
    {
        Dictionary<string, string> flagCoreReport = new Dictionary<string, string>
        {
            { "SPAM", "Content considered as spam" },
            { "HATE_SPEECH", "Content considered as hate speech" },
            { "VIOLENCE", "Content considered as violence" },
            { "SEXUAL_CONTENT", "Content considered as sexual content" },
            { "FAKE_NEWS", "Content considered as fake news" },
            { "HARASSMENT", "Content considered as harassment" },
            { "BULLYING", "Content considered as bullying" },
            { "DISINFORMATION", "Content considered as disinformation" },
            { "MISINFORMATION", "Content considered as misinformation" },
            { "SCAM", "Content considered as scam" },
            { "PHISHING", "Content considered as phishing" },
            { "MALWARE", "Content considered as malware" },
            { "SPYWARE", "Content considered as spyware" },
            { "RANSOMWARE", "Content considered as ransomware" },
        };

        await _context.Database.BeginTransactionAsync();

        foreach (var (code, description) in flagCoreReport)
        {
            try
            {
                await Create(FlagType.REPORT, code, description);
                await Create(FlagType.PENDING_REPORT, code, description);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogWarning($"FlagService/GenerateBasicFlagCore: {e}");
                continue;
            }
            catch (Exception e)
            {
                await _context.Database.RollbackTransactionAsync();
                _logger.LogError($"FlagService/GenerateBasicFlagCore: {e}");

                return false;
            }
        }

        await _context.Database.CommitTransactionAsync();

        return true;
    }
}
