using AutoMapper;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Repositories;

namespace StorageApi.Models
{
  public class DocumentReferenceModel
  {
    public string Id { get; set; }
    public string Name { get; set; }
  }

  public class DocumentReferenceValidator : AbstractValidator<DocumentReferenceModel>
  {
    public DocumentReferenceValidator()
    {
      RuleFor(_ => _.Id).NotEmpty();
      RuleFor(_ => _.Name).NotEmpty();
    }
  }

  public class DocumentReferenceProfile : Profile
  {
    public DocumentReferenceProfile()
    {
      CreateMap<DocumentReference, DocumentReferenceModel>()
        .ReverseMap()
        .ForMember(
          _=>_.Id,
          opt=>opt.MapFrom(x=>ObjectId.Parse(x.Id)));
    }
  }
}