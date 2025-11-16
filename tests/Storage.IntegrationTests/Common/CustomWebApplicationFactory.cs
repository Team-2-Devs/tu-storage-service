using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Application.Ports.Outbound;
using Storage.Domain.ValueObjects;

namespace Storage.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration((context, configbuilder) =>
    {
      // Test only configuration for internal auth
      var testConfig = new Dictionary<string, string?>()
      {
        ["InternalAuth:ApiKey"] = "test-storage-token"
      };

      configbuilder.AddInMemoryCollection(testConfig);
    });

    builder.ConfigureServices(services =>
    {
      // Replace real ObjectStoragePresigner with fake
      var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IObjectStoragePresigner));
      if (descriptor is not null) services.Remove(descriptor);

      services.AddSingleton<IObjectStoragePresigner, FakeObjectStoragePresigner>();
    });
  }
}

file sealed class FakeObjectStoragePresigner : IObjectStoragePresigner
{
  public Task<(string url, DateTimeOffset expiresAt)> PresignGetAsync(
    ObjectKey key,
    TtlSeconds ttl,
    CancellationToken ct = default)
    => Task.FromResult(("http://local/presigned", new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero)));

  public Task<(string url, DateTimeOffset expiresAt)> PresignPutAsync(
    ObjectKey key,
    ContentType contentType,
    TtlSeconds ttl,
    CancellationToken ct = default)
    => Task.FromResult(("http://local/presigned", new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero)));
}
