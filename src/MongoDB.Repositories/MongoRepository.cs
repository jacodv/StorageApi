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
  public class MongoRepository<TDocument> : IRepository<TDocument>, IDisposable
    where TDocument : IDocument
  {
    private readonly Task _watcher;
    public readonly IMongoCollection<TDocument> Collection;
    private readonly ILogger<MongoRepository<TDocument>> _logger;
    private readonly IUserSession _userSession;
    private readonly IMongoCollection<CollectionAuditDocument> _auditCollection;

    public MongoRepository(IDatabaseSettings settings, ILogger<MongoRepository<TDocument>> logger, IUserSession userSession)
    {
      _logger = logger;
      _userSession = userSession;
      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      var collectionName = GetCollectionName(typeof(TDocument));
      Collection = database.GetCollection<TDocument>(collectionName);
      _auditCollection = database.GetCollection<CollectionAuditDocument>($"Audit");
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


    public virtual TDocument InsertOne(TDocument document)
    {
      _setInsertState(document);
      Collection.InsertOne(document);
      return document;
    }
    public async Task<TDocument> InsertOneAsync(TDocument document)
    {
      _setInsertState(document);
      await Collection.InsertOneAsync(document);
      return document;
    }
    public IEnumerable<TDocument> InsertMany(ICollection<TDocument> documents)
    {
      foreach (var document in documents)
      {
        _setInsertState(document);
      }
      Collection.InsertMany(documents);
      return documents;
    }
    public async Task<IEnumerable<TDocument>> InsertManyAsync(ICollection<TDocument> documents)
    {
      foreach (var document in documents)
      {
        _setInsertState(document);
      }
      await Collection.InsertManyAsync(documents);
      return documents;
    }

    public TDocument ReplaceOne(TDocument document)
    {
      _setUpdateState(document);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
      Collection.FindOneAndReplace(filter, document);
      return document;
    }
    public async Task<TDocument> ReplaceOneAsync(TDocument document)
    {
      _setInsertState(document);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
      await Collection.FindOneAndReplaceAsync(filter, document);
      return document;
    }

    public TDocument DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Collection.FindOneAndDelete(filterExpression);
    }
    public async Task<TDocument> DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return await Collection.FindOneAndDeleteAsync(filterExpression);
    }
    public TDocument DeleteById(string id)
    {
      var objectId = new ObjectId(id);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
      return Collection.FindOneAndDelete(filter);
    }
    public async Task<TDocument> DeleteByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return await Collection.FindOneAndDeleteAsync(filter);
    }
    public long DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
      var result = Collection.DeleteMany(filterExpression);
      return result.DeletedCount;
    }
    public async Task<long> DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      var result =  await Collection.DeleteManyAsync(filterExpression);
      return result.DeletedCount;
    }

    #region Private
    private void _setInsertState(TDocument document)
    {
      document.CreatedBy = _userSession.GetUserName();
      document.UpdatedAt = null;
      document.CreatedBy = null;
    }
    private void _setUpdateState(TDocument document)
    {
      document.UpdatedAt = DateTime.Now;
      document.UpdatedBy = _userSession.GetUserName();
    }
    private async Task _addWatcher()
    {
      _logger.LogDebug($"Watcher started");

      var indexKeys = Builders<CollectionAuditDocument>.IndexKeys.Ascending(x => x.CreatedAt);
      var indexOptions = new CreateIndexOptions()
      {
        Name = "CreatedAt",
        Unique = false,
        ExpireAfter = TimeSpan.FromDays(30)
      };
      var createIndexModel = new CreateIndexModel<CollectionAuditDocument>(indexKeys, indexOptions);
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
          var auditDocument = new CollectionAuditDocument()
          {
            DocumentKey = change.DocumentKey,
            OperationType = change.OperationType,
            BackingDocument = change.BackingDocument,
            CollectionNamespace = change.CollectionNamespace,
            FullDocument = change.FullDocument,
            UpdateDescription = change.UpdateDescription,
          };
          // process change event
          _auditCollection.InsertOne(auditDocument);
          _logger.LogDebug($"{change.OperationType} on {change.DocumentKey}");

        });
      }
    }
    #endregion

    public void Dispose()
    {
      _watcher?.Dispose();
    }
  }
}