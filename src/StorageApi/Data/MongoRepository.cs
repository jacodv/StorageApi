﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;

namespace StorageApi.Data
{
  public class MongoRepository<TDocument> : IRepository<TDocument>
    where TDocument : IDocument
  {
    public readonly IMongoCollection<TDocument> Collection;

    public MongoRepository(IDatabaseSettings settings)
    {
      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      Collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
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
  }
}