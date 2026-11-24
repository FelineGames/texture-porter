using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TexterRename.Core;

public static class TextureMapper
{
    public static Dictionary<string, string>? LoadDefaultMapping()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string? resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(AppConfig.MappingFilename, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return null;

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        string[] lines = reader.ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        return ParseRules(lines);
    }

    public static Dictionary<string, string>? LoadMapping(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return null;

        string path = Path.Combine(folder, AppConfig.MappingFilename);
        if (!File.Exists(path))
            return null;

        try
        {
            return ParseRules(File.ReadLines(path));
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, string> ParseRules(IEnumerable<string> lines)
    {
        var rules = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string line in lines)
        {
            int eqIndex = line.IndexOf('=');
            if (eqIndex >= 0)
            {
                string src = line[..eqIndex].Trim();
                string output = line[(eqIndex + 1)..].Trim();
                if (!string.IsNullOrEmpty(src))
                {
                    rules[src] = output;
                }
            }
        }
        return rules;
    }

    public static int ProcessFiles(string source, string output, Dictionary<string, string> rules)
    {
        int count = 0;
        foreach (string filePath in Directory.EnumerateFiles(source))
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                rules.TryGetValue(fileName, out string? mappedName))
            {
                string outFile = Path.Combine(output, mappedName);
                string? outDir = Path.GetDirectoryName(outFile);
                if (!string.IsNullOrEmpty(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                File.Copy(filePath, outFile, overwrite: true);
                count++;
            }
        }
        return count;
    }
}
