using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Repositories.Interfaces;
using MongoDB.Repositories.Settings;
using MongoDB.Repositories.Tests.Models;
using Moq;
using Newtonsoft.Json;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace MongoDB.Repositories.Tests
{
  public class TodoRepositoryTests : IClassFixture<ToDoRepositoryFixture>, IDisposable
  {
    private readonly ToDoRepositoryFixture _toDoRepositoryFixture;

    public TodoRepositoryTests(ToDoRepositoryFixture toDoRepositoryFixture, ITestOutputHelper output)
    {
      _toDoRepositoryFixture = toDoRepositoryFixture;
      _toDoRepositoryFixture.Setup(output);
    }

    [Fact]
    public void Constructors_GivenValidInput_ShouldInstances()
    {
      //Assert
      _toDoRepositoryFixture.Should().NotBeNull();
      _toDoRepositoryFixture.TodoRepository.Should().NotBeNull();
      _toDoRepositoryFixture.Logger.LogInformation("Construction passed");
      _toDoRepositoryFixture.TodosTestData.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AsQueryable_GivenNoFilter_ShouldReturnAllItems()
    {
      //Setup
      
      //Action
      var allItems = _toDoRepositoryFixture.TodoRepository.AsQueryable().ToList();
      
      //Assert
      allItems.Count.Should().BeGreaterThan(0);
      _toDoRepositoryFixture.Logger.LogDebug("allItems.Count: {0}", allItems.Count);
    }

    [Fact]
    public void AsQueryable_GivenValidFilter_ShouldReturnFilteredItems()
    {
      //Setup
      var allItems = _toDoRepositoryFixture.TodoRepository.AsQueryable().ToList();
      var lastItemId = allItems.Last().Id;

      //Action
      var filteredItem = _toDoRepositoryFixture.TodoRepository
        .AsQueryable()
        .Where(x => x.Id == lastItemId)
        .ToList();

      //Assert
      filteredItem.Count.Should().Be(1);
    }

    [Fact]
    public async Task InsertOne_GivenValidItem_ShouldSetCreatedBy()
    {
      //Setup
      var testName = "InsertOne_GivenValidItem_ShouldSetCreatedBy";
      var newItem = Builder<Todo>
        .CreateNew()
        .With(x => x.Name = testName)
        .Build();
      _toDoRepositoryFixture.MockUserSession.Setup(mc => mc.GetUserName())
        .Returns(testName);

      //Action
      var insertedItem = await _toDoRepositoryFixture.TodoRepository.InsertOneAsync(newItem);

      //Assert
      _toDoRepositoryFixture.Logger.LogDebug(JsonConvert.SerializeObject(insertedItem));
      insertedItem.CreatedBy.Should().Be(testName);
    }

    public void Dispose()
    {
      _toDoRepositoryFixture?.Dispose();
    }
  }

  public class ToDoRepositoryFixture : IDisposable
  {
    public const string TodoCollectionName = "Todo";
    private DatabaseSettings _databaseSettings;

    public ToDoRepositoryFixture()
    {
      Config = InitConfiguration();
      var databaseSettings = new DatabaseSettings();
      Config.Bind("DatabaseSettings", databaseSettings);
      MockUserSession = new Mock<IUserSession>(MockBehavior.Strict);
      _databaseSettings = databaseSettings;
    }

    public readonly IConfiguration Config;
    public Mock<IUserSession> MockUserSession;
    public ILogger<MongoRepository<Todo>> Logger;

    public IList<Todo> TodosTestData { get; private set; }
    public MongoRepository<Todo> TodoRepository { get; private set; }

    public void Setup(ITestOutputHelper output)
    {
      Logger = CreateLogger<Todo>(output);
      TodoRepository = new MongoRepository<Todo>(_databaseSettings, Logger, MockUserSession.Object);
      TodosTestData = SetupTestToDos(_databaseSettings);
      Logger.LogInformation("TodoTestCollectionFeature setup");
    }

    private static ILogger<MongoRepository<T>> CreateLogger<T>(ITestOutputHelper output)
    where T:Document
    {
      var serilogLogger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.TestOutput(output)
        .CreateLogger();

      var loggerFactory = (ILoggerFactory)new LoggerFactory();
      loggerFactory.AddSerilog(serilogLogger);

      return loggerFactory.CreateLogger<MongoRepository<T>>();
    }

    public static IList<Todo> SetupTestToDos(IDatabaseSettings settings)
    {
      var todos = Builder<Todo>.CreateListOfSize(3)
        .All()
        .TheFirst(1)
        .With(x => x.DueDate = null)
        .With(x => x.Status = ActionStatus.Started)
        .TheNext(1)
        .With(x => x.DueDate = DateTime.Now.AddMonths(1).Date)
        .With(x => x.Status = ActionStatus.Created)
        .With(x => x.Tags = new List<string>() { "Tag1" })
        .TheLast(1)
        .With(x => x.DueDate = DateTime.Now.AddDays(1).Date)
        .With(x => x.Status = ActionStatus.Cancelled)
        .Build();

      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      var collection = database.GetCollection<Todo>(TodoCollectionName);

      collection.DeleteMany(new BsonDocument());
      collection.InsertMany(todos);
      return todos;
    }
    public static IConfiguration InitConfiguration()
    {
      var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
      return config;
    }

    public void Dispose()
    {
      MockUserSession?.VerifyAll();
    }
  }
}
