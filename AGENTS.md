# AGENTS.md — UnlockWarning

## Project identity
- Windows-only .NET 6 WPF desktop app, target `net6.0-windows10.0.26100.0`, platform **x86**.
- Both WPF and Windows Forms are enabled (`UseWPF`, `UseWindowsForms`).
- No tests exist in this repo.

## Build & run
```powershell
dotnet restore
dotnet build UnlockWarning.csproj
```
CI publishes a single-file executable on tag pushes (`v*`):
```powershell
dotnet publish UnlockWarning.csproj -c Release -r win-x86 `
  -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true `
  -p:PublishTrimmed=false -o ./publish --self-contained true
```

## Implicit usings
The SDK auto-generates `GlobalUsings.g.cs` with these globals — do **not** add manual `using` statements for them:
- `System`, `System.Collections.Generic`, `System.Drawing`, `System.Linq`, `System.Threading`, `System.Threading.Tasks`, `System.Windows.Forms`

## Architecture notes
- **Entry point**: `App.xaml.cs` — startup, system tray icon, UAC elevation, shell context-menu registration.
- The app **requires Administrator privileges**. On non-admin launch it re-spawns itself with `runas`.
- It registers a right-click menu under `HKEY_CLASSES_ROOT\*\shell\WarningUnlockSHA256`; the `-S <filepath>` arg opens the SHA256 viewer directly.
- `-N` arg launches the main window hidden (used when re-spawning as admin).
- The app depends on a companion Windows service named **"Windows Hanging Up"** (`Utility.cs`) — started on launch, paused on password-authenticated exit.
- Password gate (`AskForPassword.xaml.cs`) uses double-MD5 with a hardcoded salt.

## Native dependencies
- `Pp.dll` and `Protect.dll` are copied to output automatically (see `.csproj`).
- `ffmpeg/` folder is declared in the project; `avcodec-61.dll`, `avformat-61.dll`, `avutil-59.dll`, `avfilter-10.dll`, and `ffmpeg.exe` live there (tracked in git).

## Code conventions
- File-scoped namespaces (`namespace Foo;`), enabled nullable, implicit usings.
- XAML + code-behind pattern. No MVVM, no DI container.
- Chinese-language UI strings and comments throughout.
- Strong-name signing via `CrawlingCrab.snk` with `PublicSign=true`.

## CI
- `.github/workflows/build.yml` triggers on `v*` tags, publishes to GitHub Releases with changelog from `CHANGELOG.md`.
