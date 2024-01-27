using SimpleResult;

namespace LinkTrim.Web.Services;

public interface IStorage
{
    Task<Option<string>> GetUrl(string key);
    Task SetUrl(string key, string value);
    Task<bool> Exists(string key);
    Task Remove(string key);
}
