namespace StudentScorePrediction.Domain.Enums;

public enum Gender
{
    Male,
    Female
}

public enum AlgorithmType
{
    SdcaRegression,
    FastTreeRegression,
    FastForestRegression
}

public enum TrainingStatus
{
    Queued,
    LoadingData,
    PreparingData,
    Training,
    Evaluating,
    SavingModel,
    Completed,
    Failed
}

public enum ModelStatus
{
    Active,
    Inactive,
    Archived
}
