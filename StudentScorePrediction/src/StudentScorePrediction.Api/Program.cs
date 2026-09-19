using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.ML.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Student Score Prediction API",
        Version = "v1",
        Description = "API for predicting student final scores using ML.NET"
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application layer
builder.Services.AddApplication();

// Add ML Service
builder.Services.AddSingleton<IMlService, MlService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

// Initialize database and generate sample dataset on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var mlService = scope.ServiceProvider.GetRequiredService<IMlService>();
    
    await context.Database.EnsureCreatedAsync();
    
    // Generate initial dataset if none exists
    var datasetCount = await context.DatasetInfos.CountAsync();
    if (datasetCount == 0)
    {
        Console.WriteLine("Generating initial dataset with 10,000 records...");
        await mlService.GenerateDatasetAsync(10000, CancellationToken.None);
        Console.WriteLine("Initial dataset generated successfully.");
    }
}

app.Run();
