using LoggingTrial.Internals;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(
    "v1",
    options => options.AddDocumentTransformer(new ServerAndPathsTransformer("/temp/api"))
);

builder.Services.AddMessaging().AddHttpJsonConfiguration().AddErrorHandling();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.UseExceptionHandler();

app.MapGetThing();

app.MapScalarApiReference(
    "docs",
    config =>
    {
        config.WithTitle("API reference").WithTheme(ScalarTheme.Laserwave);
    }
);

app.Run();
