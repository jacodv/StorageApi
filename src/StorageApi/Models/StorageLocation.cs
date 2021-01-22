using AutoMapper;
using FluentValidation;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("Location")]
  public class StorageLocation : Document, IDocumentReference
  {
  }

  public class StorageLocationModel : DocumentReferenceModel
  {
  }

  public class StorageLocationInsertUpdateModel: IHasName
  {
    public string Name { get; set; }
  }


  public class StorageLocationValidator : AbstractValidator<StorageLocationInsertUpdateModel>
  {
    public StorageLocationValidator()
    {
      RuleFor(v => v.Name).NotEmpty();
    }
  }

  public class StorageLocationProfile : Profile
  {
    public StorageLocationProfile()
    {
      CreateMap<StorageLocation, StorageLocationModel>();
      CreateMap<StorageLocationInsertUpdateModel, StorageLocation>()
        .ForMember(
          x => x.Id, 
          opt => opt.Ignore());
    }
  }
}