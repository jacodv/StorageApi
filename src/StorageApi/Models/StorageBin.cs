using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Helpers;
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
    public StorageBinLocation StorageBinLocation { get; set; }
  }

  public class StorageBinLocation
  {
    public DocumentReference Unit { get; set; }
    public DocumentReference Location { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }

    public static StorageBinLocation FromUnit(StorageUnit unit, int rowIndex, int columnIndex)
    {
      return new StorageBinLocation()
      {
        Unit = unit.ToReference(),
        Location = unit.Location,
        RowIndex = rowIndex,
        ColumnIndex = columnIndex
      };
    }
  }

  public class StorageBinInsertUpdateModel:IHasName
  {
    public string Name { get; set; }
    public List<StorageBinContentInsertUpdateModel> Contents { get; set; }
  }

  public class StorageBinModel: DocumentReferenceModel
  {
    public double Weight { get; set; }
    public List<StorageBinContentModel> Contents { get; set; }
    public StorageBinLocationModel StorageBinLocation { get; set; }
  }

  public class StorageBinLocationModel
  {
    public DocumentReferenceModel Unit { get; set; }
    public DocumentReferenceModel Location { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
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
      CreateMap<StorageBinLocation, StorageBinLocationModel>()
        .ReverseMap();
      CreateMap<StorageBin, StorageBinModel>();
      CreateMap<StorageBinInsertUpdateModel, StorageBin>()
        .ForMember(
          _ => _.Id,
          opt => opt.Ignore())
        .ForMember(
        _ => _.Weight,
        opt => opt.Ignore());
    }
  }
}