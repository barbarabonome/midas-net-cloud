using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence;
using Midas.Infrastructure.Persistence.Repositories;
using System.Text.Json.Serialization;
using Midas.API.Filters;
using Midas.API.Services;
using Midas.API.HealthChecks;
using Midas.API.Business.Interfaces;
using Midas.API.Business.Implementations;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5220";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Midas.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SchemaFilter<FiltroCamposBooleanos>();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
Title = "Midas API",
        Version = "v1",
        Description = "API para controle financeiro - Sistema Midas"
    });
});

// Configure DbContext for Oracle
builder.Services.AddDbContext<MidasContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"),
   b => b.UseOracleSQLCompatibility("11")));

// Register repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IGastoRepository, GastoRepository>();
builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();
builder.Services.AddScoped<ICofrinhoRepository, CofrinhoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Register Business services
builder.Services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();
builder.Services.AddScoped<IGastoBusiness, GastoBusiness>();
builder.Services.AddScoped<IReceitaBusiness, ReceitaBusiness>();
builder.Services.AddScoped<ICorinhoBusiness, CorinhoBusiness>();
builder.Services.AddScoped<ICategoriaBusiness, CategoriaBusiness>();

// Register HATEOAS service
builder.Services.AddScoped<HateoasLinkGenerator>();

// Register Health Check services para injeção de dependência
builder.Services.AddScoped<ApiHealthCheck>();
builder.Services.AddScoped<OracleHealthCheck>();

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api")
    .AddCheck<OracleHealthCheck>("oracle");

// Configure CORS - mais permissivo para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
       .AllowAnyMethod()
   .AllowAnyHeader();
    });
});

var app = builder.Build();

// Adicionar CORS antes de outras middlewares
app.UseCors("AllowAll");

// Middleware para logging de requisições com correlação
app.Use(async (context, next) =>
{
  var correlationId = context.Request.Headers.ContainsKey("X-Correlation-ID")
        ? context.Request.Headers["X-Correlation-ID"].ToString()
     : Guid.NewGuid().ToString();

    context.Items["CorrelationId"] = correlationId;
    context.Response.Headers.Add("X-Correlation-ID", correlationId);

    using (LogContext.PushProperty("CorrelationId", correlationId))
    using (LogContext.PushProperty("RequestPath", context.Request.Path))
    using (LogContext.PushProperty("RequestMethod", context.Request.Method))
    {
    Log.Information("Requisição iniciada: {Method} {Path}", context.Request.Method, context.Request.Path);
  await next(context);
        Log.Information("Requisição finalizada com status: {StatusCode}", context.Response.StatusCode);
 }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
 c.SwaggerEndpoint("/swagger/v1/swagger.json", "Midas API v1");
   c.RoutePrefix = "swagger";
        c.DocumentTitle = "Midas API Documentation";
    });
    app.UseDeveloperExceptionPage();
}

// Comentando HTTPS redirect para testes
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map health checks endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

app.Run();
