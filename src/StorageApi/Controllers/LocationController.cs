using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Repositories.Interfaces;
using Serilog;
using StorageApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LocationController : CrudController<Location, LocationModel, LocationInsertUpdateModel>
  {
    public LocationController(IRepository<Location> locations, IMapper mapper, ILogger<LocationController> logger)
    : base(locations, mapper, logger)
    {

    }
  }
}

