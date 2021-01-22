using System;

namespace StorageAPI.Models
{
  public class Document
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string LastUpdatedBy { get; set; }
  }
}