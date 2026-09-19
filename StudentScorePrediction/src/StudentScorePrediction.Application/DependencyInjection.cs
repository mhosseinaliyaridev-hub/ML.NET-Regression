using Microsoft.Extensions.DependencyInjection;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Application.Services;

namespace StudentScorePrediction.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, Infrastructure.UnitOfWork>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IPredictionService, PredictionService>();
        services.AddScoped<ITrainingService, TrainingService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<IDatasetService, DatasetService>();
        
        return services;
    }
}
