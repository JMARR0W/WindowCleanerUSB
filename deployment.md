# Deployment

The intended production flow is:

1. Start normal Windows.
2. Insert deployment USB.
3. Launch `Reinstall.exe`.
4. Detect hardware and select profile.
5. Show the exact target disk and deployment configuration.
6. Require explicit confirmation during development.
7. Hand off to WinPE.
8. Re-detect and validate the target disk in WinPE.
9. Apply the Windows image.
10. Apply unattended configuration.
11. Reboot into the new installation.

The boot handoff and disk modification stages are not implemented in this initial repository.
