using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class PredictionRepository : Repository<Prediction>, IPredictionRepository
{
    public PredictionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
