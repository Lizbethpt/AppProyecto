using PdfSharpCore.Fonts;
using System.Reflection;
using System;
using System.IO;
public class EmbeddedFontResolver : IFontResolver
{
    public string DefaultFontName => "Arial";
    public byte[] GetFont(string faceName)
    {
        var name = faceName.Split('#')[0];
        var resourcePath = $"AppVE.Resources.Fonts.{name}.ttf";
        using Stream stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream(resourcePath);
        if (stream == null)
            throw new FileNotFoundException(
                $"Fuente incrustada no encontrada en: {resourcePath}");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            string suffix = isBold ? "b" : "";
            return new FontResolverInfo("Arial" + (suffix.Length > 0 ? $"#{suffix}" : ""));
        }
        return new FontResolverInfo(DefaultFontName);
    }
}