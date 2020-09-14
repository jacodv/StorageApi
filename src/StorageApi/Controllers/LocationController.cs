using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using StorageApi.Helpers;
using StorageApi.Interfaces;
using StorageApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StorageApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LocationController : ControllerBase
  {
    private readonly IRepository<Location> _locations;
    private readonly IMapper _mapper;

    public LocationController(IRepository<Location> locations, IMapper mapper)
    {
      _locations = locations;
      _mapper = mapper;
    }
    // GET: api/<LocationController>
    [HttpGet]
    public IEnumerable<DocumentReference> Get()
    {
      return _locations.AsQueryable().Take(50).ToList().Select(s=>s.ToReference());
    }

    // GET api/<LocationController>/5
    [HttpGet("{id}")]
    public async Task<Location> Get(string id)
    {
      return await _locations.FindByIdAsync(id);
    }

    // POST api/<LocationController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LocationInsertUpdateModel model)
    {
      await _locations.InsertOneAsync(_mapper.Map<LocationInsertUpdateModel, Location>(model));
      return Ok();
    }

    // PUT api/<LocationController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] LocationInsertUpdateModel model)
    {
      var itemToUpdate = _mapper.Map<LocationInsertUpdateModel, Location>(model);
      itemToUpdate.Id = ObjectId.Parse(id);
      await _locations.ReplaceOneAsync(itemToUpdate);
      return Ok();
    }

    // DELETE api/<LocationController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      await _locations.DeleteByIdAsync(id);
      return Ok();
    }
  }
}
