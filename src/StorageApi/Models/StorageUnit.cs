using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Helpers;
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
      var storageRows = new List<StorageRow>();
      for (var rowIndex = 0; rowIndex < rows; rowIndex++)
      {
        var row = new StorageRow(){Index = rowIndex};
        for (var columnIndex = 0; columnIndex < columnsPerRow; columnIndex++)
        {
          row.StorageColumns.Add(new StorageColumn(){Index = columnIndex});
        }
        storageRows.Add(row);
      }
      return storageRows;
    }
    
    public DocumentReference GetAssignedBin(int rowIndex, int columnIndex)
    {
      if(Rows==null || Rows.Count-1<rowIndex)
        throw new ArgumentOutOfRangeException(nameof(rowIndex), $"Invalid rowIndex:{rowIndex}");

      if (Rows[rowIndex].StorageColumns==null || Rows[rowIndex].StorageColumns.Count - 1 < columnIndex)
        throw new ArgumentOutOfRangeException(nameof(columnIndex), $"Invalid columnIndex:{columnIndex} for rowIndex:{rowIndex}");

      return Rows[rowIndex].StorageColumns[columnIndex].Bin;
    }
    public StorageBin AssignBin(StorageBin bin, int rowIndex, int columnIndex)
    {
      if (bin == null) 
        throw new ArgumentNullException(nameof(bin));
      
      if (Rows == null || Rows.Count - 1 < rowIndex)
        throw new ArgumentOutOfRangeException(nameof(rowIndex), $"Invalid rowIndex:{rowIndex}");

      if (Rows[rowIndex].StorageColumns == null || Rows[rowIndex].StorageColumns.Count - 1 < columnIndex)
        throw new ArgumentOutOfRangeException(nameof(columnIndex), $"Invalid columnIndex:{columnIndex} for rowIndex:{rowIndex}");

      bin.StorageBinLocation  = StorageBinLocation.FromUnit(this, rowIndex, columnIndex);
      Rows[rowIndex].StorageColumns[columnIndex].Bin = bin.ToReference();
      return bin;
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