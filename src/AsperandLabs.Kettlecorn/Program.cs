using AsperandLabs.Kettlecorn.Config;
using AsperandLabs.Kettlecorn.Connections.PythonHttp;
using AsperandLabs.Kettlecorn.Integration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionFactory, PythonHttpConnectionFactory>();
builder.Services.AddSingleton<ConnectionPool>();

var connectionConfig = new PythonHttpConnectionConfig();
builder.Configuration.Bind("PythonHttpConnectionConfig", connectionConfig);
builder.Services.AddSingleton(connectionConfig);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProxyController}/{action=Index}");

app.Run();