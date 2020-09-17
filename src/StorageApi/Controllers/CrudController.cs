using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Repositories;
using MongoDB.Repositories.Interfaces;

namespace StorageApi.Controllers
{
  public abstract class CrudController<TDal, TModel, TUpInsModel> : ControllerBase
    where TDal:Document
  {
    private readonly IRepository<TDal> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CrudController<TDal, TModel, TUpInsModel>> _logger;

    protected CrudController(IRepository<TDal> repository, IMapper mapper, ILogger<CrudController<TDal, TModel, TUpInsModel>> logger)
    {
      _repository = repository;
      _mapper = mapper;
      _logger = logger;
      _logger.LogInformation("Info from controller");
    }
    // GET: api/<LocationController>
    [HttpGet]
    public IEnumerable<TModel> Get()
    {
      return _mapper.Map<IEnumerable<TDal>, IEnumerable<TModel>>(_repository.AsQueryable().Take(50).ToList());
    }

    // GET api/<LocationController>/5
    [HttpGet("{id}")]
    public async Task<TModel> Get(string id)
    {
      return _mapper.Map<TDal, TModel>(await _repository.FindByIdAsync(id));
    }

    // POST api/<LocationController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TUpInsModel model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      await _repository.InsertOneAsync(_mapper.Map<TUpInsModel, TDal>(model));
      return Ok();
    }

    // PUT api/<LocationController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] TUpInsModel model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var itemToUpdate = _mapper.Map<TUpInsModel, TDal>(model);
      itemToUpdate.Id = ObjectId.Parse(id);
      await _repository.ReplaceOneAsync(itemToUpdate);
      return Ok();
    }

    // DELETE api/<LocationController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      await _repository.DeleteByIdAsync(id);
      return Ok();
    }
  }
}