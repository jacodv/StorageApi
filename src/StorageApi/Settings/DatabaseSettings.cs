using StorageApi.Interfaces;

namespace StorageApi.Settings
{
  public class DatabaseSettings : IDatabaseSettings
  {
    public string DatabaseName { get; set; } = "StorageAPI";
    public string ConnectionString { get; set; } = "mongodb://localhost";
    public string Secret { get; set; } = "$torag3Api$3cret!";
  }
}
