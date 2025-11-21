namespace WinHubX.Impostazioni
{
    public static class MonitorSettings
    {
        public static bool PuliziaAutomaticaCPU { get; set; } = false;
        public static bool PuliziaAutomaticaRAM { get; set; } = false;
        public static decimal LimiteCPU { get; set; } = 50; // default
        public static decimal LimiteRAM { get; set; } = 40; // default
        public static bool ShowFahrenheitcpu { get; set; } = false;
        public static bool ShowFahrenheitgpu { get; set; } = false;

    }

    public class MonitoraggioConfig
    {
        public int LimiteGB { get; set; }
        public bool ShowFahrenheitcpu { get; set; } = false;
        public bool ShowFahrenheitgpu { get; set; } = false;
    }
}
