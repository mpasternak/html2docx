using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

string html;
string outputFile;
bool showStacktrace = false;

// Parse --stacktrace flag
var filteredArgs = new List<string>();
foreach (var arg in args)
{
    if (arg == "--stacktrace")
    {
        showStacktrace = true;
    }
    else
    {
        filteredArgs.Add(arg);
    }
}
args = filteredArgs.ToArray();

if (args.Contains("--help"))
{
    Console.WriteLine("Usage: HtmlToDocx [input_file] [output_file] [options]");
    Console.WriteLine("  input_file:   HTML file to convert (use '-' for stdin, default: stdin)");
    Console.WriteLine("  output_file:  DOCX output file (default: stdout)");
    Console.WriteLine("  --help:       Show this help message");
    Console.WriteLine("  --stacktrace: Show detailed stack trace on errors");
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

try
{
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
}
catch (IOException ex) when (ex.Message.Contains("being used") || ex.Message.Contains("locked") || ex.Message.Contains("access"))
{
    Console.Error.WriteLine();



    Console.Error.WriteLine("ERROR: Cannot write to output file.");
    Console.Error.WriteLine("This error is likely caused by the file being open in another application,");
    Console.Error.WriteLine("such as Microsoft Word, LibreOffice Writer, or another word processor.");
    Console.Error.WriteLine();
    Console.Error.WriteLine("To fix this issue:");
    Console.Error.WriteLine("1. Close the document in any open applications");
    Console.Error.WriteLine("2. Try running the command again");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Error details:");
    Console.Error.WriteLine(ex.Message);
    
    if (showStacktrace)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("Stack trace:");
        Console.Error.WriteLine(ex.StackTrace);
    }



    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.Error.WriteLine();



    Console.Error.WriteLine("ERROR: An unexpected error occurred while converting HTML to DOCX.");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Error details:");
    Console.Error.WriteLine(ex.Message);
    
    if (showStacktrace)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("Stack trace:");
        Console.Error.WriteLine(ex.StackTrace);
    }



    Environment.Exit(1);
}
