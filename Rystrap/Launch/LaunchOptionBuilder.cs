namespace Rystrap.Launch
{
    public static class LaunchOptionBuilder
    {
        private const string LOG_IDENT = "LaunchOptionBuilder";

        public static LaunchCommand Build(Models.LaunchOptions options, string baseLaunchArgs = "")
        {
            var command = new LaunchCommand
            {
                Arguments = baseLaunchArgs
            };

            if (options.UseCustomLaunchArgs)
            {
                command.Arguments += $" {options.CustomLaunchArgs}";
                App.Logger.WriteLine(LOG_IDENT, $"Added custom args: {options.CustomLaunchArgs}");
            }

            if (options.UseMemoryLimit)
            {
                long limitBytes = options.MemoryLimitMB * 1024 * 1024;
                command.EnvironmentVariables["ROBLOX_MEMORY_LIMIT"] = limitBytes.ToString();
                App.Logger.WriteLine(LOG_IDENT, $"Set memory limit: {options.MemoryLimitMB} MB");
            }

            switch (options.GpuPreference)
            {
                case Models.GpuPreference.HighPerformance:
                    command.EnvironmentVariables["ROBLOX_GPU_PREFERENCE"] = "high_performance";
                    App.Logger.WriteLine(LOG_IDENT, "Set GPU preference: High Performance");
                    break;

                case Models.GpuPreference.PowerSaving:
                    command.EnvironmentVariables["ROBLOX_GPU_PREFERENCE"] = "power_saving";
                    App.Logger.WriteLine(LOG_IDENT, "Set GPU preference: Power Saving");
                    break;

                case Models.GpuPreference.SpecificGpu:
                    command.Arguments += " -gpu_preference high";
                    App.Logger.WriteLine(LOG_IDENT, "Set GPU preference: Specific GPU");
                    break;
            }

            if (options.MultiInstanceEnabled)
            {
                command.Arguments += " -multi";
                App.Logger.WriteLine(LOG_IDENT, "Enabled multi-instance support");
            }

            if (options.UseCustomWorkingDirectory)
            {
                command.WorkingDirectory = options.WorkingDirectory;
                App.Logger.WriteLine(LOG_IDENT, $"Set working directory: {options.WorkingDirectory}");
            }

            foreach (var envVar in options.EnvironmentOverrides.Where(e => e.Enabled && !string.IsNullOrWhiteSpace(e.Name)))
            {
                command.EnvironmentVariables[envVar.Name] = envVar.Value;
                App.Logger.WriteLine(LOG_IDENT, $"Set env var: {envVar.Name}={envVar.Value}");
            }

            command.Arguments = command.Arguments.Trim();

            return command;
        }

        public static string BuildCommandLine(Models.LaunchOptions options, string baseLaunchArgs = "")
        {
            return Build(options, baseLaunchArgs).Arguments;
        }

        public static ProcessStartInfo BuildProcessStartInfo(Models.LaunchOptions options, string executablePath, string baseLaunchArgs = "")
        {
            var command = Build(options, baseLaunchArgs);

            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = command.Arguments,
                UseShellExecute = false
            };

            if (command.UseCustomWorkingDirectory)
                startInfo.WorkingDirectory = command.WorkingDirectory;

            foreach (var envVar in command.EnvironmentVariables)
                startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;

            return startInfo;
        }
    }

    public class LaunchCommand
    {
        public string Arguments { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new();

        public bool UseCustomWorkingDirectory => !string.IsNullOrWhiteSpace(WorkingDirectory);
    }
}
