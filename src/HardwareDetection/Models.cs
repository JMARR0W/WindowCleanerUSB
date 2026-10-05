namespace HardwareDetection;

public enum SecureBootStatus
{
    Unknown,
    Disabled,
    Enabled
}

public sealed record DiskInfo(
    int Number,
    long SizeGb,
    string Model,
    string SerialNumber,
    string BusType,
    bool IsUsb,
    bool IsBoot,
    bool IsSystem);

public sealed record HardwareInfo(
    string Manufacturer,
    string Model,
    string FirmwareMode,
    SecureBootStatus SecureBoot,
    string Architecture,
    int MemoryGb,
    string WindowsVersion,
    string WindowsBuild,
    string WindowsEdition,
    string BiosVersion,
    IReadOnlyList<DiskInfo> Disks);
