using Microsoft.Win32;
using System.Management;
using System.Runtime.InteropServices;

namespace HardwareDetection;

public sealed class HardwareDetector
{
    private enum FirmwareType
    {
        Unknown = 0,
        Bios = 1,
        Uefi = 2,
        Max = 3
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFirmwareType(out FirmwareType firmwareType);

    public HardwareInfo Detect()
    {
        using var computer = new ManagementObjectSearcher(
            "SELECT Manufacturer, Model, TotalPhysicalMemory FROM Win32_ComputerSystem");

        var system = computer.Get().Cast<ManagementObject>().FirstOrDefault();

        var manufacturer = system?["Manufacturer"]?.ToString()?.Trim() ?? "Unknown";
        var model = system?["Model"]?.ToString()?.Trim() ?? "Unknown";

        return new HardwareInfo(
            manufacturer,
            model,
            DetectFirmwareMode(),
            DetectSecureBoot(),
            Environment.Is64BitOperatingSystem ? "x64" : "x86",
            DetectMemoryGb(system),
            DetectWindowsVersion(),
            DetectWindowsBuild(),
            DetectWindowsEdition(),
            DetectBiosVersion(),
            DetectDisks());
    }

    private static string DetectFirmwareMode()
    {
        if (!GetFirmwareType(out var firmwareType))
            return "Unknown";

        return firmwareType switch
        {
            FirmwareType.Uefi => "UEFI",
            FirmwareType.Bios => "Legacy BIOS",
            _ => "Unknown"
        };
    }

    private static SecureBootStatus DetectSecureBoot()
    {
        if (!OperatingSystem.IsWindows())
            return SecureBootStatus.Unknown;

        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Control\SecureBoot\State");

            var value = key?.GetValue("UEFISecureBootEnabled");

            if (value is int intValue)
                return intValue == 1
                    ? SecureBootStatus.Enabled
                    : SecureBootStatus.Disabled;

            if (value is long longValue)
                return longValue == 1
                    ? SecureBootStatus.Enabled
                    : SecureBootStatus.Disabled;
        }
        catch
        {
            // Leave status as Unknown if the firmware state cannot be read.
        }

        return SecureBootStatus.Unknown;
    }

    private static int DetectMemoryGb(ManagementObject? system)
    {
        if (system?["TotalPhysicalMemory"] is not ulong bytes)
            return 0;

        return (int)Math.Round(bytes / 1024d / 1024d / 1024d);
    }

    private static string DetectWindowsVersion()
    {
        using var key = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        var productName = key?.GetValue("ProductName")?.ToString() ?? "Unknown";
        var buildString = key?.GetValue("CurrentBuild")?.ToString();

        if (int.TryParse(buildString, out var build))
        {
            // Windows 11 uses build 22000 and later.
            if (build >= 22000)
            {
                return productName
                    .Replace("Windows 10", "Windows 11",
                        StringComparison.OrdinalIgnoreCase);
            }
        }

        return productName;
    }

    private static string DetectWindowsBuild()
    {
        using var key = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        var displayVersion = key?.GetValue("DisplayVersion")?.ToString();

        var currentBuild =
            key?.GetValue("CurrentBuildNumber")?.ToString()
            ?? key?.GetValue("CurrentBuild")?.ToString();

        if (!string.IsNullOrWhiteSpace(displayVersion) &&
            !string.IsNullOrWhiteSpace(currentBuild))
        {
            return $"{displayVersion} ({currentBuild})";
        }

        return currentBuild ?? "Unknown";
    }

    private static string DetectWindowsEdition()
    {
        using var key = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

        return key?.GetValue("EditionID")?.ToString() ?? "Unknown";
    }

    private static string DetectBiosVersion()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT SMBIOSBIOSVersion FROM Win32_BIOS");

        var bios = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

        return bios?["SMBIOSBIOSVersion"]?.ToString()?.Trim() ?? "Unknown";
    }

    private static IReadOnlyList<DiskInfo> DetectDisks()
    {
        var disks = new List<DiskInfo>();

        using var searcher = new ManagementObjectSearcher(
            "SELECT Index, Size, Model, SerialNumber, InterfaceType, PNPDeviceID FROM Win32_DiskDrive");

        foreach (ManagementObject disk in searcher.Get())
        {
            var number = Convert.ToInt32(disk["Index"] ?? -1);

            var sizeBytes = disk["Size"] is ulong size
                ? size
                : 0UL;

            var sizeGb = sizeBytes == 0
                ? 0
                : (long)Math.Round(sizeBytes / 1024d / 1024d / 1024d);

            var model = disk["Model"]?.ToString()?.Trim() ?? "Unknown";
            var serial = disk["SerialNumber"]?.ToString()?.Trim() ?? "Unknown";
            var interfaceType = disk["InterfaceType"]?.ToString()?.Trim() ?? "Unknown";
            var pnpId = disk["PNPDeviceID"]?.ToString() ?? string.Empty;

            var isUsb =
                interfaceType.Equals("USB", StringComparison.OrdinalIgnoreCase) ||
                pnpId.Contains("USB", StringComparison.OrdinalIgnoreCase);

            disks.Add(new DiskInfo(
                number,
                sizeGb,
                model,
                serial,
                interfaceType,
                isUsb,
                IsBootDisk(number),
                IsSystemDisk(number)));
        }

        return disks;
    }

    private static bool IsBootDisk(int diskNumber)
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT DiskIndex FROM Win32_DiskPartition WHERE BootPartition = TRUE");

        foreach (ManagementObject partition in searcher.Get())
        {
            if (partition["DiskIndex"] is uint index &&
                index == diskNumber)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSystemDisk(int diskNumber)
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT DiskIndex FROM Win32_DiskPartition WHERE BootPartition = TRUE");

        foreach (ManagementObject partition in searcher.Get())
        {
            if (partition["DiskIndex"] is uint index &&
                index == diskNumber)
            {
                return true;
            }
        }

        return false;
    }
}
