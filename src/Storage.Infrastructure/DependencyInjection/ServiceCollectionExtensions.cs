using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Storage.Application.Ports.Outbound;
using Storage.Infrastructure.ExternalServices.Minio;
using Storage.Infrastructure.Options;

namespace Storage.Infrastructure.DependencyInjection;

/// <summary>Dependency injection extensions for Infrastructure (clients, adapters, and options).</summary>
public static class ServiceCollectionExtensions
{
  /// <summary>Registers Infrastructure: binds configuration to typed options, creates the MinIO client (singleton), and registers outbound adapters.</summary>
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
  {
    // Bind configuration section to typed options and validate
    services
     .AddOptions<MinioOptions>()
     .Bind(config.GetSection("Minio"))
     .ValidateDataAnnotations()
     .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoint), "Minio.Endpoint required")
     .Validate(o => !string.IsNullOrWhiteSpace(o.AccessKey), "Minio.AccessKey required")
     .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Minio.SecretKey required")
     .Validate(o => !string.IsNullOrWhiteSpace(o.BucketName), "Minio.BucketName required")
     .ValidateOnStart();

    // Register MinIO client (singleton)
    services.AddSingleton(sp =>
    {
      var opt = sp.GetRequiredService<IOptions<MinioOptions>>().Value;

      return new MinioClient()
      .WithEndpoint(opt.Endpoint)
      .WithCredentials(opt.AccessKey, opt.SecretKey)
      .WithRegion(opt.Region)
      .WithSSL(opt.UseSsl)
      .Build();
    });

    // Register outbound adapter implementing the application port
    services.AddScoped<IObjectStoragePresigner, MinioObjectStoragePresigner>();

    return services;
  }
}
