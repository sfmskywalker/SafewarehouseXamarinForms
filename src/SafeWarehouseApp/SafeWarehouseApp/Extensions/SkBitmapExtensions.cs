using SkiaSharp;

namespace SafeWarehouseApp.Extensions
{
    public static class SkBitmapExtensions
    {
        public static SKBitmap Resize(this SKBitmap bitmap, int maxWidth)
        {
            if (bitmap.Width <= maxWidth)
                return bitmap;
            
            var scale = maxWidth / (float)bitmap.Width;
            return bitmap.Resize(new SKSizeI(maxWidth, (int) (bitmap.Height * scale)), SKFilterQuality.High);
        }
    }
}