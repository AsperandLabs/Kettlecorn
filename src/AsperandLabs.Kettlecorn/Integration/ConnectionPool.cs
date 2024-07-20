using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace AsperandLabs.Kettlecorn.Integration;

public class ConnectionPool : IDisposable
{
    private readonly ILogger<ConnectionPool> _log;
    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentQueue<IConnection> _pool;

    public ConnectionPool(ILogger<ConnectionPool> log, IConnectionFactory connectionFactory)
    {
        _log = log;
        _connectionFactory = connectionFactory;
        _pool = new ConcurrentQueue<IConnection>();
        ExpandPool(4);
    }

    public Task<IActionResult> Do(PathString path, HttpMethod method, IHeaderDictionary headers, Stream? body = null)
    {
        if (!_pool.TryDequeue(out var connection))
        {
            _log.LogInformation("No available connection, expanding pool");
            var newConnection = _connectionFactory.CreateConnection();
            connection = newConnection;
        }

        var result = connection.Do(path, method, headers, body);
        _pool.Enqueue(connection);
        return result;
    }

    /// <summary>
    /// Adds a specified number of connections to the pool
    /// </summary>
    /// <param name="numberOfConnections"></param>
    private void ExpandPool(int numberOfConnections)
    {
        for (var i = 0; i < numberOfConnections; i++)
        {
            _pool.Enqueue(_connectionFactory.CreateConnection());
        }
    }

    public void Dispose()
    {
        while (_pool.TryDequeue(out var connection))
        {
            connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}