using System.Data;

using Microsoft.Extensions.Options;

using SimpleResult;

using StackExchange.Redis;

namespace LinkTrim.Web.Services;

public class UrlStorage(
    IConnectionMultiplexer connection,
    IOptions<LinkTrimOptions> linkTrimOptions)
    : IStorage
{
    private readonly LinkTrimOptions _linkTrimOptions = linkTrimOptions.Value;

    private const string UrlField = "longUrl";

    public async Task<Option<string>> GetUrl(string key)
    {
        var redisDb = GetDatabase();
        var value = await redisDb.HashGetAsync(key, UrlField);

        return value.HasValue ?
            value.ToString().ToOption() :
            Option<string>.None;
    }

    public async Task SetUrl(string key, string value)
    {
        var redisDb = GetDatabase();
        var tran = redisDb.CreateTransaction();

        var _ = tran.HashSetAsync(key, UrlField, value);
        _ = tran.SetAddAsync(ValueKey(value), key);

        _ = tran.KeyExpireAsync(key, _linkTrimOptions.Expired);
        _ = tran.KeyExpireAsync(ValueKey(value), _linkTrimOptions.Expired);

        bool committed = await tran.ExecuteAsync();
        if (!committed)
        {
            throw new RedisException("Transaction failed to commit");
        }
    }

    public async Task<bool> Exists(string key)
    {
        var redisDb = GetDatabase();

        return await redisDb.KeyExistsAsync(key);
    }

    public async Task<Option<string>> ExistsValue(string value)
    {
        var redisDb = GetDatabase();
        var setKey = ValueKey(value);  // Generate the Redis key for the set using the value

        // Retrieve the first key associated with the value from the set
        var keys = await redisDb.SetMembersAsync(setKey);

        if (keys.Length > 0)
        {
            return Option<string>.Some(keys[0]!);  // Return the first key wrapped in a Some
        }

        return Option<string>.None;
    }

    public async Task Remove(string key)
    {
        var redisDb = GetDatabase();
        var value = await redisDb.HashGetAsync(key, UrlField);
        if (!value.IsNull)
        {
            // If the key exists, remove it and the associated index
            var tran = redisDb.CreateTransaction();
            await redisDb.KeyDeleteAsync(key);
            await redisDb.SetRemoveAsync(new RedisKey(ValueKey(value.ToString())), key);
            await tran.ExecuteAsync();
        }
    }

    private IDatabase GetDatabase() => connection.GetDatabase();

    private static string ValueKey(string key) => "value:" + key;
}