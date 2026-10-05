# WindowCleanerUSB
A hands free plug and play windows reinstaller
```bash
windows-reinstaller/
├── README.md
├── LICENSE
├── .gitignore
│
├── src/
│   ├── Launcher/
│   │   ├── Launcher.csproj
│   │   ├── Program.cs
│   │   └── ...
│   │
│   ├── HardwareDetection/
│   │   ├── HardwareDetection.csproj
│   │   └── ...
│   │
│   └── Deployment/
│       └── ...
│
├── winpe/
│   ├── scripts/
│   │   ├── StartDeploy.cmd
│   │   ├── DetectHardware.ps1
│   │   ├── SelectDisk.ps1
│   │   ├── PartitionDisk.ps1
│   │   └── DeployWindows.ps1
│   │
│   └── config/
│       └── deployment.json
│
├── profiles/
│   ├── generic/
│   │   ├── profile.json
│   │   └── unattend.xml
│   ├── dell/
│   │   └── profile.json
│   ├── hp/
│   │   └── profile.json
│   └── lenovo/
│       └── profile.json
│
├── drivers/
│   ├── README.md
│   └── .gitkeep
│
├── build/
│   ├── Build.ps1
│   ├── Build-WinPE.ps1
│   ├── Build-USB.ps1
│   └── configuration.json
│
├── tests/
│   ├── HardwareDetection.Tests/
│   └── Deployment.Tests/
│
└── docs/
    ├── architecture.md
    ├── development.md
    └── deployment.md
```
