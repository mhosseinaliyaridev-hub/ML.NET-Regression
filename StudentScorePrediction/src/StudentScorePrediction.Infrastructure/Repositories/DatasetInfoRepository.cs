using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class DatasetInfoRepository : Repository<DatasetInfo>, IDatasetInfoRepository
{
    public DatasetInfoRepository(ApplicationDbContext context) : base(context)
    {
    }
}
