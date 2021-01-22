using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("assign")]
    public async Task<StorageBinModel> AssignBinToUnit([FromServices] IRepository<StorageUnit> unitRepository,[FromBody] AssignStorageBinModel model)
    {
      if (model == null) 
        throw new ArgumentNullException(nameof(model));

      var unit = await unitRepository.FindByIdAsync(model.UnitId);
      if (unit == null)
        throw new ArgumentOutOfRangeException(nameof(model), $"Invalid Unit Id:{model.UnitId}");

      var bin = await _repository.FindByIdAsync(model.BinId);
      if(bin==null)
        throw new ArgumentOutOfRangeException(nameof(model), $"Invalid Bin Id:{model.UnitId}");

      var existingBin = unit.GetAssignedBin(model.RowIndex, model.ColumnIndex);
      if (existingBin == null)
      {
        // Assign new bin
        var assignedBin = unit.AssignBin(bin, model.RowIndex, model.ColumnIndex);
        await unitRepository.ReplaceOneAsync(unit);
        await _repository.ReplaceOneAsync(assignedBin);
        return _mapper.Map<StorageBin, StorageBinModel>(assignedBin);
      }

      // Clear previous
      var unassignedBin = await _repository.FindByIdAsync(existingBin.Id);
      unassignedBin.StorageBinLocation = null;
      await _repository.ReplaceOneAsync(unassignedBin);

      // Assign new bin
      var replacedBin = unit.AssignBin(bin, model.RowIndex, model.ColumnIndex);
      await unitRepository.ReplaceOneAsync(unit);
      await _repository.ReplaceOneAsync(replacedBin);
      return _mapper.Map<StorageBin, StorageBinModel>(replacedBin);
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