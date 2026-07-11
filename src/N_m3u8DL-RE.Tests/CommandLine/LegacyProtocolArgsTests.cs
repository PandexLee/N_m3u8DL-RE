using System.Text;
using N_m3u8DL_RE.CommandLine;

namespace N_m3u8DL_RE.Tests.CommandLine;

public class LegacyProtocolArgsTests
{
    [Fact]
    public void NormalizeArgs_LegacyM3u8DlProtocol_MapsCliV3OptionsToReOptions()
    {
        const string url = "https://surrit.com/example/1080p/video.m3u8";
        const string title = "RCTD-617 title";
        const string dir = @"J:\videos\jav\";
        const string referer = "Referer:https://missav.ws/";
        const string userAgent = "User-Agent:Mozilla/5.0";
        var legacyArgs = $"\"{url}\" --saveName \"{title}\" --workDir \"{dir}\" --headers \"{referer}|{userAgent}\" --maxThreads \"48\" --minThreads \"16\" --retryCount \"100\" --timeOut \"100\" --enableDelAfterDone --noProxy";
        var protocolArg = "m3u8dl://" + Convert.ToBase64String(Encoding.UTF8.GetBytes(legacyArgs));

        var normalized = CommandInvoker.NormalizeArgs([protocolArg]);

        Assert.Equal(
            [
                url,
                "--save-name", title,
                "--save-dir", dir,
                "-H", referer,
                "-H", userAgent,
                "--thread-count", "48",
                "--download-retry-count", "100",
                "--http-request-timeout", "100",
                "--del-after-done",
                "--noProxy"
            ],
            normalized);
    }
}
