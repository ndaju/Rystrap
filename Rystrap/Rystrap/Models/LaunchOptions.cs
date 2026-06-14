using System.Collections.ObjectModel;

namespace Rystrap.Models
{
    public class LaunchOptions
    {
        public string CustomLaunchArgs { get; set; } = "";

        public long MemoryLimitMB { get; set; } = 0;

        public GpuPreference GpuPreference { get; set; } = GpuPreference.Default;

        public bool MultiInstanceEnabled { get; set; } = true;

        public string WorkingDirectory { get; set; } = "";

        public ObservableCollection<EnvironmentVariableOverride> EnvironmentOverrides { get; set; } = new();

        public bool UseCustomLaunchArgs => !string.IsNullOrWhiteSpace(CustomLaunchArgs);

        public bool UseMemoryLimit => MemoryLimitMB > 0;

        public bool UseCustomWorkingDirectory => !string.IsNullOrWhiteSpace(WorkingDirectory);

        public LaunchOptions Clone()
        {
            var clone = new LaunchOptions
            {
                CustomLaunchArgs = CustomLaunchArgs,
                MemoryLimitMB = MemoryLimitMB,
                GpuPreference = GpuPreference,
                MultiInstanceEnabled = MultiInstanceEnabled,
                WorkingDirectory = WorkingDirectory
            };

            foreach (var envVar in EnvironmentOverrides)
                clone.EnvironmentOverrides.Add(new EnvironmentVariableOverride
                {
                    Name = envVar.Name,
                    Value = envVar.Value,
                    Enabled = envVar.Enabled
                });

            return clone;
        }
    }

    public class EnvironmentVariableOverride
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public bool Enabled { get; set; } = true;
    }

    public enum GpuPreference
    {
        Default,
        HighPerformance,
        PowerSaving,
        SpecificGpu
    }
}
