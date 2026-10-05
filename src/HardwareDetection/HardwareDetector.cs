using System.Management;

namespace HardwareDetection;

public sealed class HardwareDetector
{
    public HardwareInfo Detect()
    {
        return new HardwareInfo(
            Manufacturer: GetSingle("Win32_ComputerSystem", "Manufacturer"),
            Model: GetSingle("Win32_ComputerSystem", "Model"),
            FirmwareMode: DetectFirmwareMode(),
            SecureBoot: DetectSecureBoot(),
            Architecture: Environment.Is64BitOperatingSystem ? "x64" : "x86",
            MemoryGb: DetectMemoryGb(),
            Disks: DetectDisks());
    }

    private static string GetSingle(string wmiClass, string property)
    {
        using var searcher = new ManagementObjectSearcher(
            $"SELECT {property} FROM {wmiClass}");

        foreach (ManagementObject obj in searcher.Get())
            return obj[property]?.ToString()?.Trim() ?? "Unknown";

        return "Unknown";
    }

    private static string DetectFirmwareMode()
    {
        // Windows exposes this through the environment in most normal installations.
        return Environment.GetEnvironmentVariable("firmware_type")
               ?? "Unknown";
    }

    private static bool DetectSecureBoot()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                @"root\cimv2\Security\MicrosoftTpm",
                "SELECT * FROM Win32_Tpm");

            // Presence of TPM is not equivalent to Secure Boot.
            // Leave this conservative until the production detector is implemented.
            _ = searcher.Get().Count;
        }
        catch
        {
            // Ignore detection errors in the initial scaffold.
        }

        return false;
    }

    private static int DetectMemoryGb()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

        foreach (ManagementObject obj in searcher.Get())
        {
            if (ulong.TryParse(obj["TotalPhysicalMemory"]?.ToString(), out var bytes))
                return (int)Math.Round(bytes / 1024d / 1024d / 1024d);
        }

        return 0;
    }

    private static IReadOnlyList<DiskInfo> DetectDisks()
    {
        var result = new List<DiskInfo>();

        using var searcher = new ManagementObjectSearcher(
            "SELECT Index, Size, InterfaceType, PNPDeviceID FROM Win32_DiskDrive");

        foreach (ManagementObject obj in searcher.Get())
        {
            var index = Convert.ToInt32(obj["Index"]);
            var size = obj["Size"] is null ? 0L : Convert.ToInt64(obj["Size"]);
            var interfaceType = obj["InterfaceType"]?.ToString() ?? "Unknown";
            var pnp = obj["PNPDeviceID"]?.ToString() ?? "";

            result.Add(new DiskInfo(
                index,
                (long)Math.Round(size / 1024d / 1024d / 1024d),
                interfaceType,
                pnp.Contains("USB", StringComparison.OrdinalIgnoreCase)));
        }

        return result;
    }
}
