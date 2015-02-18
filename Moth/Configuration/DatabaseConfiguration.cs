namespace Moth.Configuration
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
    }
}