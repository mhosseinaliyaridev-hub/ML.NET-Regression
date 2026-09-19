using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class ModelVersionRepository : Repository<ModelVersion>, IModelVersionRepository
{
    public ModelVersionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
