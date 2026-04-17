# Changelog

All notable changes to **DefenderUI** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
- Placeholder section — new changes land here before the next release tag.

---

## [1.0.0] — 2026-04-17

### Added
- 🎉 **Initial public release** of DefenderUI — a premium WinUI 3 antivirus UI concept.
- **Seven fully-featured pages**:
  - **Dashboard** — Hero protection status card, KPI row (scanned files, blocked threats, quarantined items, security score), real-time protection panel, last scan summary, update status, recent activity log, and alert InfoBars.
  - **Scan** — Four scan types (Quick, Full, Custom, USB), circular progress ring with percentage and scan-line effect, file count / elapsed / ETA display, live detected-threats list, and Pause/Stop/Continue controls.
  - **Protection** — Six protection modules (Real-time, Web, File, Ransomware, Email, Network) with per-module toggles, descriptions, and statistics.
  - **Quarantine** — Card-based threat list with risk badges, search & filter, bulk delete, and restore/delete per item.
  - **Reports** — Segmented time filter (7 / 30 / 90 days), trend charts, summary KPI cards, and threat-type distribution.
  - **Update** — Version info, update hero card, manual update with progress bar, and update history.
  - **Settings** — Eight categories (General, Protection, Notifications, Update, Privacy, Appearance, Scheduled Scans, Exclusions) with left-rail navigation.
- **Premium design system**:
  - Dark theme with Mica backdrop (native Windows 11 look & feel)
  - Custom color tokens, typography scale, and reusable card / button styles
  - Rounded corners, soft shadows, elevation cards
- **Comprehensive animation system** powered by the Windows Composition API (GPU-accelerated, 60 fps):
  - Staggered entrance animations on page navigation
  - Card hover lift with translation and shadow elevation
  - Button spring-back press/release effects
  - Ripple effect from pointer position
  - Continuous glow pulse on hero status card
  - Progress shimmer on scan / update bars
  - Vertical scan-line across the scan progress ring
  - Odometer count-up on KPI numbers
  - Smooth fade + slide page transitions
- **Full MVVM architecture** built on `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`).
- **Dependency Injection** via `Microsoft.Extensions.DependencyInjection` (services and view-models registered in `App.xaml.cs`).
- **Mock data service** (`MockDataService`) providing realistic sample data for every page — tarama, tehdit, güncelleme ve rapor verilerini simüle eder.
- **Accessibility** — `AutomationProperties` on interactive controls, keyboard navigation, and theme-resource-driven colors for proper contrast.
- **Supported architectures**: `x86`, `x64`, `ARM64`.
- **Packaging**: Unpackaged (`WindowsPackageType=None`) for easy `dotnet run` launch.

### Documentation
- `README.md` with badges, feature list, screenshots section, and getting-started guide
- `ARCHITECTURE.md` documenting project structure, pages, MVVM layers, and design tokens
- `CONTRIBUTING.md`, `SECURITY.md`, `CODE_OF_CONDUCT.md`, `CHANGELOG.md`
- GitHub issue templates (bug / feature), PR template, and issue-chooser config

---

[Unreleased]: https://github.com/caymazyusuf72/defenderui/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/caymazyusuf72/defenderui/releases/tag/v1.0.0