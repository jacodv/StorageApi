﻿using StorageApi.Interfaces;

namespace StorageApi.Models
{
  public class StorageColumn
  {
    public int Index { get; set; }
    public DocumentReference Bin { get; set; }
  }
}