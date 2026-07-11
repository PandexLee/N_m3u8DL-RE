using N_m3u8DL_RE.Util;

namespace N_m3u8DL_RE.Tests.Util;

public class UrlProtocolRegistrarTests
{
    [Fact]
    public void IsRegisterRequest_AcceptsLegacyCliOption()
    {
        Assert.True(UrlProtocolRegistrar.IsRegisterRequest(["--registerUrlProtocol"]));
    }

    [Fact]
    public void IsRegisterRequest_AcceptsKebabCaseOption()
    {
        Assert.True(UrlProtocolRegistrar.IsRegisterRequest(["--register-url-protocol"]));
    }

    [Fact]
    public void BuildCommandValue_QuotesExecutableAndProtocolArgument()
    {
        Assert.Equal(
            @"""C:\tools\N_m3u8DL-RE.exe"" ""%1""",
            UrlProtocolRegistrar.BuildCommandValue(@"C:\tools\N_m3u8DL-RE.exe"));
    }
}
