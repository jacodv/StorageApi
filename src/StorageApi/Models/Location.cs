using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using MongoDB.Repositories;
using MongoDB.Repositories.Attributes;
using MongoDB.Repositories.Interfaces;
using StorageApi.Interfaces;

namespace StorageApi.Models
{
  [BsonCollection("Location")]
  public class Location : Document, IDocumentReference
  {
  }

  public class LocationModel : DocumentReferenceModel
  {
  }

  public class LocationInsertUpdateModel: IHasName
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