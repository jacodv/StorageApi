using System.Collections.Generic;

namespace StorageApi.Models
{
  public class StorageRow
  {
    public StorageRow()
    {
      StorageColumns = new List<StorageColumn>();
    }
    public int Index { get; set; }
    public List<StorageColumn> StorageColumns { get; set; }
  }
}