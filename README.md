# Homebrew tap for Git Extensions macOS previews

This public tap installs the ad-hoc-signed Apple Silicon development preview from an immutable
release of [`zemskovmm/gitextensions`](https://github.com/zemskovmm/gitextensions).

## Install

Requirements: Apple Silicon and macOS 14 Sonoma or newer.

```sh
brew install --cask zemskovmm/gitextensions/git-extensions-avalonia
```

The cask installs `Git Extensions Avalonia.app` in `/Applications`. It does not use
`--no-quarantine`. Version 0.1.1 has a structurally valid ad-hoc bundle signature, but it is not
Developer ID signed or notarized, so macOS may block the first launch. Review the warning, then use
**System Settings → Privacy & Security → Open Anyway** only if you accept the risk.

## Verify

Version 0.1.1 is pinned to:

- source commit [`847947b065bb5d878541872fb747762523077881`](https://github.com/zemskovmm/gitextensions/commit/847947b065bb5d878541872fb747762523077881);
- immutable release [`avalonia-macos-v0.1.1`](https://github.com/zemskovmm/gitextensions/releases/tag/avalonia-macos-v0.1.1);
- archive SHA-256 `a2fa31e793da4a50b2bc877b70857f2214b4a7191c323f97c63cd6f0061444d2`.

GitHub publishes release attestations for the immutable release. With GitHub CLI installed:

```sh
gh release verify avalonia-macos-v0.1.1 --repo zemskovmm/gitextensions
```

Version 0.1.0 is superseded because its completed app bundle was not re-signed after packaging,
causing Gatekeeper to report that the app was damaged.

## Preview limitations

This package makes the current app testable by other macOS users; it is not the completed macOS
MVP. Known gaps include refs-tree correctness, accessibility, unverified mutation paths, complete
macOS integration, Developer ID signing, and notarization.

## Uninstall

```sh
brew uninstall --cask git-extensions-avalonia
```

User settings and repositories are not removed.

## Updating the cask

Publish a new immutable `avalonia-macos-vX.Y.Z` release first. Then update `version` and `sha256`
in `Casks/git-extensions-avalonia.rb` and repeat the audit, install, launch, uninstall, and clean
reinstall checks. Never replace a published release asset or reuse a version.

Git Extensions is distributed under GPL-3.0. The release links the exact corresponding source and
includes the GPL license in the application bundle.
