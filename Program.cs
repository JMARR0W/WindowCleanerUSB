using HardwareDetection;
using Deployment;

Console.WriteLine("Windows Reinstaller");
Console.WriteLine("==================");
Console.WriteLine();

var detector = new HardwareDetector();
var hardware = detector.Detect();

Console.WriteLine($"Manufacturer : {hardware.Manufacturer}");
Console.WriteLine($"Model        : {hardware.Model}");
Console.WriteLine($"Firmware     : {hardware.FirmwareMode}");
Console.WriteLine($"Secure Boot  : {hardware.SecureBoot}");
Console.WriteLine($"Architecture : {hardware.Architecture}");
Console.WriteLine($"Memory (GB)  : {hardware.MemoryGb}");
Console.WriteLine($"Disks        : {hardware.Disks.Count}");

foreach (var disk in hardware.Disks)
{
    Console.WriteLine($"  Disk {disk.Number}: {disk.SizeGb} GB, {disk.BusType}, {disk.IsUsb ? "USB" : "Internal"}");
}

var selector = new ProfileSelector();
var profile = selector.Select(hardware);

Console.WriteLine();
Console.WriteLine($"Selected profile: {profile.Name}");
Console.WriteLine();
Console.WriteLine("Deployment/boot handoff is not implemented in this initial scaffold.");
Console.WriteLine("No disk changes have been made.");
