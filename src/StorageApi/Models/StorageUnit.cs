using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("StorageUnit")]
  public class StorageUnit : Document, IDocumentReference
  {
    public StorageUnit()
    {
      Rows = new List<StorageRow>();
    }
    public List<StorageRow> Rows { get; set; }
    public DocumentReference Location { get; set; }

    public static List<StorageRow> FromLayout(int rows, int columnsPerRow)
    {
      var result = new List<StorageRow>(rows);
      result.ForEach(row=>row.StorageColumns=new List<StorageColumn>(columnsPerRow));
      return result;
    }
  }

  public class StorageUnitInsertUpdateModel:IHasName
  {
    public string Name { get; set; }
    public int Rows { get; set; }
    public int ColumnsPerRow { get; set; }
  }

  public class StorageUnitModel: DocumentReferenceModel 
  {
    public StorageUnitModel()
    {
      Rows = new List<StorageRowModel>();
    }
    public List<StorageRowModel> Rows { get; set; }
    public DocumentReferenceModel Location { get; set; }
  }

  public class StorageUnitValidator : AbstractValidator<StorageUnitInsertUpdateModel>
  {
    public StorageUnitValidator()
    {
      RuleFor(_ => _.Rows).GreaterThan(0);
      RuleFor(_ => _.ColumnsPerRow).GreaterThan(0);
    }
  }

  public class StorageUnitProfile : Profile
  {
    public StorageUnitProfile()
    {
      CreateMap<StorageUnit, StorageUnitModel>();
      CreateMap<StorageUnitInsertUpdateModel, StorageUnit>()
        .ForMember(
          storageUnit => storageUnit.Rows,
          map => map.MapFrom(
            source => StorageUnit.FromLayout(source.Rows, source.ColumnsPerRow)
          ));
    }
  }
}