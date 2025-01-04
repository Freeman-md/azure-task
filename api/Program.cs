using System.Text.Json;
using System.Text.Json.Serialization;
using api.Contracts;
using api.Middlewares;
using api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var  MyAllowedSpecificOrigins = "_myAllowedSpecificOrigins";


var connection = builder.Configuration.GetConnectionString("AzureSQL");

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowedSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    
}

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection, sqlServerOptionsAction: sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
}));

builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    });


var app = builder.Build();

// Ensure database is created or migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowedSpecificOrigins);

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();