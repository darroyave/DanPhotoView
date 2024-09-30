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

        using var resized = original
            .Resize(new SKImageInfo(targetHeight, targetWidth), SKFilterQuality.None);

        var image = SKImage.FromBitmap(resized);

        var outputStream = new MemoryStream();

        using (var data = image.Encode(SKEncodedImageFormat.Png, 0))
        {
            data.SaveTo(outputStream);
        }

        outputStream.Position = 0;

        return outputStream;
    }
}
