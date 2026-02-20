using System.ComponentModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoggingTrial.Internals;

/// <summary>
///     Converts any <see cref="InvalidEnumArgumentException" /> thrown on the server into an HTTP response with
///     status code 400 and a serialized <see cref="ProblemDetails" /> object.
/// </summary>
/// <remarks>
///     This class is adapted from a very helpful
///     <a href="https://timdeschryver.dev/blog/translating-exceptions-into-problem-details-responses">blog post</a> by Tim
///     Deschryver.
/// </remarks>
/// <param name="problemDetailsService">Creates the <see cref="ProblemDetails" /> response.</param>
internal sealed class InvalidEnumArgumentExceptionHandler(
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken _
    )
    {
        if (exception is not InvalidEnumArgumentException invalidEnumArgumentException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        ProblemDetails problemDetails = new()
        {
            Title = "Invalid Enum Argument",
            Detail =
                $"An exception of type '{nameof(InvalidEnumArgumentException)}' was thrown while handling the request.",
            Status = StatusCodes.Status400BadRequest,
            Extensions = { { "exceptionMessage", invalidEnumArgumentException.Message } },
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problemDetails }
        );
    }
}
