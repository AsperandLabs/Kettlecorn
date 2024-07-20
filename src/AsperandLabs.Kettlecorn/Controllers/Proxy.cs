using AsperandLabs.Kettlecorn.Integration;
using Microsoft.AspNetCore.Mvc;

namespace AsperandLabs.Kettlecorn.Controllers;

[ApiController]
[Route("{*url}")]  
public class ProxyController : Controller
{
    private readonly ConnectionPool _pool;

    public ProxyController(ConnectionPool pool)
    {
        _pool = pool;
    }
    
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    public Task<IActionResult> Index()
    {
        var method = new HttpMethod(HttpContext.Request.Method);
        return _pool.Do(HttpContext.Request.Path, method, HttpContext.Request.Headers, HttpContext.Request.Body);
    }
}