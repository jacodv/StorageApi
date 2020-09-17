using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace MongoDB.Repositories
{
  public class MongoRepository<TDocument> : IRepository<TDocument>
    where TDocument : IDocument
  {
    private Task _watcher;
    public readonly IMongoCollection<TDocument> Collection;
    private readonly ILogger<MongoRepository<TDocument>> _logger;
    private readonly IMongoCollection<ChangeStreamDocument<TDocument>> _auditCollection;

    public MongoRepository(IDatabaseSettings settings, ILogger<MongoRepository<TDocument>> logger )
    {
      _logger = logger;
      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      var collectionName = GetCollectionName(typeof(TDocument));
      Collection = database.GetCollection<TDocument>(collectionName);
      _auditCollection = database.GetCollection<ChangeStreamDocument<TDocument>>($"{collectionName}Audit");
      _watcher = _addWatcher();
    }

    private protected string GetCollectionName(Type documentType)
    {
      return ((BsonCollectionAttribute)documentType
        .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
        .FirstOrDefault())?.CollectionName;
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
      return Collection.AsQueryable();
    }
    public virtual IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Collection.Find(filterExpression).ToEnumerable();
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(
      Expression<Func<TDocument, bool>> filterExpression,
      Expression<Func<TDocument, TProjected>> projectionExpression)
    {
      return Collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Collection.Find(filterExpression).FirstOrDefault();
    }
    public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Task.Run(() => Collection.Find(filterExpression).FirstOrDefaultAsync());
    }
    public virtual TDocument FindById(string id)
    {
      var objectId = new ObjectId(id);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
      return Collection.Find(filter).SingleOrDefault();
    }
    public virtual Task<TDocument> FindByIdAsync(string id)
    {
      return Task.Run(() =>
      {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return Collection.Find(filter).SingleOrDefaultAsync();
      });
    }


    public virtual void InsertOne(TDocument document)
    {
      Collection.InsertOne(document);
    }
    public virtual Task InsertOneAsync(TDocument document)
    {
      return Task.Run(() => Collection.InsertOneAsync(document));
    }
    public void InsertMany(ICollection<TDocument> documents)
    {
      Collection.InsertMany(documents);
    }
    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
      await Collection.InsertManyAsync(documents);
    }

    public void ReplaceOne(TDocument document)
    {
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
      Collection.FindOneAndReplace(filter, document);
    }
    public virtual async Task ReplaceOneAsync(TDocument document)
    {
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
      await Collection.FindOneAndReplaceAsync(filter, document);
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
      Collection.FindOneAndDelete(filterExpression);
    }
    public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Task.Run(() => Collection.FindOneAndDeleteAsync(filterExpression));
    }
    public void DeleteById(string id)
    {
      var objectId = new ObjectId(id);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
      Collection.FindOneAndDelete(filter);
    }
    public Task DeleteByIdAsync(string id)
    {
      return Task.Run(() =>
      {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        Collection.FindOneAndDeleteAsync(filter);
      });
    }
    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
      Collection.DeleteMany(filterExpression);
    }
    public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Task.Run(() => Collection.DeleteManyAsync(filterExpression));
    }


    private async Task _addWatcher()
    {
      _logger.LogDebug($"Watcher started");

      var indexKeys = Builders<ChangeStreamDocument<TDocument>>.IndexKeys.Ascending(x => x.ClusterTime);
      var indexOptions = new CreateIndexOptions()
      {
        Name = "ClusterTime",
        Unique = false,
        ExpireAfter = TimeSpan.FromDays(30)
      };
      var createIndexModel = new CreateIndexModel<ChangeStreamDocument<TDocument>>(indexKeys, indexOptions);
      await _auditCollection.Indexes.CreateOneAsync(createIndexModel);

      //Get the whole document instead of just the changed portion
      var options = new ChangeStreamOptions()
      {
        FullDocument = ChangeStreamFullDocumentOption.Default
      };

      //The operationType can be one of the following: insert, update, replace, delete, invalidate
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<TDocument>>()
        .Match("{ operationType: { $in: [ 'replace', 'insert', 'update', 'delete' ] } }");

      using (var cursor = await Collection.WatchAsync(pipeline, options))
      {
        await cursor.ForEachAsync(change =>
        {
          // process change event
          _auditCollection.InsertOne(change);
          _logger.LogDebug($"{change.OperationType} on {change.DocumentKey}");

        });
      }
    }
  }
}