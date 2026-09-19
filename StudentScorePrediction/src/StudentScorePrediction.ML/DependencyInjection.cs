using Microsoft.Extensions.DependencyInjection;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.ML;

public static class DependencyInjection
{
    public static IServiceCollection AddMlServices(this IServiceCollection services)
    {
        services.AddScoped<IMlService, MlService>();
        return services;
    }
}
