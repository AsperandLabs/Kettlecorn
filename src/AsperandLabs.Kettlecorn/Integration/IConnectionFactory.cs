namespace AsperandLabs.Kettlecorn.Integration;

public interface IConnectionFactory
{
    IConnection CreateConnection();
}