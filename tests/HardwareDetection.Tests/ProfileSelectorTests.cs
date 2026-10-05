using Deployment;
using HardwareDetection;
using Xunit;

namespace HardwareDetection.Tests;

public class ProfileSelectorTests
{
    [Fact]
    public void SelectsUefiGenericProfile()
    {
        var hardware = new HardwareInfo(
            Manufacturer: "Generic",
            Model: "TestMachine",
            FirmwareMode: "UEFI",
            SecureBoot: SecureBootStatus.Disabled,
            Architecture: "x64",
            MemoryGb: 16,
            WindowsVersion: "Windows 11 Home",
            WindowsBuild: "24H2 (26100)",
            WindowsEdition: "Professional",
            BiosVersion: "TEST",
            Disks: System.Array.Empty<DiskInfo>());

        var profile = ProfileSelector.Select(hardware);

        Assert.Equal("generic-uefi", profile.Name);
        Assert.Equal("Windows 11 Home", profile.WindowsEdition);
        Assert.Equal("internal-system-disk", profile.DiskPolicy);
    }
}
