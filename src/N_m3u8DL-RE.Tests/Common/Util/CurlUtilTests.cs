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
}
