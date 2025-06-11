using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

public class SKBitmapRenderer : IBarcodeRenderer<SKBitmap>
{
    public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
    {
        // Método requerido, puedes delegarlo al otro con opciones por defecto
        return Render(matrix, format, content, new EncodingOptions());
    }

    public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
    {
        int width = matrix.Width;
        int height = matrix.Height;

        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = false,
            Style = SKPaintStyle.Fill
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (matrix[x, y])
                {
                    canvas.DrawPoint(x, y, paint);
                }
            }
        }

        return bitmap;
    }
}