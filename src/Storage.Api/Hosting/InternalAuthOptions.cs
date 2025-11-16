namespace Storage.Api.Hosting;

public sealed class InternalAuthOptions
{
  public const string SectionName = "InternalAuth";

  public string ApiKey { get; set; } = string.Empty;
}
