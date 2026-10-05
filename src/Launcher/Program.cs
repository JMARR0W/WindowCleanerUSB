using HardwareDetection;
using Deployment;

var detector = new HardwareDetector();
var hardware = detector.Detect();

Console.WriteLine("Windows Reinstaller - Hardware Detection");
Console.WriteLine("----------------------------------------");
Console.WriteLine($"Manufacturer : {hardware.Manufacturer}");
Console.WriteLine($"Model        : {hardware.Model}");
Console.WriteLine($"Firmware     : {hardware.FirmwareMode}");
Console.WriteLine($"Secure Boot  : {hardware.SecureBoot}");
Console.WriteLine($"Architecture : {hardware.Architecture}");
Console.WriteLine($"Memory       : {hardware.MemoryGb} GB");
Console.WriteLine();

Console.WriteLine("Disks:");
foreach (var disk in hardware.Disks)
{
    Console.WriteLine(
        $"  Disk {disk.Number}: {disk.SizeGb} GB, Bus={disk.BusType}, USB={disk.IsUsb}");
}

var profile = ProfileSelector.Select(hardware);

Console.WriteLine();
Console.WriteLine($"Selected profile: {profile.Name}");
Console.WriteLine($"Windows edition : {profile.WindowsEdition}");
Console.WriteLine($"Disk policy     : {profile.DiskPolicy}");
Console.WriteLine();
Console.WriteLine("Deployment/boot handoff is not implemented yet.");
Console.WriteLine("No disk changes have been made.");
