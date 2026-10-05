using HardwareDetection;

namespace Deployment;

public sealed record DeploymentProfile(
    string Name,
    string WindowsEdition,
    string DiskPolicy);

public sealed class ProfileSelector
{
    public DeploymentProfile Select(HardwareInfo hardware)
    {
        // Initial fallback profile.
        // Manufacturer-specific rules will be added later.
        if (string.Equals(hardware.FirmwareMode, "UEFI",
                StringComparison.OrdinalIgnoreCase))
        {
            return new DeploymentProfile(
                "generic-uefi",
                "Windows 11 Pro",
                "internal-system-disk");
        }

        return new DeploymentProfile(
            "generic",
            "Windows 11 Pro",
            "internal-system-disk");
    }
}
