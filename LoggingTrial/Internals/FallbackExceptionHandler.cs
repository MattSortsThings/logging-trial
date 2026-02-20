using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoggingTrial.Internals;

/// <summary>
///     Converts any exception thrown on the server into an HTTP response with status code 500 and a serialized
///     <see cref="ProblemDetails" /> object.
/// </summary>
/// <remarks>
///     This class is adapted from a very helpful
///     <a href="https://timdeschryver.dev/blog/translating-exceptions-into-problem-details-responses">blog post</a> by Tim
///     Deschryver.
/// </remarks>
/// <param name="problemDetailsService">Creates the <see cref="ProblemDetails" /> response.</param>
internal sealed class FallbackExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken _
    )
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        string exceptionType = exception.GetType().Name;

        ProblemDetails problemDetails = new()
        {
            Title = "Unexpected Error",
            Detail =
                $"An exception of type '{exceptionType}' was thrown while handling the request.",
            Status = StatusCodes.Status500InternalServerError,
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problemDetails }
        );
    }
}
