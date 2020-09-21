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
  public class StorageBinController : CrudController<StorageBin, StorageBinModel, StorageBinInsertUpdateModel>
  {
    public StorageBinController(IRepository<StorageBin> repository, IMapper mapper, ILogger<StorageBinController> logger)
      : base(repository, mapper, logger)
    {

    }

    [HttpPost]
    [Route("findTags")]
    public IEnumerable<StorageBinModel> GetContainingTags([FromBody]List<string> expectedTags)
    {
      if (expectedTags == null || !expectedTags.Any())
        throw new ArgumentOutOfRangeException( nameof(expectedTags),"Tags are null or empty");

      var matchingBins = _repository.AsQueryable()
        .Where(
          w =>
            w.StorageBinLocation != null &&
            w.Contents.Any(content => content.Tags.Intersect(expectedTags).Count() == expectedTags.Count))
        .ToList();
      _logger.LogInformation(JsonConvert.SerializeObject(matchingBins));

      return _mapper.Map<List<StorageBin>, List<StorageBinModel>>(matchingBins);
    }
  }
}