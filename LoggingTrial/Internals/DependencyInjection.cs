using System.Text.Json;
using System.Text.Json.Serialization;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;

namespace LoggingTrial.Internals;

/// <summary>
///     Extension methods to be invoked at the application composition root.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Adds the error handling services to the application service descriptor collection.
    /// </summary>
    /// <param name="services">Contains service descriptors for the application.</param>
    /// <returns>The original <see cref="IServiceCollection" /> instance.</returns>
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(ConfigureProblemDetailsOptions);

        services
            .AddExceptionHandler<InvalidEnumArgumentExceptionHandler>()
            .AddExceptionHandler<BadHttpRequestExceptionHandler>()
            .AddExceptionHandler<TimeoutDbUpdateExceptionHandler>()
            .AddExceptionHandler<FallbackExceptionHandler>();

        return services;
    }

    private static void ConfigureProblemDetailsOptions(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = CustomizeInstance;
    }

    private static void CustomizeInstance(ProblemDetailsContext context)
    {
        HttpRequest request = context.HttpContext.Request;

        context.ProblemDetails.Instance = $"{request.Method} {request.Path}{request.QueryString}";
    }

    public static IServiceCollection AddHttpJsonConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        });

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services
            .AddSlimMessageBus(builder =>
                builder
                    .WithProviderMemory()
                    .AutoDeclareFrom(
                        typeof(DomainError).Assembly,
                        messageTypeToTopicConverter: type => type.FullName
                    )
            )
            .AddHttpContextAccessor();

        return services;
    }
}
