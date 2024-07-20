using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using AsperandLabs.Kettlecorn.Integration;
using Microsoft.AspNetCore.Mvc;

namespace AsperandLabs.Kettlecorn.Connections.PythonHttp;

/// <summary>
/// A wrapper for the sas workspace to make pooling easier
/// </summary>
public class PythonHttpConnection : IConnection
{
    private readonly ILogger<PythonHttpConnection> _log;
    private readonly int _port;
    private readonly Process _process;
    private readonly HttpClient _httpClient;
    private readonly HashSet<string> _contentHeaders = ["Content-Type", "Content-Length"];

    public PythonHttpConnection(ILogger<PythonHttpConnection> log, int port, Process process)
    {
        _log = log;
        _port = port;
        _process = process;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri($"http://localhost:{port}");
    }

    public string UniqueIdentifier => $"con:{nameof(PythonHttpConnection)}:{_port}";

    public async Task<IActionResult> Do(PathString path, HttpMethod method, IHeaderDictionary headers, Stream? body = null)
    {
        var request = new HttpRequestMessage(method, path);
        request.Content = new StreamContent(body ?? new MemoryStream());
        foreach (var header in headers)
        {
            if (_contentHeaders.Contains(header.Key))
                request.Content.Headers.Add(header.Key, header.Value.ToArray());
            else
                request.Headers.Add(header.Key, header.Value.ToArray());
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return new OkObjectResult(await response.Content.ReadAsStreamAsync());
        return new BadRequestResult();
    }

    public bool Healthy()
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            listener.Stop();
            return false;
        }
        catch (SocketException)
        {
            return true;
        }
    }

    public void Start()
    {
        if(!_process.Start())
            throw new ApplicationException("PythonHttpConnection failed to start.");
        
        while (!Healthy())
        {
            Thread.Sleep(10);
        }
        _log.LogInformation("Started PythonHttpConnection on port: {Port}", _port);
    }

    public void Reset()
    {
        _process.Kill();
        Start();
    }


    public void Dispose()
    {
        _process.Kill();
        _process.Dispose();
    }
}