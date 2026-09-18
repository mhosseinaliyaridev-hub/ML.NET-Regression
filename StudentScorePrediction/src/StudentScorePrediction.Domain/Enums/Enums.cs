namespace StudentScorePrediction.Domain.Enums;

public enum AlgorithmType
{
    SdcaRegression = 0,
    FastTreeRegression = 1,
    FastForestRegression = 2
}

public enum TrainingStatus
{
    Queued = 0,
    LoadingData = 1,
    PreparingData = 2,
    Training = 3,
    Evaluating = 4,
    SavingModel = 5,
    Completed = 6,
    Failed = 7
}
