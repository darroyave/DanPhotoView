using SkiaSharp;

namespace DanPhotoView.Service;

public interface IImageService
{
    Stream CreateThumbnail(Stream inputStream, int targetWidth, int targetHeight);
}

public class ImageService : IImageService
{
    public Stream CreateThumbnail(Stream inputStream, int targetWidth, int targetHeight)
    {
        using var original = SKBitmap.Decode(inputStream);

        float widthRatio = (float)targetWidth / original.Width;
        float heightRatio = (float)targetHeight / original.Height;
        float scale = Math.Min(widthRatio, heightRatio);

        int finalWidth = (int)(original.Width * scale);
        int finalHeight = (int)(original.Height * scale);

        var resized = new SKBitmap(finalWidth, finalHeight);

        using (var canvas = new SKCanvas(resized))
        {
            var paint = new SKPaint
            {
                FilterQuality = SKFilterQuality.High
            };

            canvas.DrawBitmap(original, new SKRect(0, 0, finalWidth, finalHeight), paint);
        }

        var image = SKImage.FromBitmap(resized);

        var outputStream = new MemoryStream();

        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            data.SaveTo(outputStream);
        }

        outputStream.Position = 0;

        return outputStream;
    }
}
