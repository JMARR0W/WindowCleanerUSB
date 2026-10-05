# Architecture

## Components

### Launcher
User-facing Windows application. Responsible for displaying detected hardware, selecting a profile, and eventually initiating the deployment boot handoff.

### HardwareDetection
Reusable library for querying firmware, hardware, disks, Windows state, and other deployment-relevant information.

### WinPE deployment
Runs outside the installed Windows environment. Responsible for deployment-time inspection and, once implemented and tested, disk preparation and Windows image application.

### Profiles
Data-driven rules for hardware families and deployment policies.

### Build
Reproducible generation of WinPE and bootable USB media.

## Safety boundary

The launcher should never perform destructive disk operations against the live Windows installation. Destructive deployment belongs in the WinPE stage, where the target disk can be explicitly identified and validated.
