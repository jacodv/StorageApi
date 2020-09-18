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
    public async Task<ActionResult<TModel>> Post([FromBody] TUpInsModel model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var insertedItem = await _repository.InsertOneAsync(_mapper.Map<TUpInsModel, TDal>(model));
      return _mapper.Map<TDal,TModel>(insertedItem);
    }

    // PUT api/<LocationController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult<TModel>> Put(string id, [FromBody] TUpInsModel model)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var itemToUpdate = _mapper.Map<TUpInsModel, TDal>(model);
      itemToUpdate.Id = ObjectId.Parse(id);
      var updatedItem = await _repository.ReplaceOneAsync(itemToUpdate);
      return _mapper.Map<TDal, TModel>(updatedItem);
    }

    // DELETE api/<LocationController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<TModel>> Delete(string id)
    {
      var deletedItem =  await _repository.DeleteByIdAsync(id);
      if (deletedItem == null)
        return NotFound(id);
      return _mapper.Map<TDal, TModel>(deletedItem);
    }
  }
}