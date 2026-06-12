namespace Rystrap.Updates
{
    public class UpdateInfo
    {
        public Version Version { get; set; } = null!;

        public string TagName { get; set; } = "";

        public string Name { get; set; } = "";

        public string Changelog { get; set; } = "";

        public string DownloadUrl { get; set; } = "";

        public string AssetName { get; set; } = "";

        public DateTime ReleasedAt { get; set; }

        public bool IsNewerThanCurrent => Version > Utilities.ParseVersionSafe(App.Version);

        public static UpdateInfo? FromGithubRelease(GithubRelease release)
        {
            const string LOG_IDENT = "UpdateInfo::FromGithubRelease";

            try
            {
                string versionStr = release.TagName;

                if (versionStr.StartsWith('v'))
                    versionStr = versionStr[1..];

                int plusIdx = versionStr.IndexOf('+');
                if (plusIdx != -1)
                    versionStr = versionStr[..plusIdx];

                if (!Version.TryParse(versionStr, out var version))
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to parse version from tag '{release.TagName}'");
                    return null;
                }

                var asset = release.Assets?
                    .FirstOrDefault(a => a.Name.Contains(App.ProjectName, StringComparison.OrdinalIgnoreCase)
                                     && a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

                string downloadUrl = asset?.BrowserDownloadUrl ?? "";

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    App.Logger.WriteLine(LOG_IDENT, "No suitable download asset found");
                    return null;
                }

                DateTime releasedAt = DateTime.TryParse(release.CreatedAt, out var dt) ? dt : DateTime.MinValue;

                return new UpdateInfo
                {
                    Version = version,
                    TagName = release.TagName,
                    Name = release.Name,
                    Changelog = release.Body ?? "",
                    DownloadUrl = downloadUrl,
                    AssetName = asset?.Name ?? "",
                    ReleasedAt = releasedAt
                };
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                return null;
            }
        }
    }
}
