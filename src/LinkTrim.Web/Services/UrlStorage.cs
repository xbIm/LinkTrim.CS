using SimpleResult;
using StackExchange.Redis;

namespace LinkTrim.Web.Services;

//todo: Add logging
public class UrlStorage(IConnectionMultiplexer connection) : IStorage
{
    private const string UrlField = "longUrl";

    public async Task<Option<string>> GetUrl(string key)
    {
        var redisDb = connection.GetDatabase();

        var value = await redisDb.HashGetAsync(key, UrlField);

        return value.HasValue ?
            value.ToString().ToOption() :
            Option<string>.None;
    }

    // todo: add expire
    public async Task SetUrl(string key, string value)
    {
        var redisDb = connection.GetDatabase();
        var tran = redisDb.CreateTransaction();
        await redisDb.HashSetAsync(key, UrlField, value);

        redisDb.SetAdd("valueIndex:" + value, key);

        await tran.ExecuteAsync();
    }

    public async Task<bool> Exists(string value)
    {
        var redisDb = connection.GetDatabase();

        var values = await redisDb.SetMembersAsync("valueIndex:" + value);
        return values.Any();
    }

    public async Task Remove(string key)
    {
        var redisDb = connection.GetDatabase();
        var value = await redisDb.HashGetAsync(key, UrlField);
        if (!value.IsNull)
        {
            // If the key exists, remove it and the associated index
            var tran = redisDb.CreateTransaction();
            await redisDb.KeyDeleteAsync(key);
            await redisDb.SetRemoveAsync("valueIndex:" + value, key);
            await tran.ExecuteAsync();
        }
    }
}
