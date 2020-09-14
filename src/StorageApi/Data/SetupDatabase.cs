using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using StorageApi.Interfaces;
using StorageApi.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Operations;


namespace StorageApi.Data
{
  public static class SetupDatabase
  {
    public static async Task Init(IServiceProvider serviceProvider)
    {
      await _createLocationIndexes(((MongoRepository<Location>)serviceProvider.GetService<IRepository<Location>>()).Collection);
      await _createStorageUnitIndexes(((MongoRepository<StorageUnit>)serviceProvider.GetService<IRepository<StorageUnit>>()).Collection);
      await _createBinIndexes(((MongoRepository<StorageBin>)serviceProvider.GetService<IRepository<StorageBin>>()).Collection);
    }

    private static async Task _createLocationIndexes(IMongoCollection<Location> collection)
    {
      var indexKeys = Builders<Location>.IndexKeys.Ascending(x => x.Name);
      await _createIndex(collection, indexKeys, "location_name_1");
    }

    private static async Task _createStorageUnitIndexes(IMongoCollection<StorageUnit> collection)
    {
      var indexKeys = Builders<StorageUnit>.IndexKeys.Ascending(x => x.Name);
      await _createIndex(collection, indexKeys, "bin_name_1", false);
      //TODO: Ensure name is unique per location.
    }

    private static async Task _createBinIndexes(IMongoCollection<StorageBin> userCollection)
    {
      var indexKeys = Builders<StorageBin>.IndexKeys.Ascending(x => x.Name);
      await _createIndex(userCollection, indexKeys, "storageUnit_name_1");
    }

    private static async Task _createIndex<T>(IMongoCollection<T> userCollection, IndexKeysDefinition<T> indexKeys,
      string indexName, bool isUnique = true)
    {
      var createIndexOptions = new CreateIndexOptions()
      {
        Name = indexName,
        Unique = isUnique
      };
      var createIndexModel = new CreateIndexModel<T>(indexKeys, createIndexOptions);
      await userCollection.Indexes.CreateOneAsync(createIndexModel);
    }
  }
}