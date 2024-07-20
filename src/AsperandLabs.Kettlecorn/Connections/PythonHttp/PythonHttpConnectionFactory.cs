using System.Diagnostics;
using AsperandLabs.Kettlecorn.Config;
using AsperandLabs.Kettlecorn.Integration;

namespace AsperandLabs.Kettlecorn.Connections.PythonHttp;

public class PythonHttpConnectionFactory: IConnectionFactory
{
    private readonly ILogger<PythonHttpConnectionFactory> _log;
    private readonly PythonHttpConnectionConfig _config;
    private readonly IServiceProvider _serviceProvider;
    private int _nextPort = 9000;

    public PythonHttpConnectionFactory(ILogger<PythonHttpConnectionFactory> log, PythonHttpConnectionConfig config, IServiceProvider serviceProvider)
    {
        _log = log;
        _config = config;
        _serviceProvider = serviceProvider;
    }
    public IConnection CreateConnection()
    {
        var port = _nextPort;
        _nextPort++;
        var process = new Process();
        process.StartInfo.WorkingDirectory = _config.WorkingDirectory;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Arguments = $"{_config.EntryPoint} --workers 1 --host 0.0.0.0 --port {port}";
        process.StartInfo.FileName = _config.RelativeStartFile;
        process.StartInfo.EnvironmentVariables["NIXTLA_NUMBA_CACHE"] = "1";
        
        _log.LogInformation($"Starting PythonHttp server on port {port}");
        
        var connection = new PythonHttpConnection(_serviceProvider.GetService<ILogger<PythonHttpConnection>>()!, port, process);
        connection.Start();

        return connection;
    }
}