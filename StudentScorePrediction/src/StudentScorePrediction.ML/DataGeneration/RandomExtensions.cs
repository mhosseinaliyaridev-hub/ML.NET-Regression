using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.ML.DataGeneration;

public static class RandomExtensions
{
    private static readonly Random _random = new();

    public static int Next(this Random random, int minValue, int maxValue)
        => random.Next(minValue, maxValue);

    public static float NextFloat(this Random random, float minValue, float maxValue)
        => (float)(random.NextDouble() * (maxValue - minValue) + minValue);

    public static T NextEnum<T>(this Random random) where T : Enum
    {
        var values = Enum.GetValues<T>();
        return values[random.Next(values.Length)];
    }

    public static float NextGaussian(this Random random, float mean, float stdDev)
    {
        var u1 = 1.0 - random.NextDouble();
        var u2 = 1.0 - random.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return (float)(mean + stdDev * randStdNormal);
    }
}
