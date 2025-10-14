using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System.IO;
using System.Threading.Tasks;

string html;
string outputFile;

if (args.Contains("--help"))
{
    Console.WriteLine("Usage: HtmlToDocx [input_file] [output_file]");
    Console.WriteLine("  input_file:  HTML file to convert (use '-' for stdin, default: stdin)");
    Console.WriteLine("  output_file: DOCX output file (default: stdout)");
    Console.WriteLine("  --help:      Show this help message");
    return;
}

if (args.Length == 0)
{
    // No arguments: read from stdin, write to stdout
    using var reader = new StreamReader(Console.OpenStandardInput());
    html = await reader.ReadToEndAsync();
    outputFile = null; // Will use stdout
}
else if (args.Length == 1)
{
    // One argument: input file
    if (args[0] == "-")
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        html = await reader.ReadToEndAsync();
    }
    else
    {
        html = await File.ReadAllTextAsync(args[0]);
    }
    outputFile = null; // Will use stdout
}
else if (args.Length == 2)
{
    // Two arguments: input file and output file
    if (args[0] == "-")
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        html = await reader.ReadToEndAsync();
    }
    else
    {
        html = await File.ReadAllTextAsync(args[0]);
    }
    outputFile = args[1];
}
else
{
    Console.Error.WriteLine("Error: Too many arguments. Use --help for usage information.");
    return;
}

using var memoryStream = new MemoryStream();
{
    using var doc = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
    var main = doc.AddMainDocumentPart();
    main.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
        new DocumentFormat.OpenXml.Wordprocessing.Body()
    );

    var converter = new HtmlConverter(main);
    await converter.ParseBody(html);
    main.Document.Save();
}

if (outputFile == null)
{
    // Write to stdout
    memoryStream.Position = 0;
    using var stdout = Console.OpenStandardOutput();
    await memoryStream.CopyToAsync(stdout);
}
else
{
    // Write to file
    memoryStream.Position = 0;
    using var fileStream = File.Create(outputFile);
    await memoryStream.CopyToAsync(fileStream);
}
