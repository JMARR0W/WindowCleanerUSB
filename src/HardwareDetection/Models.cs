namespace HardwareDetection;

public sealed record DiskInfo(
    int Number,
    long SizeGb,
    string BusType,
    bool IsUsb);

public sealed record HardwareInfo(
    string Manufacturer,
    string Model,
    string FirmwareMode,
    bool SecureBoot,
    string Architecture,
    int MemoryGb,
    IReadOnlyList<DiskInfo> Disks);
