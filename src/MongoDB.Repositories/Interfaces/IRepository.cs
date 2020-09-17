﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Repositories.Interfaces
{
  public interface IRepository<TDocument> where TDocument : IDocument
  {
    IQueryable<TDocument> AsQueryable();

    IEnumerable<TDocument> FilterBy(
      Expression<Func<TDocument, bool>> filterExpression);
    IEnumerable<TProjected> FilterBy<TProjected>(
      Expression<Func<TDocument, bool>> filterExpression,
      Expression<Func<TDocument, TProjected>> projectionExpression);

    TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);
    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    TDocument FindById(string id);
    Task<TDocument> FindByIdAsync(string id);

    TDocument InsertOne(TDocument document);
    Task<TDocument> InsertOneAsync(TDocument document);
    IEnumerable<TDocument> InsertMany(ICollection<TDocument> documents);
    Task<IEnumerable<TDocument>> InsertManyAsync(ICollection<TDocument> documents);

    TDocument ReplaceOne(TDocument document);
    Task<TDocument> ReplaceOneAsync(TDocument document);

    TDocument DeleteOne(Expression<Func<TDocument, bool>> filterExpression);
    Task<TDocument> DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    TDocument DeleteById(string id);
    Task<TDocument> DeleteByIdAsync(string id);
    long DeleteMany(Expression<Func<TDocument, bool>> filterExpression);
    Task<long> DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
  }
}
