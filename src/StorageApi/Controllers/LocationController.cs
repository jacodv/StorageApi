using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Repositories.Interfaces;
using StorageApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LocationController : CrudController<StorageLocation, StorageLocationModel, StorageLocationInsertUpdateModel>
  {
    public LocationController(IRepository<StorageLocation> locations, IMapper mapper, ILogger<LocationController> logger)
    : base(locations, mapper, logger)
    {

    }
  }
}

