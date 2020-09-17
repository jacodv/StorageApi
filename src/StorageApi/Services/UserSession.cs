using MongoDB.Repositories.Interfaces;

namespace StorageApi.Services
{
  public class UserSession: IUserSession
  {
    private string _userName;

    public string GetUserName()
    {
      return _userName ?? "System";
    }
  }
}
