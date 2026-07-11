using Microsoft.Win32;

namespace N_m3u8DL_RE.Util;

internal static class UrlProtocolRegistrar
{
    private const string Scheme = "m3u8dl";

    public static bool TryHandle(string[] args)
    {
        if (!IsRegisterRequest(args))
            return false;

        RegisterCurrentExecutable();
        Console.WriteLine($"{Scheme}:// protocol registered to {Environment.ProcessPath}");
        return true;
    }

    internal static bool IsRegisterRequest(string[] args)
    {
        return args.Any(arg =>
            string.Equals(arg, "--registerUrlProtocol", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(arg, "--register-url-protocol", StringComparison.OrdinalIgnoreCase));
    }

    internal static string BuildCommandValue(string executablePath)
    {
        return $"\"{executablePath}\" \"%1\"";
    }

    private static void RegisterCurrentExecutable()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("m3u8dl:// protocol registration is only supported on Windows.");

        var executablePath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(executablePath))
            throw new InvalidOperationException("Unable to resolve current executable path.");

        using var schemeKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{Scheme}");
        if (schemeKey == null)
            throw new InvalidOperationException("Unable to create protocol registry key.");

        schemeKey.SetValue("", $"URL:{Scheme} Protocol");
        schemeKey.SetValue("URL Protocol", "");

        using var commandKey = schemeKey.CreateSubKey(@"shell\open\command");
        if (commandKey == null)
            throw new InvalidOperationException("Unable to create protocol command registry key.");

        commandKey.SetValue("", BuildCommandValue(executablePath));
    }
}
