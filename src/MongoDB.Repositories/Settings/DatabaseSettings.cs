using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories.Settings
{
  public class DatabaseSettings : IDatabaseSettings
  {
    public string DatabaseName { get; set; } = "StorageAPI";
    public string ConnectionString { get; set; } = "mongodb://localhost";
    public string Secret { get; set; } = "$torag3Api$3cret!";
  }
}
