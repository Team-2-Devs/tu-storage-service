using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Storage.Api.Hosting;

public sealed class InternalAuthMiddleware
{
  private const string HeaderName = "x-internal-token";

  private readonly RequestDelegate _next;
  private readonly string _expectedToken;

  public InternalAuthMiddleware(RequestDelegate next, IOptions<InternalAuthOptions> options)
  {
    _next = next;
    _expectedToken = options.Value.ApiKey ?? string.Empty;
  }

  public async Task InvokeAsync(HttpContext context)
  {

    // if no secret is configured
    if (string.IsNullOrEmpty(_expectedToken))
    {
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      await context.Response.WriteAsync("Internal auth not configured");

      return;
    }

    // check for required header
    if (!context.Request.Headers.TryGetValue(HeaderName, out var headerValues))
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync("Missing internal token");

      return;
    }

    var provided = headerValues.ToString();

    // Validate token
    if (!IsValid(provided, _expectedToken))
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsync("Invalid internal token");

      return;
    }

    await _next(context);
  }

  private static bool IsValid(string provided, string expected)
  {
    var providedBytes = Encoding.UTF8.GetBytes(provided);
    var expectedBytes = Encoding.UTF8.GetBytes(expected);

    if (providedBytes.Length != expectedBytes.Length)
      return false;

    // prevents timing attacks
    return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
  }
}
