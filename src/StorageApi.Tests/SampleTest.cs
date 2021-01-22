using System;
using Xunit;

namespace StorageApi.Tests
{
  public class SampleTest: IClassFixture<SampleFixture>, IDisposable
  {
    private readonly SampleFixture _sampleFixture;

    public SampleTest(SampleFixture sampleFixture)
    {
      _sampleFixture = sampleFixture;
    }

    [Fact]
    public void method_GiventestingFor_Shouldresult()
    {
      //Setup
      
      //Action

      //Assert
    }

    public void Dispose()
    {
      _sampleFixture?.Dispose();
    }
  }

  public class SampleFixture: IDisposable
  {
    public void Dispose()
    {
      
    }
  }
}
