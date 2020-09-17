using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("Bin")]
  public class StorageBin: Document, IDocumentReference
  {
    public StorageBin()
    {
      Contents = new List<StorageBinContent>();
    }
    public List<StorageBinContent> Contents { get; set; }
    [BsonIgnore]
    public double Weight
    {
      get
      {
        return Contents?.Sum(_ => _.TotalWeight) ?? 0;
      }
    }
  }

  public class StorageBinInsertUpdateModel:IHasName
  {
    public string Name { get; set; }
    public List<StorageBinContentModel> Contents { get; set; }
  }

  public class StorageBinModel: StorageBinInsertUpdateModel
  {
    public string Id { get; set; }
    public double Weight { get; set; }
  }

  public class StorageBinValidator : AbstractValidator<StorageBinInsertUpdateModel>
  {
    public StorageBinValidator()
    {
      RuleFor(_ => _.Name).NotEmpty();
    }
  }

  public class StorageBinProfile : Profile
  {
    public StorageBinProfile()
    {
      CreateMap<StorageBin, StorageBinModel>();
      CreateMap<StorageBinContentInsertUpdateModel, StorageBin>()
        .ForMember(
          _ => _.Id,
          opt => opt.Ignore())
        .ForMember(
        _ => _.Weight,
        opt => opt.Ignore());
    }
  }
}