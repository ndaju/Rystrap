namespace Rystrap.Updates
{
    public class UpdateDownloader : IDisposable
    {
        private const string LOG_IDENT = "UpdateDownloader";

        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cts = new();

        public event EventHandler<double>? ProgressChanged;
        public event EventHandler<string>? StatusChanged;
        public event EventHandler<Exception>? DownloadFailed;
        public event EventHandler<string>? DownloadCompleted;

        public bool IsDownloading { get; private set; }
        public string DownloadedFilePath { get; private set; } = "";

        public UpdateDownloader()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
        }

        public async Task DownloadUpdateAsync(UpdateInfo updateInfo)
        {
            if (IsDownloading)
                return;

            IsDownloading = true;
            StatusChanged?.Invoke(this, Strings.Update_Downloading);

            try
            {
                string tempDir = Path.Combine(Paths.Temp, "Updates");
                Directory.CreateDirectory(tempDir);

                string fileName = $"Rystrap-{updateInfo.TagName}.exe";
                string filePath = Path.Combine(tempDir, fileName);

                using var response = await _httpClient.GetAsync(updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, _cts.Token);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;

                using var contentStream = await response.Content.ReadAsStreamAsync(_cts.Token);
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920);

                byte[] buffer = new byte[81920];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, _cts.Token)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), _cts.Token);
                    totalRead += bytesRead;

                    double progress = totalBytes.HasValue && totalBytes > 0
                        ? (double)totalRead / totalBytes.Value * 100
                        : -1;

                    ProgressChanged?.Invoke(this, progress);
                }

                DownloadedFilePath = filePath;
                StatusChanged?.Invoke(this, Strings.Update_DownloadComplete);
                DownloadCompleted?.Invoke(this, filePath);

                App.Logger.WriteLine(LOG_IDENT, $"Download complete: {filePath}");
            }
            catch (OperationCanceledException)
            {
                App.Logger.WriteLine(LOG_IDENT, "Download was cancelled");
                StatusChanged?.Invoke(this, Strings.Update_DownloadCancelled);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                DownloadFailed?.Invoke(this, ex);
            }
            finally
            {
                IsDownloading = false;
            }
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _httpClient.Dispose();
        }
    }
}
