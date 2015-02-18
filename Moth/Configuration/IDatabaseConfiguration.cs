namespace Moth.Configuration
{
    public interface IDatabaseConfiguration
    {
        string Name { get; }

        string Provider { get; }

        string ConnectionString { get; }
    }
}