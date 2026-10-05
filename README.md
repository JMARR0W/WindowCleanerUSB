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

## Safety

The final deployment system is intended for machines you own or are authorized to administer. Any implementation that erases a disk must identify the target disk explicitly and require an intentional confirmation during development/testing.
