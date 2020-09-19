using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using StorageApi.Helpers;
using StorageApi.Models;
using StorageApi.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace StorageApi.Tests.Integration
{
  public abstract class IntegrationTestBase<TModel, TInsUpdModel>: IDisposable
  where TModel:DocumentReferenceModel
  {
    protected IHost Host;
    protected HttpClient Client;
    protected HttpClient AnonClient;
    protected string BaseUrl { get; }
    protected readonly ILogger Output;
    private object _locker = new object();

    protected IntegrationTestBase(string baseUrl, ITestOutputHelper output)
    {
      BaseUrl = baseUrl;
      Output = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.TestOutput(output, LogEventLevel.Verbose)
        .CreateLogger()
        .ForContext<IntegrationTestBase<TModel, TInsUpdModel>>(); ;
      
      var hostBuilder = new HostBuilder()
        .ConfigureWebHost(webHost =>
        {
          // Add TestServer
          webHost.UseTestServer();
          webHost.UseStartup<Startup>();
        });

      Host = hostBuilder.StartAsync().Result;

      // Create a new scope
      using (var scope = Host.Services.CreateScope())
      {
        //Do the migration asynchronously
        lock (_locker)
        {
          DemoDataHelper.Populate(scope.ServiceProvider).Wait();
        }
      }
      AnonClient = Host.GetTestClient();
      Client = Host.GetTestClient();
    }

    [Fact]
    public async Task Get_GivenNoArguments_ShouldReturnItems()
    {
      //Action
      var items = await _getAllItems<TModel>();

      //Assert
      items.Count.Should().BeGreaterThan(0);
      Output.Debug(JsonConvert.SerializeObject(items));
    }

    [Fact]
    public async Task Get_GivenId_ShouldReturnItem()
    {
      // Setup
      var items = await _getAllItems<TModel>();

      //Action
      var item = await _getItem<TModel>(items.First().Id);

      //Assert
      item.Should().NotBeNull();
    }

    [Fact]
    public async Task Full_CRUD_ShouldInsertUpdateAndDelete()
    {
      //INSERT TESTS
      var insertedItem  = await _insertTest();

      //UPDATE TESTS
      if (_testUpdate)
      {
        var updatedItem = await _updateTest(insertedItem.Id);
        updatedItem.Name.Should().NotBe(insertedItem.Name);
      }

      //DELETE TESTS
      await _deleteAndValidate(insertedItem.Id, BaseUrl);
    }

    private async Task<TModel> _updateTest(string id)
    {
      //Setup
      var updateItem = GetUpdateModel();

      //Action
      var updatedItem = await _put<TModel>($"{BaseUrl}/{id}", updateItem);

      //Assert
      updatedItem.Should().NotBeNull();
      return updatedItem;
    }
    private async Task<TModel> _insertTest()
    {
      // Setup
      var newItem = GetInsertModel();

      //Action
      var insertedItem = await _post<TModel>(BaseUrl, newItem);

      //Assert
      insertedItem.Should().NotBeNull();
      return insertedItem;
    }

    #region Abstract

    protected abstract TInsUpdModel GetInsertModel();
    protected abstract TInsUpdModel GetUpdateModel();

    #endregion

    #region shared

    protected static async Task<T> _evaluateResponse<T>(HttpResponseMessage response)
    {
      if (response.IsSuccessStatusCode)
      {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
      }
      throw new InvalidOperationException($"Failed: {response.StatusCode}:\n{await response.Content.ReadAsStringAsync()}");
    }
    protected async Task<T> _get<T>(string url)
    {
      return await _evaluateResponse<T>(await Client.GetAsync(url));
    }
    protected async Task _deleteAndValidate(string id, string baseUrl)
    {
      var deleteResponse = await Client.DeleteAsync($"{baseUrl}/{id}");
      deleteResponse.IsSuccessStatusCode.Should().BeTrue();
      var getResponse = await Client.GetAsync($"{baseUrl}/{id}");
      getResponse.IsSuccessStatusCode.Should().BeTrue();
      getResponse.Content.ReadAsStringAsync().Result.Should().BeNullOrEmpty();
    }
    protected async Task<T> _post<T>(string url, object payload = null, bool isAnonymous = false)
    {
      var client = isAnonymous ? AnonClient : Client;

      if (payload == null)
        return await _evaluateResponse<T>(await client.PostAsync(url, null));
      return await _evaluateResponse<T>(await client.PostAsync(url, new JsonContent(payload)));
    }
    protected async Task<T> _put<T>(string url, object payload = null)
    {
      if (payload == null)
        return await _evaluateResponse<T>(await Client.PutAsync(url, null));
      return await _evaluateResponse<T>(await Client.PutAsync(url, new JsonContent(payload)));
    }
    protected async Task<List<T>> _getAllItems<T>()
    {
      return await Client.GetStringToItems<T>(BaseUrl);
    }
    protected async Task<T> _getItem<T>(string id)
    {
      return await Client.GetStringToItem<T>($"{BaseUrl}/{id}");
    }

    protected bool _testUpdate = true;
    #endregion

    #region IDispose
    public void Dispose()
    {
      Host?.Dispose();
      Client?.Dispose();
      AnonClient?.Dispose();
    }
    #endregion
  }
}