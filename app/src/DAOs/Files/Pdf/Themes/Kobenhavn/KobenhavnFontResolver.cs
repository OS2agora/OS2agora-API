using PdfSharp.Fonts;
using System.IO;

namespace Agora.DAOs.Files.Pdf.Themes.Kobenhavn
{
    public class KobenhavnFontResolver : IFontResolver
    {
        private readonly string _fontPath;

        public KobenhavnFontResolver(string fontPath)
        {
            _fontPath = fontPath;

            // Registrér skrifttyper som standard fontresolver
            GlobalFontSettings.FontResolver = this;
        }

        public byte[] GetFont(string faceName)
        {
            // Kortlæg skrifttypenavne til skrifttypefiler
            string fontFile = faceName switch
            {
                "KBH-Black" => Path.Combine(_fontPath, "KBH-Black.otf"),
                "KBH-BlackItalic" => Path.Combine(_fontPath, "KBH-BlackItalic.otf"),
                "KBH-Bold" => Path.Combine(_fontPath, "KBH-Bold.otf"),
                "KBH-Italic" => Path.Combine(_fontPath, "KBH-Italic.otf"),
                "KBH-BoldItalic" => Path.Combine(_fontPath, "KBH-BoldItalic.otf"),
                "KBH-Regular" => Path.Combine(_fontPath, "KBH-Regular.otf"),
                "KBHTekst" => Path.Combine(_fontPath, "KBHTekst.otf"),
                "KBHTekst-Bold" => Path.Combine(_fontPath, "KBHTekst-Bold.otf"),
                "KBHTekst-BoldItalic" => Path.Combine(_fontPath, "KBHTekst-BoldItalic.otf"),
                "KBHTekst-Italic" => Path.Combine(_fontPath, "KBHTekst-Italic.otf"),
                _ => throw new FileNotFoundException($"Font '{faceName}' not found.")
            };

            if (!File.Exists(fontFile))
                throw new FileNotFoundException($"Font file not found: {fontFile}");

            return File.ReadAllBytes(fontFile);
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            
            if (familyName == "KBH-Black")
            {
                return ResolveKBHBlack(isItalic);
            }

            if (familyName == "KBHTekst")
            {
                return ResolveKBHTekst(isBold, isItalic);
            }

            if (familyName == "KBH")
            {
                return ResolveKBH(isBold, isItalic);
            }

            return null;
        }

        private FontResolverInfo ResolveKBHBlack(bool isItalic) 
        {
            if (isItalic) return new FontResolverInfo("KBH-BlackItalic");
            return new FontResolverInfo("KBH-Black");
        }

        private FontResolverInfo ResolveKBHTekst(bool isBold, bool isItalic)
        {
            if (isBold && isItalic) return new FontResolverInfo("KBHTekst-BoldItalic");
            if (isBold) return new FontResolverInfo("KBHTekst-Bold");
            if (isItalic) return new FontResolverInfo("KBHTekst-Italic");
            return new FontResolverInfo("KBHTekst");
        }

        private FontResolverInfo ResolveKBH(bool isBold, bool isItalic)
        {
            if (isBold && isItalic) return new FontResolverInfo("KBH-BoldItalic");
            if (isBold) return new FontResolverInfo("KBH-Bold");
            if (isItalic) return new FontResolverInfo("KBH-Italic");
            return new FontResolverInfo("KBH-Regular");
        }
    }

}