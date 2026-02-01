using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Extensions;
using Utility.Mediator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "API for managing tasks and jobs"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Add Infrastructure services
DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration);

// Add Mediator
Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(assembly => assembly.FullName != null && assembly.FullName.StartsWith("TaskManager"))
    .ToArray();
builder.Services.AddMediator(assemblies);

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations and seed data
await app.InitializeDatabaseAsync();

app.UseCors("frontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();