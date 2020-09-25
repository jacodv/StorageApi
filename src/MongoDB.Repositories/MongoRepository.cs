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
    public readonly IMongoCollection<TDocument> Collection;
    private readonly ILogger<MongoRepository<TDocument>> _logger;
    private readonly IUserSession _userSession;

    public MongoRepository(IDatabaseSettings settings, 
      ILogger<MongoRepository<TDocument>> logger, 
      IUserSession userSession)
    {
      _logger = logger;
      _userSession = userSession;
      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      var collectionName = GetCollectionName(typeof(TDocument));
      Collection = database.GetCollection<TDocument>(collectionName);
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
    
    public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return Task.Run(() => Collection.Find(filterExpression).FirstOrDefaultAsync());
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


    public async Task<TDocument> InsertOneAsync(TDocument document)
    {
      _setInsertState(document);
      await Collection.InsertOneAsync(document);
      return document;
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

    public async Task<TDocument> ReplaceOneAsync(TDocument document)
    {
      _setInsertState(document);
      var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
      await Collection.FindOneAndReplaceAsync(filter, document);
      return document;
    }

    public async Task<TDocument> DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
      return await Collection.FindOneAndDeleteAsync(filterExpression);
    }
    public async Task<TDocument> DeleteByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return await Collection.FindOneAndDeleteAsync(filter);
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
      document.UpdatedBy = null;
    }
    private void _setUpdateState(TDocument document)
    {
      document.UpdatedAt = DateTime.Now;
      document.UpdatedBy = _userSession.GetUserName();
    }
    #endregion
  }
}