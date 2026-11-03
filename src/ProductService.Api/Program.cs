using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Grpc.Swagger;
using Microsoft.OpenApi.Models;
using ProductService.Api.Grpc;
using ProductService.Api.Interceptors;
using ProductService.Api.Middleware;
using ProductService.Application;
using ProductService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = false;
    options.TimestampFormat = "HH:mm:ss ";
});
builder.Logging.AddDebug();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// gRPC + JSON transcoding (транскодинг публикует те же ручки по HTTP: /v2/products).
// Валидация живёт в прикладном слое, поэтому gRPC, транскодинг и REST проверяются одинаково.
builder.Services
    .AddGrpc(options =>
    {
        // Порядок важен: логирование внешнее, оно видит уже итоговый статус,
        // который формирует ExceptionInterceptor.
        options.Interceptors.Add<LoggingInterceptor>();
        options.Interceptors.Add<ExceptionInterceptor>();
    })
    .AddJsonTranscoding();

builder.Services.AddGrpcReflection();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Вид товара в JSON — строкой: "General", "HouseholdChemicals", "Electronics", "Food".
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Service API",
        Version = "v1",
        Description = "Сервис хранения информации о товарах. "
                      + "REST: /api/v1/products. gRPC: /v2/products.",
    });
    
    options.CustomSchemaIds(type => (type.FullName ?? type.Name)
        .Replace("ProductService.", string.Empty, StringComparison.Ordinal)
        .Replace('+', '.'));

    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
        options.IncludeGrpcXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();
app.MapGrpcService<ProductsGrpcService>();
app.MapGrpcReflectionService();

app.Run();
