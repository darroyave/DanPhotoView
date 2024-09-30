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

        var aspectRatio = (float)original.Width / original.Height;

        int newWidth, newHeight;

        if (aspectRatio > 1)
        {
            newWidth = targetWidth;
            newHeight = (int)(targetWidth / aspectRatio);
        }
        else
        {
            newHeight = targetHeight;
            newWidth = (int)(targetHeight * aspectRatio);
        }

        using var resized = original
            .Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

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
