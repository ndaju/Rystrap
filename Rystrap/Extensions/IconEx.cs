using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rystrap.Extensions
{
    public static class IconEx
    {
        public static Icon GetSized(this Icon icon, int width, int height) => new(icon, new System.Drawing.Size(width, height));

        public static ImageSource GetImageSource(this Icon icon, bool handleException = true)
        {
            if (handleException)
            {
                try
                {
                    return CreateHighResSource(icon);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException("IconEx::GetImageSource", ex);
                    Frontend.ShowMessageBox(string.Format(Strings.Dialog_IconLoadFailed, ex.Message));
                    return BootstrapperIcon.IconRystrap.GetIcon().GetImageSource(false);
                }
            }
            else
            {
                return CreateHighResSource(icon);
            }
        }

        private static ImageSource CreateHighResSource(Icon icon)
        {
            var memStream = new MemoryStream();
            icon.Save(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            var decoder = new IconBitmapDecoder(
                memStream,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);
            var frame = decoder.Frames.OrderByDescending(f => f.PixelWidth).First();

            var pngStream = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(frame);
            pngEncoder.Save(pngStream);
            pngStream.Seek(0, SeekOrigin.Begin);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = pngStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static ImageSource GetImageSourceFromAppResource(string uriString)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(new Uri(uriString, UriKind.RelativeOrAbsolute));
            var memStream = new MemoryStream();
            streamInfo.Stream.CopyTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            var decoder = new IconBitmapDecoder(
                memStream,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);
            var frame = decoder.Frames.OrderByDescending(f => f.PixelWidth).First();

            var pngStream = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(frame);
            pngEncoder.Save(pngStream);
            pngStream.Seek(0, SeekOrigin.Begin);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = pngStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
