using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace LoggingTrial.Internals;

/// <summary>
///     Extension methods for the <see cref="DomainError" /> record type.
/// </summary>
internal static class DomainErrorExtensions
{
    /// <summary>
    ///     Maps the domain error to a <see cref="ProblemDetails" /> object.
    /// </summary>
    /// <param name="domainError">The domain error to be mapped.</param>
    /// <returns>A new <see cref="ProblemDetails" /> instance.</returns>
    internal static ProblemDetails ToProblemDetails(this DomainError domainError)
    {
        (int statusCode, string problemType) = domainError.Type.ToProblemStatusCodeAndType();

        return new ProblemDetails
        {
            Title = domainError.Title,
            Detail = domainError.Description,
            Status = statusCode,
            Type = problemType,
            Extensions =
                domainError.AdditionalData?.ToDictionary(kvp => kvp.Key, object? (kvp) => kvp.Value)
                ?? [],
        };
    }

    private static (int StatusCode, string Type) ToProblemStatusCodeAndType(
        this DomainErrorType domainErrorType
    )
    {
        return domainErrorType switch
        {
            DomainErrorType.Unexpected => (
                StatusCodes.Status500InternalServerError,
                "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            ),
            DomainErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            ),
            DomainErrorType.Extrinsic => (
                StatusCodes.Status409Conflict,
                "https://tools.ietf.org/html/rfc9110#section-15.5.10"
            ),
            DomainErrorType.Intrinsic => (
                StatusCodes.Status422UnprocessableEntity,
                "https://tools.ietf.org/html/rfc9110#section-15.5.21"
            ),
            _ => throw new InvalidEnumArgumentException(
                nameof(domainErrorType),
                (int)domainErrorType,
                typeof(DomainErrorType)
            ),
        };
    }
}
