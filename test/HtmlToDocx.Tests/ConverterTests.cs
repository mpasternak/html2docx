using DocumentFormat.OpenXml.Packaging;
using HtmlToDocx;
using Xunit;

namespace HtmlToDocx.Tests;

public class ConverterTests
{
    [Fact]
    public async Task Produces_NonEmpty_OpenXml_Zip()
    {
        byte[] docx = await Converter.ConvertHtmlToDocxBytesAsync("<h1>Hello World</h1>");

        Assert.NotEmpty(docx);
        // DOCX is a ZIP (OPC package): the first two bytes are the "PK" signature.
        Assert.Equal((byte)'P', docx[0]);
        Assert.Equal((byte)'K', docx[1]);
    }

    [Fact]
    public async Task Preserves_Text_Content()
    {
        byte[] docx = await Converter.ConvertHtmlToDocxBytesAsync(
            "<h1>Hello World</h1><p>A paragraph.</p>");

        using var stream = new MemoryStream(docx);
        using var doc = WordprocessingDocument.Open(stream, false);
        string text = doc.MainDocumentPart!.Document.Body!.InnerText;

        Assert.Contains("Hello World", text);
        Assert.Contains("A paragraph.", text);
    }

    [Fact]
    public async Task Empty_Input_Produces_Valid_Document()
    {
        byte[] docx = await Converter.ConvertHtmlToDocxBytesAsync(string.Empty);

        using var stream = new MemoryStream(docx);
        using var doc = WordprocessingDocument.Open(stream, false);
        Assert.NotNull(doc.MainDocumentPart!.Document.Body);
    }

    [Fact]
    public async Task Renders_Table_Markup()
    {
        byte[] docx = await Converter.ConvertHtmlToDocxBytesAsync(
            "<table><tr><td>cell 1</td><td>cell 2</td></tr></table>");

        using var stream = new MemoryStream(docx);
        using var doc = WordprocessingDocument.Open(stream, false);
        string text = doc.MainDocumentPart!.Document.Body!.InnerText;

        Assert.Contains("cell 1", text);
        Assert.Contains("cell 2", text);
    }
}
