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
            "Generic",
            "TestMachine",
            "UEFI",
            false,
            "x64",
            16,
            System.Array.Empty<DiskInfo>());

        var profile = ProfileSelector.Select(hardware);

        Assert.Equal("generic-uefi", profile.Name);
        Assert.Equal("Windows 11 Pro", profile.WindowsEdition);
        Assert.Equal("internal-system-disk", profile.DiskPolicy);
    }
}
