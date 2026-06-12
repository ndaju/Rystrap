namespace Rystrap.Models.Persistable
{
    public class State
    {
        public bool PromptWebView2Install { get; set; } = true;

        public bool ForceReinstall { get; set; } = false;

        public WindowState SettingsWindow { get; set; } = new();

        public bool FirstRunCompleted { get; set; } = false;

        public string LastSelectedAccountId { get; set; } = "";
    }
}
