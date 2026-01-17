using Microsoft.OpenApi;
using Scalar.AspNetCore;
using SimpleKsef.Schema.Filters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleKsef.Infrastructure.Web;

/// <summary>
/// API extension methods for configuring and using API-related services.
/// 
/// OpenAPI will be available at /openapi/v1.json
/// Scalar docs will be available at /api
/// </summary>
public static class ApiExtensions
{
    public static TBuilder AddApi<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddRouting(o => { o.LowercaseUrls = true; });
        builder.Services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        });

        builder.Services.AddControllers(o =>
        {
          o.ReturnHttpNotAcceptable = true;

          o.Filters.Add<TZnakowyNormalizationFilter>();
        }).AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        });

        builder.Services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        });

        var apiVersioning = builder.Services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new(1, 0);
            o.ReportApiVersions = true;
        });

        builder.Services.AddOpenApi(o => {
          o.AddDocumentTransformer((document, context, ct) =>
          {
              document.Info.Title = "Simple KSeF API";
              document.Info.Description = "API for issuing and managing KSeF invoices";
              return Task.CompletedTask;
          });
          
          o.AddOperationTransformer((operation, context, ct) =>
          {
            operation.Responses ??= new OpenApiResponses();

            // 415
            if (!operation.Responses.TryGetValue("415", out var r415))
            {
                r415 = new OpenApiResponse();
                operation.Responses["415"] = r415;
            }
            r415.Description =
                "Unsupported Media Type. This endpoint accepts only JSON. " +
                "Send requests with Content-Type: application/json.";

            // 406
            if (!operation.Responses.TryGetValue("406", out var r406))
            {
                r406 = new OpenApiResponse();
                operation.Responses["406"] = r406;
            }
            r406.Description =
                "Not Acceptable. This endpoint produces only JSON. " +
                "Set Accept: application/json (or */*).";

            return Task.CompletedTask;
          });
        });

        apiVersioning.AddApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'VVV";
            o.SubstituteApiVersionInUrl = true;
        });
        
        return builder;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.MapControllers();
        app.UseExceptionHandler();

        if (!app.Environment.IsProduction())
        {
            app.MapOpenApi();
            app.MapScalarApiReference("api", o =>
            {
                o.TagSorter = TagSorter.Alpha;
            });
        }

        return app;
    }
}
