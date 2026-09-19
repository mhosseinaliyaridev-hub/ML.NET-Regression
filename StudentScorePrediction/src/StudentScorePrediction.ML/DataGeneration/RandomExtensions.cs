namespace StudentScorePrediction.ML.DataGeneration;

public static class RandomExtensions
{
    private static readonly double[] _precomputedNormals = new double[1024];
    private static int _precomputedIndex = 0;
    private static bool _hasPrecomputed = false;

    public static double NextGaussian(this Random random, double mean = 0, double stdDev = 1)
    {
        // Box-Muller transform for Gaussian distribution
        if (_hasPrecomputed)
        {
            _hasPrecomputed = false;
            return mean + stdDev * _precomputedNormals[_precomputedIndex];
        }

        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();

        double mag = stdDev * Math.Sqrt(-2.0 * Math.Log(u1));
        double z0 = mag * Math.Cos(2.0 * Math.PI * u2);
        double z1 = mag * Math.Sin(2.0 * Math.PI * u2);

        _precomputedNormals[0] = z1;
        _precomputedIndex = 0;
        _hasPrecomputed = true;

        return mean + z0;
    }

    public static float NextFloat(this Random random, float min, float max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }

    public static T NextEnum<T>(this Random random) where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(random.Next(values.Length))!;
    }
}
