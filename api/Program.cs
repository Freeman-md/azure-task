using api.Contracts;
using api.Middlewares;
using api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("AzureSQL");
}
else
{
    connection = Environment.GetEnvironmentVariable("AzureSQL");
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();