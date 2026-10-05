using Deployment;
using HardwareDetection;

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
Console.WriteLine($"Windows      : {hardware.WindowsVersion}");
Console.WriteLine($"Build        : {hardware.WindowsBuild}");
Console.WriteLine($"Edition      : {hardware.WindowsEdition}");
Console.WriteLine($"BIOS         : {hardware.BiosVersion}");
Console.WriteLine();

Console.WriteLine("Disks:");

foreach (var disk in hardware.Disks)
{
    Console.WriteLine($"  Disk {disk.Number}");
    Console.WriteLine($"    Model  : {disk.Model}");
    Console.WriteLine($"    Serial : {disk.SerialNumber}");
    Console.WriteLine($"    Size   : {disk.SizeGb} GB");
    Console.WriteLine($"    Bus    : {disk.BusType}");
    Console.WriteLine($"    USB    : {disk.IsUsb}");
    Console.WriteLine($"    Boot   : {disk.IsBoot}");
    Console.WriteLine($"    System : {disk.IsSystem}");
    Console.WriteLine();
}

var profile = ProfileSelector.Select(hardware);

Console.WriteLine($"Selected profile: {profile.Name}");
Console.WriteLine($"Windows edition : {profile.WindowsEdition}");
Console.WriteLine($"Disk policy     : {profile.DiskPolicy}");
Console.WriteLine();
Console.WriteLine("Deployment/boot handoff is not implemented yet.");
Console.WriteLine("No disk changes have been made.");
