using N_m3u8DL_RE.Common.Util;

namespace N_m3u8DL_RE.Tests.Common.Util;

public class CurlUtilTests
{
    [Fact]
    public void FormatHeaderForCurl_LowercaseBrowserHeaders_UsesBrowserHeaderCasing()
    {
        Assert.Equal(
            "User-Agent: Mozilla/5.0",
            CurlUtil.FormatHeaderForCurl("user-agent", "Mozilla/5.0"));

        Assert.Equal(
            "Referer: https://missav.ws/",
            CurlUtil.FormatHeaderForCurl("referer", "https://missav.ws/"));
    }

    [Fact]
    public async Task CopyToFileAsync_ReportsProgressWhileWritingFile()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"curl-util-test-{Guid.NewGuid():N}.bin");
        try
        {
            await using var input = new MemoryStream([1, 2, 3, 4, 5]);
            long reported = 0;

            var written = await CurlUtil.CopyToFileAsync(input, tempFile, size => reported += size);

            Assert.Equal(5, written);
            Assert.Equal(5, reported);
            Assert.Equal([1, 2, 3, 4, 5], await File.ReadAllBytesAsync(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
