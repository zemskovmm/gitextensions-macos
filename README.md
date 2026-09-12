# Git Extensions for macOS

An Apple Silicon development preview of the upstream Git Extensions Avalonia port.
Application source, product documentation, Homebrew cask and release automation live
in this single public repository. This is **not** a completed macOS MVP or a claim
of WinForms parity.

## Install

Requires Apple Silicon and macOS 14 or newer. Preview **0.1.2** is available from
the [immutable release](https://github.com/zemskovmm/gitextensions-macos/releases/tag/avalonia-macos-v0.1.2).

```sh
brew trust --cask zemskovmm/gitextensions-macos/git-extensions-avalonia
brew tap zemskovmm/gitextensions-macos https://github.com/zemskovmm/gitextensions-macos.git
brew install --cask zemskovmm/gitextensions-macos/git-extensions-avalonia
```

The explicit tap URL is required because this repository is not named `homebrew-*`.
Recent Homebrew versions require the cask-specific trust step; older versions
without `brew trust` can omit that line. Trust only after reviewing the cask.
The app bundle is ad-hoc signed, **not Developer ID signed or notarized**. macOS may
require your explicit approval in **System Settings → Privacy & Security → Open Anyway**.
Do not disable Gatekeeper or remove quarantine globally.

## Build

Install the .NET 10 SDK, Git and Xcode Command Line Tools, then:

```sh
git clone --recurse-submodules https://github.com/zemskovmm/gitextensions-macos.git
cd gitextensions-macos
dotnet restore GitExtensions.Avalonia.slnx -p:BuildAvalonia=true -p:Configuration=Release -p:ShouldUnsetParentConfigurationAndPlatform=false
dotnet build GitExtensions.Avalonia.slnx -c Release -m:1 --no-restore -p:BuildAvalonia=true -p:ShouldUnsetParentConfigurationAndPlatform=false
bash eng/avalonia/test-package-macos-app.sh
```

Only upstream third-party dependencies remain submodules in `externals/`; no
separate owner fork or Homebrew tap repository is required.

## Project and upstream

- [Workspace operations](docs/workspace.md)
- [MVP specification and open acceptance gates](docs/mvp-specification.md)
- [Architecture](docs/architecture.md) and [roadmap](docs/roadmap.md)
- [Single-repository decision](docs/adr/0004-single-public-repository.md)
- [Upstream Avalonia port PR #13189](https://github.com/gitextensions/gitextensions/pull/13189)
  and [proposal #13188](https://github.com/gitextensions/gitextensions/issues/13188)
- [Original upstream README, contributors and sponsorship](README.upstream.md)

The reviewed upstream baseline is `32ced348a2ee115a1d65960258479d3b8d6b8dd1`.
Local prerequisites include startup repair, selected-line staging, actionable macOS
Git repair (`355338323f06135aa928094229320185025abb39`) and the official app icon
(`0ab619e7fa7b103356fdabaee01c4eae4cd5e058`). These are not evidence of full parity.
ADRs 0001 and 0002 remain proposals.

## License and provenance

Git Extensions is distributed under [GPL-3.0](LICENSE.md). Preserve source,
translations, artwork, contributors and third-party notices. Application and tap
Git histories are retained. The former private planning ledger's history is **not**
published; only reviewed current documentation is imported. Corresponding source
for each binary is identified by its exact release tag and commit.
