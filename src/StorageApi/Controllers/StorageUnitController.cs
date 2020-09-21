using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Repositories.Interfaces;
using Newtonsoft.Json;
using StorageApi.Models;

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StorageUnitController : CrudController<StorageUnit, StorageUnitModel, StorageUnitInsertUpdateModel>
  {
    public StorageUnitController(IRepository<StorageUnit> repository, IMapper mapper, ILogger<StorageUnitController> logger)
      : base(repository, mapper, logger)
    {
    }

    #region Overrides

    protected override void ValidateUpdateModel(StorageUnitInsertUpdateModel model)
    {
      throw new InvalidOperationException("Storage Units must be modified with specific actions");
    }

    #endregion
  }
}