using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Repositories;
using MongoDB.Repositories.Interfaces;
using Moq;
using Serilog;
using StorageApi.Controllers;
using StorageApi.Interfaces;
using StorageApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace StorageApi.Tests
{
  public abstract class ControllerTestBase<TDal, TModel, TInsUpdateModel, TController>
    where TDal : Document
    where TModel : DocumentReferenceModel
    where TInsUpdateModel : IHasName
    where TController : CrudController<TDal, TModel, TInsUpdateModel>
  {
    private readonly Mapper _mapper;
    private TController _crudController;
    private Mock<IRepository<TDal>> _mockRepository;

    protected ControllerTestBase(ITestOutputHelper output, Profile mapperProfile)
    {
      var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
      _mapper = new Mapper(mapperConfiguration);

      var serilogLogger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.TestOutput(output)
        .CreateLogger();

      var loggerFactory = (ILoggerFactory)new LoggerFactory();
      loggerFactory.AddSerilog(serilogLogger);

      var logger = loggerFactory.CreateLogger<TController>();
      _mockRepository = new Mock<IRepository<TDal>>(MockBehavior.Strict);

      _crudController = (TController)Activator
        .CreateInstance(
          typeof(TController),
          new object[]
          {
            _mockRepository.Object,
            _mapper,
            logger
          });
    }

    [Fact]
    public void Get_Items_ShouldReturnItems()
    {
      //Setup
      var itemsSetup = AllItems(2);
      _mockRepository
        .Setup(mc => mc.AsQueryable())
        .Returns(itemsSetup.AsQueryable());

      //Action
      var locationsResult = _crudController.Get();

      //Assert
      locationsResult.Count().Should().BeGreaterThan(0);
    }
    [Fact]
    public void Get_Locations_ShouldNot_ReturnMoreThan50Locations()
    {
      //Setup
      var itemsSetup = AllItems(55);
      _mockRepository
        .Setup(mc => mc.AsQueryable())
        .Returns(itemsSetup.AsQueryable());

      //Action
      var locationsResult = _crudController.Get();

      //Assert
      locationsResult.Count().Should().BeLessOrEqualTo(50);
    }

    [Fact]
    public async Task Get_GivenId_Should_ReturnLocation()
    {
      var itemSetup = NewItem();

      _mockRepository
        .Setup(mc => mc.FindByIdAsync(It.IsAny<string>()))
        .ReturnsAsync(NewItem);

      // Action
      var locationResult = await _crudController.Get(itemSetup.Id.ToString());

      // Assert
      locationResult.Should().NotBeNull();
      locationResult.Name.Should().Be(itemSetup.Name);
    }

    [Fact]
    public async Task Post_GivenNewItem_Should_ReturnInsert()
    {
      var listOfItems = new List<TDal>();
      var newItem = ToBeInsertedItem();

      _mockRepository
        .Setup(mc => mc.InsertOneAsync(It.IsAny<TDal>()))
        .Returns(Task.CompletedTask)
        .Callback<TDal>(listOfItems.Add);

      // Action
      var postResult = await _crudController.Post(newItem);

      // Assert
      (postResult as OkResult).Should().NotBeNull();
      listOfItems.Count.Should().Be(1);
    }

    [Fact]
    public async Task Put_GivenExistingItem_Should_Update()
    {
      var itemSetup = NewItem();
      var existingItem = ToBeUpdatedItem();

      _mockRepository
        .Setup(mc => mc.ReplaceOneAsync(It.IsAny<TDal>()))
        .Returns(Task.CompletedTask)
        .Callback<TDal>(updated =>
        {
          itemSetup.Name = updated.Name;
        });

      // Action
      var locationResult = await _crudController.Put(itemSetup.Id.ToString(), existingItem);

      // Assert
      (locationResult as OkResult).Should().NotBeNull();
      existingItem.Name.Should().Be(itemSetup.Name);
    }

    [Fact]
    public async Task Delete_GivenExistingItem_Should_Delete()
    {
      //Setup
      var listOfItems = new Dictionary<string, TDal>();
      var itemSetup = NewItem();
      listOfItems.Add(itemSetup.Id.ToString(), itemSetup);

      _mockRepository
        .Setup(mc => mc.DeleteByIdAsync(It.IsAny<string>()))
        .Returns(Task.CompletedTask)
        .Callback<string>(id => listOfItems.Remove(id));

      // Action
      var locationResult = await _crudController.Delete(itemSetup.Id.ToString());

      // Assert
      (locationResult as OkResult).Should().NotBeNull();
      listOfItems.Count.Should().Be(0);
    }

    #region Abstract

    protected abstract IList<TDal> AllItems(int count);
    protected abstract TDal NewItem();
    protected abstract TInsUpdateModel ToBeInsertedItem();
    protected abstract TInsUpdateModel ToBeUpdatedItem();

    #endregion
  }
}