using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Rystrap.AppData;

namespace Rystrap.UI.ViewModels.Settings
{
    public class PerformanceViewModel : NotifyPropertyChangedViewModel
    {
        public int FpsUnlockerTarget
        {
            get => App.Settings.Prop.FpsUnlockerTarget;
            set { App.Settings.Prop.FpsUnlockerTarget = value; OnPropertyChanged(nameof(FpsUnlockerTarget)); OnPropertyChanged(nameof(FpsUnlockerDisplay)); }
        }
        public string FpsUnlockerDisplay => FpsUnlockerTarget > 0 ? $"{FpsUnlockerTarget} FPS" : "Unlimited (default)";

        public int RenderDistance
        {
            get => App.Settings.Prop.RenderDistance;
            set { App.Settings.Prop.RenderDistance = value; OnPropertyChanged(nameof(RenderDistance)); OnPropertyChanged(nameof(RenderDistanceDisplay)); }
        }
        public string RenderDistanceDisplay => RenderDistance > 0 ? $"{RenderDistance} studs" : "Default";

        public string GraphicsQualityPreset
        {
            get => App.Settings.Prop.GraphicsQualityPreset;
            set { App.Settings.Prop.GraphicsQualityPreset = value; OnPropertyChanged(nameof(GraphicsQualityPreset)); }
        }
        public string[] GraphicsQualityOptions => new[] { "Auto", "Low", "Medium", "High", "Ultra" };

        public bool DisableParticles
        {
            get => App.Settings.Prop.DisableParticles;
            set { App.Settings.Prop.DisableParticles = value; OnPropertyChanged(nameof(DisableParticles)); }
        }
        public bool DisableLighting
        {
            get => App.Settings.Prop.DisableLighting;
            set { App.Settings.Prop.DisableLighting = value; OnPropertyChanged(nameof(DisableLighting)); }
        }

        public string CpuPriorityClass
        {
            get => App.Settings.Prop.CpuPriorityClass;
            set { App.Settings.Prop.CpuPriorityClass = value; OnPropertyChanged(nameof(CpuPriorityClass)); }
        }
        public string[] CpuPriorityOptions => new[] { "Normal", "AboveNormal", "High", "Realtime" };

        public string CpuAffinityMask
        {
            get => App.Settings.Prop.CpuAffinityMask;
            set { App.Settings.Prop.CpuAffinityMask = value; OnPropertyChanged(nameof(CpuAffinityMask)); }
        }

        // Region selector
        public string PreferredServerRegion
        {
            get => App.Settings.Prop.PreferredServerRegion;
            set { App.Settings.Prop.PreferredServerRegion = value; OnPropertyChanged(nameof(PreferredServerRegion)); }
        }
        public string[] RegionOptions => new[] { "Default", "US", "EU", "Asia", "Australia" };
    }
}
