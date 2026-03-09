/*
 * FuelApp API bootstrap summary
 *
 * Application purpose:
 * - Configures a REST API for FuelApp domain entities (players, associations, teams, staff members, evaluations).
 * - Uses Entity Framework Core with SQL Server via FuelAppContext.
 * - Registers repository interfaces/implementations for data access.
 * - Enables OpenAPI/Swagger and controller-based endpoints.
 *
 * Core service registration:
 * - DbContext: FuelAppContext (SQL Server connection from configuration).
 * - Repositories: IPlayerRepository, IAssociationRepository, ITeamRepository,
 *   IStaffMemberRepository, IEvaluationRepository.
 * - Controllers + JSON enum string serialization.
 * - CORS policy: AllowBlazor (http://localhost:5015).
 * - HTTP logging and OpenAPI/Swagger generation.
 *
 * Middleware stack (in execution order):
 * 1) Routing (UseRouting)
 * 2) Environment-specific middleware
 *    - Development: CORS, DeveloperExceptionPage, Swagger, SwaggerUI, OpenAPI endpoint, HTTP logging
 *    - Production: ExceptionHandler, HSTS, HTTPS redirection
 * 3) Custom request sanitization middleware (path/query/body inspection)
 * 4) Endpoint mapping (MapControllers)
 */

using FuelApp.Data;
using FuelApp.Repositories.Implementations;
using FuelApp.Repositories.Interfaces;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore; // swagger generation
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework to use SQL Server with the connection string from appsettings.json
builder.Services.AddDbContext<FuelAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

// Add API explorer for generating OpenAPI documentation
builder.Services.AddEndpointsApiExplorer(); 

// Add Swagger generation with XML comments for better documentation
var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(xmlPath);
}); 

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); 

// builtin HTTP logging middleware
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
}); 

// Add controllers with JSON string enum converter
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }); 

// Configure CORS to allow requests from the Blazor client application
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        builder => builder
            .WithOrigins("http://localhost:5015")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Register repositories for dependency injection
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IAssociationRepository, AssociationRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();

// Build the application
var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    // Log the application URLs on startup for easy access to Swagger and testing
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var addresses = app.Urls.Any()
            ? app.Urls
            : app.Services.GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>()?.Addresses ?? Array.Empty<string>();

        foreach (var address in addresses)
        {
            logger.LogInformation("Swagger UI: {SwaggerUrl}", $"{address}/swagger/index.html");
        }
    });
}

app.UseRouting(); // Enable routing

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowBlazor");
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
    app.UseSwagger(); // Enable Swagger middleware for development
    app.UseSwaggerUI(); // Enable Swagger UI middleware for development
    app.MapOpenApi(); // Map OpenAPI endpoints for development
    app.UseHttpLogging(); // Enable HTTP logging middleware
}
else
{
    app.UseExceptionHandler("/Error"); // Use a generic error handler in production
    app.UseHsts(); // Use HTTP Strict Transport Security in production
    app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS in production
}

// global error handling in place for all environments

// Note: Authentication and authorization middleware are not enabled in this version, part of to do list
//app.UseAuthentication(); // Enable authentication middleware
//app.UseAuthorization(); // Enable authorization middleware

// Global sanitization middleware - inspects path, query, and body (custom practice)
// Will explore more robust built-in or third-party sanitization libraries in the future
app.Use(async (context, next) =>
{
    var path = context.Request.Path.ToString();
    var query = context.Request.QueryString.ToString();
    
    // Reject requests with dangerous characters in path or query
    if (Regex.IsMatch(path, "[<>\"']") || Regex.IsMatch(query, "[<>\"']"))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = "Request path or query contains potentially dangerous characters." });
        return;
    }

    // Enable buffering to allow reading the body multiple times
    context.Request.EnableBuffering();

    // Read and inspect the request body if present
    if (context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
    {
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            
            // Reject requests with dangerous patterns in body 
            // ' is not a threat because we are using parameterized queries, but we will check for angle brackets and common XSS patterns
            if (Regex.IsMatch(body, "[<>]") || Regex.IsMatch(body, "script|iframe|onclick|onerror", RegexOptions.IgnoreCase))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = "Request body contains potentially dangerous content." });
                return;
            }
            
            // Reset the body stream position so the controller can read it
            context.Request.Body.Position = 0;
        }
    }

    await next();
});

app.MapControllers(); 

app.Run();
