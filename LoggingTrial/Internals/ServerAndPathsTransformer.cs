using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LoggingTrial.Internals;

public sealed class ServerAndPathsTransformer(string basePath) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        ReplaceServer(document);
        ReplacePaths(document);

        return Task.CompletedTask;
    }

    private void ReplaceServer(OpenApiDocument document)
    {
        string updatedServerUrl = document.Servers?.FirstOrDefault() is { } existing
            ? existing.Url?.TrimEnd('/') + "/" + basePath.TrimStart('/')
            : basePath;

        updatedServerUrl = EnsureStartsWithHttps(updatedServerUrl);

        document.Servers = new List<OpenApiServer> { new() { Url = updatedServerUrl } };
    }

    private void ReplacePaths(OpenApiDocument document)
    {
        OpenApiPaths updatedPaths = new();

        foreach ((string path, IOpenApiPathItem item) in document.Paths)
        {
            updatedPaths.Add(path.Replace(basePath, string.Empty), item);
        }

        document.Paths = updatedPaths;
    }

    private static string EnsureStartsWithHttps(string serverUrl) => serverUrl;
}
