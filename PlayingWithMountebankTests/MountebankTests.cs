using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Imposters;
using Xunit;

namespace PlayingWithMountebankTests
{
  public class MountebankTests : IDisposable
  {
    private const int _port   = 4546;
    private Uri _localhostUri = new Uri($"http://localhost:{_port}");

    private readonly HttpClient _httpClient;

    private readonly MountebankClient _mountebankClient;
    private readonly HttpImposter _imposter;

    public MountebankTests()
    {
      _httpClient = new HttpClient { BaseAddress = _localhostUri };

      _mountebankClient = new MountebankClient();

      _mountebankClient.DeleteAllImposters();

      _imposter = _mountebankClient.CreateHttpImposter(_port);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task TestStatus(HttpStatusCode statusCode)
    {
      // Arrange
      _imposter
        .AddStub()
        .OnPathAndMethodEqual("/user", Method.Get)
        .ReturnsStatus(statusCode);

      _mountebankClient.Submit(_imposter);

      // Act
      using var response = await _httpClient.GetAsync("/user");

      // Assert
      Assert.Equal(statusCode, response.StatusCode);
    }

    public void Dispose()
    {
      _httpClient.Dispose();

      _mountebankClient.DeleteAllImposters();
    }
  }
}
