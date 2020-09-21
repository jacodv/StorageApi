using System;
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
    protected readonly IRepository<TDal> _repository;
    protected readonly IMapper _mapper;
    protected readonly ILogger<CrudController<TDal, TModel, TUpInsModel>> _logger;

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
      if (string.IsNullOrEmpty(id)) 
        throw new ArgumentNullException(nameof(id));

      return _mapper.Map<TDal, TModel>(await _repository.FindByIdAsync(id));
    }

    // POST api/<LocationController>
    [HttpPost]
    public async Task<TModel> Post([FromBody] TUpInsModel model)
    {
      if (model == null) 
        throw new ArgumentNullException(nameof(model));

      var insertedItem = await _repository.InsertOneAsync(_mapper.Map<TUpInsModel, TDal>(model));
      return _mapper.Map<TDal,TModel>(insertedItem);
    }

    // PUT api/<LocationController>/5
    [HttpPut("{id}")]
    public async Task<TModel> Put(string id, [FromBody] TUpInsModel model)
    {
      if (id == null) 
        throw new ArgumentNullException(nameof(id));
      if (model == null)
        throw new ArgumentNullException(nameof(model));

      ValidateUpdateModel(model);

      var itemToUpdate = _mapper.Map<TUpInsModel, TDal>(model);
      itemToUpdate.Id = ObjectId.Parse(id);
      var updatedItem = await _repository.ReplaceOneAsync(itemToUpdate);
      if(updatedItem==null)
        throw new ArgumentOutOfRangeException(nameof(id));
      return _mapper.Map<TDal, TModel>(updatedItem);
    }

    protected virtual void ValidateUpdateModel(TUpInsModel model)
    {
      
    }

    // DELETE api/<LocationController>/5
    [HttpDelete("{id}")]
    public async Task<TModel> Delete(string id)
    {
      if (id == null) 
        throw new ArgumentNullException(nameof(id));
      
      var deletedItem =  await _repository.DeleteByIdAsync(id);
      if (deletedItem == null)
        return default(TModel);
      return _mapper.Map<TDal, TModel>(deletedItem);
    }
  }
}