using System.Collections.Concurrent;
using System.Diagnostics;
using Dgml;

if (Debugger.IsAttached)
{
    args = new string[2];
    args[0] = "D:\\source\\tests\\zharradan";
    args[1] = "out.dgml";
}

if (args.Length != 2)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  depscan <directory> <outputfilename>");
    Console.WriteLine("  e.g. depscan C:\\source output.dgml");
    return;
}

if (!Directory.Exists(args[0]))
{
    Console.WriteLine("Scan directory does not exist");
    return;
}

var extension = "csproj";
var searchTest = @"<ProjectReference Include=""";
var searchResults = SearchContentListInFiles(args[0], extension, searchTest);

var builder = new DgmlBuilder();

foreach (var searchResult in searchResults)
{
    var id = Path.GetFileName(searchResult.file);
    var projectName = Path.GetFileNameWithoutExtension(searchResult.file);
    builder.Nodes.Add(new Node(id, projectName));

    var dependency = Path.GetFileName(searchResult.content.Replace("<ProjectReference Include=\"", "").Replace("/>", "").Replace("\"", "").Trim());
    if (!String.IsNullOrEmpty(dependency))
    {
        var l = new Link(id, dependency);
        builder.Links.Add(l);
    }
}

builder.Save(args[1]);

static IEnumerable<(string file, int lineNumber, string content)> SearchContentListInFiles(string searchFolder, string extension, string searchText)
{
    var result = new BlockingCollection<(string file, int line, string content)>();

    var files = Directory.EnumerateFiles(searchFolder, $"*.{extension}", SearchOption.AllDirectories);
    Parallel.ForEach(files, (file) =>
    {
        var fileContent = File.ReadLines(file);

        var fileContentResult = fileContent.Select((line, i) => new { line, i })
              .Where(x => x.line.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
              .Select(s => new { s.i, s.line });

        foreach (var r in fileContentResult)
        {
            result.Add((file, r.i, r.line));
        }

        if (!fileContentResult.Any())
        {
            result.Add((file, 0, string.Empty));
        }
    });

    return result;
}