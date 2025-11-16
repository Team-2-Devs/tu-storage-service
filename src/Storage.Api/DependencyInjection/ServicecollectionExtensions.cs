using Storage.Api.Hosting;

namespace Storage.Api.DependencyInjection;

public static class ServicecollectionExtensions
{
  public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
  {
    AddInternalAuth(services, config);

    return services;
  }

  private static void AddInternalAuth(IServiceCollection services, IConfiguration config)
  {
    services.Configure<InternalAuthOptions>(config.GetSection(InternalAuthOptions.SectionName));
  }
}
