using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class ModelMetricRepository : Repository<ModelMetric>, IModelMetricRepository
{
    public ModelMetricRepository(ApplicationDbContext context) : base(context)
    {
    }
}
