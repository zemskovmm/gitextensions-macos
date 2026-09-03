# Homebrew tap for Git Extensions macOS previews

This public tap installs the unsigned Apple Silicon development preview from an immutable release
of [`zemskovmm/gitextensions`](https://github.com/zemskovmm/gitextensions).

## Install

Requirements: Apple Silicon and macOS 14 Sonoma or newer.

```sh
brew install --cask zemskovmm/gitextensions/git-extensions-avalonia
```

The cask installs `Git Extensions Avalonia.app` in `/Applications`. It does not use
`--no-quarantine`. Because version 0.1.0 is not Developer ID signed or notarized, macOS may block
the first launch. Review the warning, then use **System Settings → Privacy & Security → Open
Anyway** only if you accept the risk.

## Verify

Version 0.1.0 is pinned to:

- source commit [`acb24b24290b3ff4a932d875e1b289688b8965ba`](https://github.com/zemskovmm/gitextensions/commit/acb24b24290b3ff4a932d875e1b289688b8965ba);
- immutable release [`avalonia-macos-v0.1.0`](https://github.com/zemskovmm/gitextensions/releases/tag/avalonia-macos-v0.1.0);
- archive SHA-256 `f6f24bcc7449b8413c31218638dbec3100dea1273dc53b8737945940bbf8bdbb`.

GitHub publishes release attestations for the immutable release. With GitHub CLI installed:

```sh
gh release verify avalonia-macos-v0.1.0 --repo zemskovmm/gitextensions
```

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
