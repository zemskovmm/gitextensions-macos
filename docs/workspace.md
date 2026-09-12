# Workspace operations

## One public repository

The canonical public repository is `https://github.com/zemskovmm/gitextensions-macos.git`.
It contains the application and its distribution definition in one Git history:

| Path | Role |
|---|---|
| repository root (`src/`, `eng/`, `tests/`, solution files) | Git Extensions Avalonia application source and build/test tooling |
| `Casks/git-extensions-avalonia.rb` | Homebrew Cask definition |
| `docs/` | Public product specifications, architecture, decisions, and operating guidance |

There are no application or Homebrew owner submodules in this layout. Preserve the imported
application and cask histories when completing migration work; do not introduce the prior
workspace layout again. Local recovery materials are intentionally excluded from public history.

```sh
git clone https://github.com/zemskovmm/gitextensions-macos.git
cd gitextensions-macos
git submodule update --init --recursive
```

The recursive submodule command initializes dependencies declared by the application source; it
does not create a separate application or tap repository.

## Build and focused verification

The reviewed upstream baseline is `32ced348a2ee115a1d65960258479d3b8d6b8dd1`.
The candidate application history includes the previously published Git-version repair
`355338323f06135aa928094229320185025abb39`; the icon work is committed at
`0ab619e7fa7b103356fdabaee01c4eae4cd5e058`. These identifiers establish provenance only. They
are not build, test, parity, installation, signing, or release evidence.

Use the SDK and roll-forward contract in the repository-root `global.json`. Initialize nested
externals, then build from the repository root:

```sh
git submodule update --init --recursive
dotnet restore GitExtensions.Avalonia.slnx -p:Configuration=Release \
  -p:BuildAvalonia=true -p:ShouldUnsetParentConfigurationAndPlatform=false --force-evaluate
dotnet build GitExtensions.Avalonia.slnx -c Release -m:1 --no-restore \
  -p:BuildAvalonia=true -p:ShouldUnsetParentConfigurationAndPlatform=false
```

The explicit configuration-propagation property is required by the documented verified invocation:
default solution build behavior can request unrestored `artifacts/Debug/obj` assets for projects
outside the solution. Treat this as an invocation workaround, not as evidence of a source or
build-system fix.

Tests that mutate Git must run only against disposable fixture repositories with an isolated
`HOME`/XDG environment. Do not override `GIT_AUTHOR_*` or `GIT_COMMITTER_*`: the fixtures set and
assert local identity. Do not suppress source-location startup tests when a linked-worktree or
submodule checkout exposes a `.git` file rather than a `.git` directory; run those tests in a
disposable standalone checkout at the same commit.

The focused Avalonia test package includes the existing Git-version, portal-shell, settings,
startup, commit-window, and file-viewer coverage. Run the focused test selection and
`eng/avalonia/test-package-macos-app.sh` only after a successful build, and record the exact SHA,
environment, commands, and results in the governed evidence record for that run.

## Distribution status

ADR 0004 accepts the single-public-repository layout. The tap merge and independent
public-clone verification passed. Immutable preview **0.1.2** was published from
`ee2daa42652d353df7e1fb51c18d1b34c74681f5` by the
[release workflow](https://github.com/zemskovmm/gitextensions-macos/actions/runs/34701776348).
Local Release build had zero warnings/errors; all 134 focused tests passed with
isolated HOME/XDG and an explicit English test locale. Packaging verified all
17 expected portable plugins, the official icon, GPL license and strict ad-hoc signature.
Homebrew audit/style and installation passed on macOS 26.6.2 arm64. The installed
app launched to first-run settings after owner Open Anyway approval. This proves
installation and launch, not the complete repository fixture matrix.

```sh
brew trust --cask zemskovmm/gitextensions-macos/git-extensions-avalonia
brew tap zemskovmm/gitextensions-macos https://github.com/zemskovmm/gitextensions-macos.git
brew install --cask zemskovmm/gitextensions-macos/git-extensions-avalonia
```

The macOS MVP remains unproven. In particular, refs-tree correctness, comprehensive mutation
fixtures, keyboard-only and VoiceOver behavior, performance, and signed/notarized clean-user
installation remain release gates. See [the MVP specification](mvp-specification.md).

## Public-evidence boundary

Public documentation may state reviewed, reproducible facts and link only public sources or
sanitized public evidence. Do not link local reports, screenshots, raw command output, recovery
payloads, credentials, or predecessor-ledger history from public documentation. Historical
observations remain useful only when a new same-SHA behavioral run explicitly reproduces or
supersedes them.
