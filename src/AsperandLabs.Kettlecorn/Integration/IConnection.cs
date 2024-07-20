using Microsoft.AspNetCore.Mvc;

namespace AsperandLabs.Kettlecorn.Integration;

public interface IConnection: IDisposable
{
    string UniqueIdentifier { get; }
    Task<IActionResult> Do(PathString path, HttpMethod method, IHeaderDictionary headers, Stream? body = null);
    bool Healthy();
    void Start();
}