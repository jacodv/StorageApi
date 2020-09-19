using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;

namespace StorageApi.Models
{
  public class StorageBinContent
  {
    public StorageBinContent()
    {
      Tags = new List<string>();
    }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double UnitWeight { get; set; }
    [BsonIgnore]
    public double TotalWeight => Quantity * UnitWeight;

    public List<string> Tags { get; set; }
  }

  public class StorageBinContentInsertUpdateModel
  {
    public StorageBinContentInsertUpdateModel()
    {
      Tags = new List<string>();
    }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public double UnitWeight { get; set; }
    public List<string> Tags { get; set; }
  }

  public class StorageBinContentModel: StorageBinContentInsertUpdateModel
  {
    public double TotalWeight { get; set; }
  }

  public class StorageBinContentValidator : AbstractValidator<StorageBinContentInsertUpdateModel>
  {
    public StorageBinContentValidator()
    {
      RuleFor(_ => _.Name).NotEmpty();
      RuleFor(_ => _.Quantity).GreaterThanOrEqualTo(0);
      RuleFor(_ => _.UnitWeight).GreaterThanOrEqualTo(0);
    }
  }


  public class StorageBinContentProfile : Profile
  {
    public StorageBinContentProfile()
    {
      CreateMap<StorageBinContent, StorageBinContentModel>();
      CreateMap<StorageBinContentInsertUpdateModel, StorageBinContent>()
        .ForMember(
          x => x.TotalWeight,
          opt => opt.Ignore());
    }
  }
}