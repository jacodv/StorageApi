using StorageApi.Interfaces;
using StorageApi.Models;

namespace StorageApi.Helpers
{
  public static class ReferenceHelper
  {
    public static DocumentReference ToReference(this IDocumentReference source)
    {
      return new DocumentReference()
      {
        Id = source.Id,
        Name = source.Name
      };
    }
  }
}
