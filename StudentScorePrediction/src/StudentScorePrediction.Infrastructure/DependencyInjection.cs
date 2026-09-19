using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Interfaces;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IPredictionRepository, PredictionRepository>();
        services.AddScoped<IModelVersionRepository, ModelVersionRepository>();
        services.AddScoped<ITrainingRunRepository, TrainingRunRepository>();
        services.AddScoped<IDatasetInfoRepository, DatasetInfoRepository>();
        services.AddScoped<IModelMetricRepository, ModelMetricRepository>();
        
        return services;
    }
}
