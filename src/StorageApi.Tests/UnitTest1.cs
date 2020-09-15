using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using StorageApi.Controllers;
using StorageApi.Interfaces;
using StorageApi.Models;
using Xunit;

namespace StorageApi.Tests
{
  public class LocationTests
  {
    private LocationController _locationController;
    private readonly Mapper _mapper;
    private Mock<IRepository<Location>> _mockLocations;

    public LocationTests()
    {
      var locationProfile = new LocationProfile();
      var mapperConfiguration = new MapperConfiguration(cfg=>cfg.AddProfile(locationProfile));
      _mapper = new Mapper(mapperConfiguration);
      _mockLocations = new Mock<IRepository<Location>>(MockBehavior.Strict);


      _locationController = new LocationController(_mockLocations.Object, _mapper);
    }
    [Fact]
    public void Get_Locations_ShouldReturnLocations()
    {
      //Setup
      var locations = Builder<Location>.CreateListOfSize(2).Build();
      _mockLocations
        .Setup(mc => mc.AsQueryable())
        .Returns(locations.AsQueryable());

      //Action
      var locationsResult = _locationController.Get();

      //Assert
      locationsResult.Count().Should().BeGreaterThan(0);
    }
    [Fact]
    public void Get_Locations_ShouldNot_ReturnMoreThan50Locations()
    {
      //Setup
      var locations = Builder<Location>.CreateListOfSize(55).Build();
      _mockLocations
        .Setup(mc => mc.AsQueryable())
        .Returns(locations.AsQueryable());

      //Action
      var locationsResult = _locationController.Get();

      //Assert
      locationsResult.Count().Should().BeLessOrEqualTo(50);
    }

    [Fact]
    public async Task Get_GivenId_Should_ReturnLocation()
    {
      var location = Builder<Location>
        .CreateNew()
        .With(_ => _.Id = ObjectId.GenerateNewId())
        .Build();

      _mockLocations
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(location);

      // Action
      var locationResult = await _locationController.Get(location.Id.ToString());

      // Assert
      locationResult.Should().NotBeNull();
      locationResult.Name.Should().Be(location.Name);
    }

    [Fact]
    public async Task Post_GivenNewItem_Should_ReturnInsert()
    {
      var listOfLocations = new List<Location>();
      var location = Builder<Location>
        .CreateNew()
        .Build();
       var newItem =new LocationInsertUpdateModel()
       {
         Name = location.Name
       };

      _mockLocations
        .Setup(mc => mc.InsertOneAsync(It.IsAny<Location>()))
        .Returns(Task.CompletedTask)
        .Callback<Location>(listOfLocations.Add);

      // Action
      var locationResult = await _locationController.Post(newItem);

      // Assert
      (locationResult as OkResult).Should().NotBeNull();
      listOfLocations.Count.Should().Be(1);
      listOfLocations.First().Name.Should().Be(location.Name);
    }
  }
}
