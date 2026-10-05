using Deployment;
using HardwareDetection;

namespace HardwareDetection.Tests;

public class ProfileSelectorTests
{
    [Fact]
    public void SelectsGenericUefiProfileForUefiX64()
    {
        var hardware = new HardwareInfo(
            "Test Vendor",
            "Test Model",
            "UEFI",
            false,
            "x64",
            16,
            Array.Empty<DiskInfo>());

        var profile = new ProfileSelector().Select(hardware);

        Assert.Equal("generic-uefi", profile.Name);
        Assert.Equal("Windows 11 Pro", profile.WindowsEdition);
    }
}
