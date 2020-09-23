using System;
using System.Collections.Generic;
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
  public class ToDoTests
  {
    private static bool _testDataLoaded;
    private object _locker = new object();
    private IConfiguration _config;
    private ILogger<MongoRepository<Todo>> _logger;
    private MongoRepository<Todo> _repository;
    private Mock<IUserSession> _mockUserSession;
    private static IList<Todo> _testData;

    public ToDoTests(ITestOutputHelper output)
    {
      _config = InitConfiguration();
      var databaseSettings = new DatabaseSettings();
      _config.Bind("DatabaseSettings", databaseSettings);
      _logger = _createLogger(output);
      _mockUserSession = new Mock<IUserSession>(MockBehavior.Strict);
      _repository = new MongoRepository<Todo>(databaseSettings, _logger, _mockUserSession.Object);

      lock (_locker)
      {
        if (!_testDataLoaded)
        {
          _testData = SetupTestData(databaseSettings, "Todo");
        }
      }

      _logger.LogInformation("Todo Tests setup");
    }

    private ILogger<MongoRepository<Todo>> _createLogger(ITestOutputHelper output)
    {
      var serilogLogger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.TestOutput(output)
        .CreateLogger();

      var loggerFactory = (ILoggerFactory)new LoggerFactory();
      loggerFactory.AddSerilog(serilogLogger);

      return loggerFactory.CreateLogger<MongoRepository<Todo>>();
    }

    [Fact]
    public void Constructor_GivenValidInput_ShouldCreateInstance()
    {
      _repository.Should().NotBeNull();
    }

    [Fact]
    public void InsertOne_GivenValidItem_ShouldSetCreatedBy()
    {
      //Setup
      var testName = "InsertOne_GivenValidItem_ShouldSetCreatedBy";
      var newItem = Builder<Todo>
        .CreateNew()
        .With(x => x.Name = testName)
        .Build();
      _mockUserSession.Setup(mc => mc.GetUserName())
        .Returns(testName);

      //Action
      var insertedItem = _repository.InsertOne(newItem);

      //Assert
      _logger.LogDebug(JsonConvert.SerializeObject(insertedItem));
      insertedItem.CreatedBy.Should().Be(testName);
    }

    public static IConfiguration InitConfiguration()
    {
      var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
      return config;
    }

    public static IList<Todo> SetupTestData(IDatabaseSettings settings, string collectionName)
    {
      var todos = Builder<Todo>
        .CreateListOfSize(3)
        .All()
        .TheFirst(1)
        .With(x=>x.DueDate=null)
        .With(x=>x.Status=ActionStatus.Started)
        .TheNext(1)
        .With(x => x.DueDate = DateTime.Now.AddMonths(1).Date)
        .With(x => x.Status = ActionStatus.Created)
        .With(x=>x.Tags = new List<string>(){"Tag1"})
        .TheLast(1)
        .With(x => x.DueDate = DateTime.Now.AddDays(1).Date)
        .With(x => x.Status = ActionStatus.Cancelled)
        .Build();

      var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
      var collection = database.GetCollection<Todo>(collectionName);

      collection.DeleteMany(new BsonDocument());
      collection.InsertMany(todos);
      return todos;
    }
  }
}
