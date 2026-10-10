namespace WinHubX.Impostazioni;

internal static class HardwareSensorReading
{
    private const float MaximumSensorTemperatureCelsius = 150f;

    internal static float? SelectTemperature(IEnumerable<float?> readings)
    {
        ArgumentNullException.ThrowIfNull(readings);

        foreach (float? reading in readings)
        {
            if (reading is float value && float.IsFinite(value) && value > 0f && value <= MaximumSensorTemperatureCelsius)
                return value;
        }

        return null;
    }

    internal static float? SelectPercentage(IEnumerable<float?> readings)
    {
        ArgumentNullException.ThrowIfNull(readings);

        foreach (float? reading in readings)
        {
            if (reading is float value && float.IsFinite(value) && value >= 0f)
                return Math.Min(value, 100f);
        }

        return null;
    }
}
