using AutoMapper;
using FluentValidation;
using StorageApi.Data;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("Location")]
  public class Location : Document, IDocumentReference
  {
    public string Name { get; set; }
  }

  public class LocationModel : IReferenceModel
  {
    public string Id { get; set; }
    public string Name { get; set; }
  }

  public class LocationInsertUpdateModel
  {
    public string Name { get; set; }
  }

  public class LocationValidator : AbstractValidator<LocationInsertUpdateModel>
  {
    public LocationValidator()
    {
      RuleFor(v => v.Name).NotEmpty();
    }
  }

  public class LocationProfile : Profile
  {
    public LocationProfile()
    {
      CreateMap<Location, LocationModel>();
      CreateMap<LocationInsertUpdateModel, Location>()
        .ForMember(
          x => x.Id, 
          opt => opt.Ignore());
    }
  }
}