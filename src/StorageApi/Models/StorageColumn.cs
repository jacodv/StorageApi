using AutoMapper;
using FluentValidation;
using MongoDB.Repositories;

namespace StorageApi.Models
{
  public class StorageColumn
  {
    public int Index { get; set; }
    public DocumentReference Bin { get; set; }
  }

  public class StorageColumnModel
  {
    public int Index { get; set; }
    public DocumentReferenceModel Bin { get; set; }
  }

  public class StorageColumnValidator : AbstractValidator<StorageColumnModel>
  {
    public StorageColumnValidator()
    {
      RuleFor(_ => _.Index).GreaterThanOrEqualTo(0);
    }
  }

  public class StorageColumnProfile : Profile
  {
    public StorageColumnProfile()
    {
      CreateMap<StorageColumn, StorageColumnModel>()
        .ReverseMap();
    }
  }
}