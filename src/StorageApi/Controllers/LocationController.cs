using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Repositories;
using MongoDB.Repositories.Interfaces;
using StorageApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LocationController : CrudController<Location, LocationModel, LocationInsertUpdateModel>
  {
    public LocationController(IRepository<Location> locations, IMapper mapper)
    : base(locations, mapper)
    {
      
    }  
  }

  public abstract class CrudController<TDal, TModel, TUpInsModel> : ControllerBase
  where TDal:Document
  {
    private readonly IRepository<TDal> _repository;
    private readonly IMapper _mapper;

    protected CrudController(IRepository<TDal> repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
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
      await _repository.InsertOneAsync(_mapper.Map<TUpInsModel, TDal>(model));
      return Ok();
    }

    // PUT api/<LocationController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] TUpInsModel model)
    {
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
