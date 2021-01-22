using System;
using System.Collections.Generic;
using MongoDB.Repositories.Attributes;

namespace MongoDB.Repositories.Tests.Models
{
  [BsonCollection("Todo")]
  public class Todo: Document
  {
    public Todo()
    {
      Tags=new List<string>();
    }
    public string Author { get; set; }
    public List<string> Tags { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionStatus Status { get; set; }
  }
}