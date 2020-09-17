using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StorageApi.Tests.Helpers
{
  public static class HttpClientHelper
  {
    public static async Task<List<T>> GetStringToItems<T>(this HttpClient client, string url)
    {
      var stringResult = await client.GetStringAsync(url);
      return JsonConvert.DeserializeObject<List<T>>(stringResult);
    }
    public static async Task<T> GetStringToItem<T>(this HttpClient client, string url)
    {
      var stringResult = await client.GetStringAsync(url);
      return JsonConvert.DeserializeObject<T>(stringResult);
    }
  }
}
