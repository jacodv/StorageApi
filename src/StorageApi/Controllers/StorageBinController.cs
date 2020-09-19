using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Repositories.Interfaces;
using StorageApi.Models;

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StorageBinController : CrudController<StorageBin, StorageBinModel, StorageBinInsertUpdateModel>
  {
    public StorageBinController(IRepository<StorageBin> repository, IMapper mapper, ILogger<StorageBinController> logger)
      : base(repository, mapper, logger)
    {

    }
  }
}