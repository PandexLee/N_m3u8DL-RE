using System.Diagnostics;
using N_m3u8DL_RE.Common.Log;

namespace N_m3u8DL_RE.Common.Util;

public static class CurlUtil
{
    public static bool ShouldUseCurlFallback(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && uri.Host.EndsWith("surrit.com", StringComparison.OrdinalIgnoreCase);
    }

    public static async Task<byte[]> GetBytesAsync(string url, Dictionary<string, string>? headers, CancellationToken cancellationToken = default)
    {
        var temp = Path.Combine(Path.GetTempPath(), $"nm3u8dl-curl-{Guid.NewGuid():N}.tmp");
        try
        {
            await DownloadToFileAsync(url, temp, headers, null, null, cancellationToken);
            return await File.ReadAllBytesAsync(temp, cancellationToken);
        }
        finally
        {
            TryDelete(temp);
        }
    }

    public static async Task DownloadToFileAsync(
        string url,
        string path,
        Dictionary<string, string>? headers,
        long? fromPosition = null,
        long? toPosition = null,
        CancellationToken cancellationToken = default)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        Logger.Debug($"curl fallback => {url}");

        using var process = new Process();
        process.StartInfo.FileName = "curl.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        var args = process.StartInfo.ArgumentList;
        args.Add("--fail");
        args.Add("--location");
        args.Add("--silent");
        args.Add("--show-error");
        args.Add("--compressed");
        args.Add("--output");
        args.Add(path);

        if (fromPosition != null || toPosition != null)
        {
            args.Add("--range");
            args.Add($"{fromPosition ?? 0}-{toPosition?.ToString() ?? ""}");
        }

        if (headers != null)
        {
            foreach (var header in headers)
            {
                args.Add("-H");
                args.Add(FormatHeaderForCurl(header.Key, header.Value));
            }
        }

        args.Add(url);

        process.Start();
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            TryDelete(path);
            throw new HttpRequestException($"curl fallback failed with exit code {process.ExitCode}: {stderr}".Trim());
        }
    }

    internal static string FormatHeaderForCurl(string key, string value)
    {
        return $"{NormalizeHeaderNameForCurl(key)}: {value}";
    }

    private static string NormalizeHeaderNameForCurl(string key)
    {
        return key.Trim().ToLowerInvariant() switch
        {
            "user-agent" => "User-Agent",
            "referer" => "Referer",
            "origin" => "Origin",
            "accept" => "Accept",
            "accept-language" => "Accept-Language",
            "accept-encoding" => "Accept-Encoding",
            "cache-control" => "Cache-Control",
            "cookie" => "Cookie",
            "authorization" => "Authorization",
            _ => key.Trim()
        };
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup only.
        }
    }
}
