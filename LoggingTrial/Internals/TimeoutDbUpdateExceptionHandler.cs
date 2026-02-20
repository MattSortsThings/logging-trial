using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoggingTrial.Internals;

/// <summary>
///     Converts a <see cref="DbUpdateException" /> thrown on the server due to a database connection or command timeout
///     into an HTTP response with status code 503, a "Retry-After" header, and a serialized <see cref="ProblemDetails" />
///     object.
/// </summary>
/// <remarks>
///     This class is adapted from a very helpful
///     <a href="https://timdeschryver.dev/blog/translating-exceptions-into-problem-details-responses">blog post</a> by Tim
///     Deschryver.
/// </remarks>
/// <param name="problemDetailsService">Creates the <see cref="ProblemDetails" /> response.</param>
/// <param name="dbConnectionOptions">Contains options for connecting to the application database.</param>
internal sealed class TimeoutDbUpdateExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (
            exception is not DbUpdateException dbUpdateException
            || !dbUpdateException.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
        )
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        httpContext.Response.Headers.RetryAfter = "120";

        ProblemDetails problemDetails = new()
        {
            Title = "Database Timeout",
            Detail =
                "The database connection or command timeout expired while handling the request.",
            Status = StatusCodes.Status503ServiceUnavailable,
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problemDetails }
        );
    }
}
