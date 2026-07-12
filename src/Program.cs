using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static HtmlToDocx.Converter;

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
    // No arguments: run as a long-lived HTTP server (sidecar mode).
    // Port from HTML2DOCX_PORT (default 3030); stdin/CLI mode still available
    // via the '-' argument below.
    var port = Environment.GetEnvironmentVariable("HTML2DOCX_PORT");
    if (string.IsNullOrWhiteSpace(port))
    {
        port = "3030";
    }

    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 104857600);
    var app = builder.Build();

    app.MapGet("/health", () => Results.Ok("ok"));

    app.MapPost("/convert", async (HttpRequest request) =>
    {
        using var reader = new StreamReader(request.Body);
        var requestHtml = await reader.ReadToEndAsync();
        try
        {
            var docx = await ConvertHtmlToDocxBytesAsync(requestHtml);
            return Results.File(
                docx,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }
    });

    app.Run();
    return;
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
    byte[] docx = await ConvertHtmlToDocxBytesAsync(html);

    if (outputFile == null)
    {
        // Write to stdout
        using var stdout = Console.OpenStandardOutput();
        await stdout.WriteAsync(docx);
    }
    else
    {
        // Write to file
        await File.WriteAllBytesAsync(outputFile, docx);
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
