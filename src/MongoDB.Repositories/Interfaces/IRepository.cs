using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.Repositories.Interfaces
{
  public interface IRepository<TDocument> where TDocument : IDocument
  {
    IQueryable<TDocument> AsQueryable();

    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<TDocument> FindByIdAsync(string id);

    Task<TDocument> InsertOneAsync(TDocument document);
    Task<IEnumerable<TDocument>> InsertManyAsync(ICollection<TDocument> documents);

    Task<TDocument> ReplaceOneAsync(TDocument document);

    Task<TDocument> DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<TDocument> DeleteByIdAsync(string id);
    Task<long> DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
  }
}
