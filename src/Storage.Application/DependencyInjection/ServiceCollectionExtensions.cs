using Microsoft.Extensions.DependencyInjection;
using Storage.Application.Ports.Inbound;
using Storage.Application.UseCases;

namespace Storage.Application.DependencyInjection;

/// <summary>Dependency injection extensions for registering Application-layer services.</summary>
public static class ServiceCollectionExtensions
{
  /// <summary>Registers all Application-layer services, including use cases, validators, and behaviors.</summary>
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    // Use cases
    services.AddScoped<IPresignPutUrl, PresignPutUrl>();

    // Validators
    // Behaviors

    return services;
  }
}
