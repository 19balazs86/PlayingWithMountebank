using MbDotNet;
using MbDotNet.Models;
using System.Net;
using Xunit;

namespace PlayingWithMountebankTests;

public sealed class MountebankTests : IAsyncDisposable
{
    private const int _port = 2525;
    private readonly Uri _localhostUri = new Uri($"http://localhost:{_port}");

    private readonly HttpClient _httpClient;

    private readonly MountebankClient _mountebankClient;

    public MountebankTests()
    {
        _httpClient = new HttpClient { BaseAddress = _localhostUri };

        _mountebankClient = new MountebankClient();
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task TestStatus(HttpStatusCode statusCode)
    {
        // Arrange
        await _mountebankClient.DeleteAllImpostersAsync();

        _ = await _mountebankClient.CreateHttpImposterAsync(_port, nameof(TestStatus), imposter =>
        {
            imposter
                .AddStub()
                .OnPathAndMethodEqual("/user", Method.Get)
                .ReturnsStatus(statusCode);
        });

        // Act
        using var response = await _httpClient.GetAsync("/user");

        // Assert
        Assert.Equal(statusCode, response.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task TestJsonResponse(int id)
    {
        // Arrange
        await _mountebankClient.DeleteAllImpostersAsync();

        string path = $"/user/{id}";
        var user    = new User { Id = id, Name = $"Name_{id}" };

        _ = await _mountebankClient.CreateHttpImposterAsync(_port, nameof(TestJsonResponse), imposter =>
        {
            imposter
                .AddStub()
                .OnPathAndMethodEqual(path, Method.Get)
                .ReturnsJson(HttpStatusCode.OK, user);
        });

        // Act
        using var response = await _httpClient.GetAsync(path);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);

        User responseUser = await response.Content.ReadAsAsync<User>();

        Assert.Equal(user.Id, responseUser.Id);
        Assert.Equal(user.Name, responseUser.Name);
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient.Dispose();

        await _mountebankClient.DeleteAllImpostersAsync();
    }
}

public sealed class User
{
    public int Id { get; init; }
    public string Name { get; init; }
}
