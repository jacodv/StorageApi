using System.Collections.Generic;
using AutoMapper;
using FluentValidation;

namespace StorageApi.Models
{
  public class StorageRow
  {
    public StorageRow()
    {
      StorageColumns = new List<StorageColumn>();
    }
    public int Index { get; set; }
    public List<StorageColumn> StorageColumns { get; set; }
  }

  public class StorageRowModel
  {
    public int Index { get; set; }
    public List<StorageColumnModel> StorageColumns { get; set; }
  }

  public class StorageRowValidator : AbstractValidator<StorageRowModel>
  {
    public StorageRowValidator()
    {
      RuleFor(_ => _.Index).GreaterThanOrEqualTo(0);
      RuleFor(_ => _.StorageColumns).NotEmpty();
    }
  }

  public class StorageRowProfile : Profile
  {
    public StorageRowProfile()
    {
      CreateMap<StorageRow, StorageRowModel>()
        .ReverseMap();
    }
  }
}