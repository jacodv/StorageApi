using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Repositories.Interfaces;

namespace MongoDB.Repositories.Services
{
  public class AuditHostedService: IHostedService
  {
    private const string AUDIT_COLLECTION_NAME = "Audit";

    private readonly IDatabaseSettings _settings;
    private readonly ILogger<AuditHostedService> _logger;
    private readonly IMongoCollection<CollectionAuditDocument> _auditCollection;
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly Dictionary<string, Task> _auditTasks;
    private CancellationToken _cancellationToken;

    public AuditHostedService(IDatabaseSettings settings, ILogger<AuditHostedService> logger)
    {
      _settings = settings;
      _logger = logger;
      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      _auditCollection = database.GetCollection<CollectionAuditDocument>(AUDIT_COLLECTION_NAME);
      _auditTasks = new Dictionary<string, Task>();
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
      try
      {
        _logger.LogInformation($"AuditHostedService is starting");

        _cancellationToken = cancellationToken;
        var indexKeys = Builders<CollectionAuditDocument>
          .IndexKeys
          .Ascending(x => x.CreatedAt);
        var indexOptions = new CreateIndexOptions()
        {
          Name = "CreatedAt-TLL-30d",
          Unique = false,
          ExpireAfter = TimeSpan.FromDays(30)
        };
        var createIndexModel = new CreateIndexModel<CollectionAuditDocument>(indexKeys, indexOptions);
        _auditCollection.Indexes.CreateOne(createIndexModel);

        var docCount = _auditCollection.CountDocuments(new BsonDocument());
        if (docCount == 0)
          _auditCollection.InsertOne(new CollectionAuditDocument());
        return Task.CompletedTask;
      }
      catch (Exception e)
      {
        _logger.LogError(e,e.Message);
        throw;
      }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("AuditHostedService is stopping");
      _auditTasks.Clear();
      return Task.CompletedTask;
    }
    public void AddCollectionToAudit<T>(IMongoCollection<T> collection)
    {
      _auditTasks.Add(
        collection.CollectionNamespace.CollectionName, 
        Task.Factory.StartNew(()=>_watchTask(collection), _cancellationToken));
    }

    private async Task _watchTask<T>(IMongoCollection<T> collection)
    {
      //Get the whole document instead of just the changed portion
      var options = new ChangeStreamOptions()
      {
        FullDocument = ChangeStreamFullDocumentOption.Default
      };

      //The operationType can be one of the following: insert, update, replace, delete, invalidate
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<T>>()
        .Match("{ operationType: { $in: [ 'replace', 'insert', 'update', 'delete' ] } }");

      using var cursor = await collection.WatchAsync(pipeline, options, _cancellationToken);
      await cursor.ForEachAsync(change =>
      {
        var auditDocument = new CollectionAuditDocument()
        {
          DocumentKey = change.DocumentKey,
          OperationType = change.OperationType.ToString(),
          RemovedFields = change.UpdateDescription?.RemovedFields,
          UpdatedFields = change.UpdateDescription?.UpdatedFields
        };
        // process change event
        _auditCollection.InsertOne(auditDocument, null, _cancellationToken);
        _logger.LogDebug($"{change.OperationType} on {change.DocumentKey}");
      },_cancellationToken);
    }
  }
}
