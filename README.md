# Windows Reinstaller

A portable Windows deployment/reinstallation project.

## Goals

- Provide a user-facing Windows launcher.
- Detect hardware and firmware configuration.
- Select a deployment profile.
- Hand off to a WinPE-based deployment environment.
- Build a reproducible bootable USB.
- Keep generated Windows/WinPE artifacts out of Git.

> **Current status:** repository scaffold. Destructive disk operations and boot handoff are intentionally stubbed until they are implemented and tested.

## Repository layout

```text
src/
  Launcher/             C# launcher
  HardwareDetection/   Hardware detection library
  Deployment/           Deployment orchestration contracts

winpe/
  scripts/              WinPE-side scripts
  config/               Deployment configuration

profiles/               Hardware/deployment profiles
drivers/                Optional driver packages (not committed by default)
build/                  USB/WinPE build scripts
tests/                  Automated tests
docs/                   Architecture and development notes
```

## Prerequisites

- Windows 10/11 x64 development machine
- .NET 8 SDK
- Windows ADK
- WinPE add-on for the Windows ADK
- A Windows installation ISO for the target version
- Git

## Development

Build the solution:

```powershell
dotnet build .\WindowsReinstaller.sln
```

Run tests:

```powershell
dotnet test .\WindowsReinstaller.sln
```

Run the launcher:

```powershell
dotnet run --project .\src\Launcher\Launcher.csproj
```

## Build artifacts

Windows ISO files, generated WinPE images, generated USB contents, and compiled binaries are ignored by Git.

## Non-destructive WinPE hardware test

The first WinPE image boots into hardware inventory, writes `X:\Logs\HardwareReport.txt` in the temporary WinPE RAM drive, and pauses. It does not start deployment, change any disk, or write files to a USB drive. The report is temporary; read or photograph it before powering down.

You have already created `C:\WinPE_amd64` with `copype amd64 C:\WinPE_amd64`, so do not run `copype` again for this image. If you need a fresh workspace in the future, run that command once from **Deployment and Imaging Tools Environment** as Administrator, then run the build script below.

Then, in elevated PowerShell, build the customized image from the project directory:

```powershell
Set-Location C:\Users\JMWit\Documents\CS\WindowCleanerUSB
.\build\Build-WinPE.ps1
```

The script starts from a preserved pristine copy of the `copype` image at `C:\WinPE_amd64\boot.wim.pristine`, adds the WinPE WMI/PowerShell components, and installs `winpe\scripts\DetectHardware.ps1` as startup. It updates only `C:\WinPE_amd64\media\sources\boot.wim` and its local pristine copy.

For a test without touching a USB or the computer's internal boot configuration, create an ISO:

```cmd
MakeWinPEMedia /ISO C:\WinPE_amd64 C:\WinPE_amd64\WinPE-hardware-test.iso
```

Attach that ISO to a new Hyper-V virtual machine and boot it from the virtual DVD. The WinPE screen displays the inventory, the full report is at `X:\Logs\HardwareReport.txt`, and the command window waits at `pause`. This is the first-stage test; USB creation and Windows installation are not part of it.

The target installation configuration remains Windows 11 Home in `winpe\config\deployment.json`.

## Safety

The final deployment system is intended for machines you own or are authorized to administer. Any implementation that erases a disk must identify the target disk explicitly and require an intentional confirmation during development/testing.
