using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class TrainingRunRepository : Repository<TrainingRun>, ITrainingRunRepository
{
    public TrainingRunRepository(ApplicationDbContext context) : base(context)
    {
    }
}
