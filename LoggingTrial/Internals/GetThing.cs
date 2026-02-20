using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlimMessageBus;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace LoggingTrial.Internals;

public static class GetThing
{
    public static void MapGetThing(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("temp/api/things", HandleAsync)
            .WithName("GetThing")
            .WithDisplayName("GetThing")
            .WithSummary("Get thing")
            .Produces<GetThingResponseBody>()
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] Query query,
        [FromServices] IRequestResponseBus bus,
        CancellationToken ct = default
    )
    {
        Result<GetThingResponseBody, DomainError> result = await bus.Send(
            query,
            cancellationToken: ct
        );

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return TypedResults.Problem(result.Error.ToProblemDetails());
    }

    private sealed record Query : IRequest<Result<GetThingResponseBody, DomainError>>
    {
        [Required]
        [FromQuery(Name = "mode")]
        public required ThingMode Mode { get; init; }
    }

    private sealed class QueryHandler
        : IRequestHandler<Query, Result<GetThingResponseBody, DomainError>>
    {
        public async Task<Result<GetThingResponseBody, DomainError>> OnHandle(
            Query request,
            CancellationToken cancellationToken
        )
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

            if (request.Mode == ThingMode.Known)
            {
                return new GetThingResponseBody("Thing is known!");
            }

            if (request.Mode == ThingMode.ThrowDbTimeout)
            {
                throw new DbUpdateException("Database timeout occurred.");
            }

            if (request.Mode == ThingMode.ThrowDbUpdate)
            {
                throw new DbUpdateException("Database constraint violated.");
            }

            if (request.Mode == ThingMode.ThrowDivideByZero)
            {
                throw new DivideByZeroException("Attempted to divide by zero.");
            }

            return new DomainError
            {
                Title = "Thing Unknown",
                Description = "The specified thing is unknown.",
                Type = DomainErrorType.Intrinsic,
                AdditionalData = new Dictionary<string, object>
                {
                    { "thingMode", request.Mode.ToString() },
                    { "number", 12345 },
                },
            };
        }
    }
}

public sealed record GetThingResponseBody(string Message);
