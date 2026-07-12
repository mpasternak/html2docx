using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;

namespace HtmlToDocx;

/// <summary>
/// Wspólny rdzeń konwersji HTML → DOCX używany i przez tryb CLI,
/// i przez tryb serwera HTTP. Publiczny, aby był testowalny.
/// </summary>
public static class Converter
{
    public static async Task<byte[]> ConvertHtmlToDocxBytesAsync(string html)
    {
        using var memoryStream = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(
            memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        {
            var main = doc.AddMainDocumentPart();
            main.Document = new Document(new Body());

            var converter = new HtmlConverter(main);
            await converter.ParseBody(html);
            main.Document.Save();
        }

        return memoryStream.ToArray();
    }
}
