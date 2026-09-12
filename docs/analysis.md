# Port feasibility and strategy analysis

**Research cut:** 2026-09-02<br>
**Subject:** a fully usable Git Extensions experience on macOS<br>
**Upstream inspected:** `gitextensions/gitextensions` main `40e2457de1a02c5da1ea88d4c8b4ee10eed601e1`; Avalonia PR head `32ced348a2ee115a1d65960258479d3b8d6b8dd1`

## Executive conclusion

The project is feasible, but a greenfield “port” is now the wrong starting point. Git Extensions is a large Windows-first C#/WinForms application, and an active draft PR already adds a cross-platform .NET 10/Avalonia application while reusing the existing Git engine, translations, settings, hotkeys, models, graph logic, and a growing set of plugins. That draft already contains the repository browser, revision loading/graph/refs, commit details, changed files/diff, repository tree, and initial commit/network workflows—the exact center of the requested MVP.

The shortest credible route is therefore:

1. coordinate with the author and maintainers of upstream issue [#13188](https://github.com/gitextensions/gitextensions/issues/13188) and draft PR [#13189](https://github.com/gitextensions/gitextensions/pull/13189);
2. retain upstream history and carry reviewed macOS work in the single public repository;
3. stabilize the draft on real Apple-silicon hardware;
4. prove the repository-tree and commit-window behavior against both WinForms and Git CLI ground truth;
5. upstream small framework-neutral extractions and macOS fixes;
6. package, sign, notarize, and distribute a clearly labeled preview.

Do **not** build a second Git engine or translate the whole application to Swift for this goal. A native Swift product is viable only if the intent changes from “port Git Extensions” to “build a new macOS Git client inspired by it.”

## What exists upstream

The main branch remains Windows-bound at the project level:

- `Directory.Build.props` globally enables `UseWindowsForms` and targets the solution’s Windows framework.
- `GitUI` directly depends on WinForms, Windows API packages, EnvDTE, Win32 interop, ConEmu, and Windows desktop telemetry.
- The executable is a `WinExe` with Windows installer/manifest integration.

The lower layers are more reusable than the build graph suggests:

- `GitCommands` models repositories and constructs/runs real Git commands.
- `GitExtensions.Extensibility` exposes Git models and process/command interfaces.
- graph, parsing, settings, translations, hotkeys, status, diff, and commit-message logic include significant framework-neutral code;
- cross-platform branches already exist in path, process, environment, and diff-tool logic.

The draft Avalonia port adds separate `GitExtensions.Avalonia` and `GitUI.Avalonia` projects, sets `UseWindowsForms=false`, targets `net10.0`, links framework-neutral sources, ports the UI to AXAML/code-behind, introduces WinForms-shaped compatibility shims, compiles a small C process-group launcher on macOS, and includes parity inventories and headless tests. At the inspected head the PR contained 1,158 changed files and about 260,329 additions, so reviewability and long-term synchronization are major program risks.

## Strategic options

### Option A — Contribute to the upstream Avalonia port (recommended)

**Advantages**

- Maximum reuse and fastest path to the requested MVP.
- One Git behavior engine and one translation corpus.
- Existing Windows application remains stable during the transition.
- Already has browse/tree/graph/commit surfaces and parity scaffolding.
- Aligns with positive maintainer feedback requesting separate framework-neutral PRs and more macOS testing.

**Costs/risks**

- The draft is very large and currently has a failing SonarCloud quality gate.
- The primary author reported no Mac ownership; macOS validation is incomplete.
- A reported earlier macOS stack overflow and this analysis’s executable-name failure show real startup blockers.
- Keeping a long-lived port synchronized requires dedicated developers.
- Avalonia keyboard focus, mnemonics, native-menu behavior, accessibility, and dense-grid performance require explicit evidence.

### Option B — Independent C#/.NET/Avalonia derivative

Technically feasible and still reuses upstream code, but it creates duplicated maintenance, GPL distribution obligations, branding ambiguity, and a difficult merge path. Use only if upstream explicitly rejects the approach or governance makes delivery impossible. Even then, retain upstream commit history in this repository; do not copy files into an unrelated clean repository.

### Option C — Native SwiftUI/AppKit application

Best macOS integration and long-term native UX, but almost no direct C# UI/service reuse. Git semantics would need a new CLI service or a less-complete library binding. Reproducing the graph, commit dialog, settings, plugins, translations, and thousands of edge behaviors becomes a new multi-year product. Choose only for a macOS-only successor with deliberately smaller scope.

### Option D — Web-shell, Qt, Flutter, Java, or other rewrite

All can produce a desktop Git client. None offers the combination of existing C# reuse, active upstream work, and parity scaffolding. They solve a broader technology preference, not the stated port objective.

## What must be done

### 1. Establish collaboration and legal boundaries

- Ask the PR author whether macOS ownership/help is wanted and agree on branch/PR decomposition.
- Ask maintainers which portability refactors should land separately.
- Confirm whether the deliverable is an upstream preview, an unofficial fork, or a new branded product.
- Preserve GPL-3.0 notices and source availability for every distributed derivative.
- Avoid implying official status before maintainers accept the work.

### 2. Create a reproducible macOS baseline

- Pin an exact upstream PR SHA and initialize all submodules.
- Use isolated HOME/XDG settings and disposable repositories.
- Build on Apple silicon and later Intel/Rosetta if that architecture is promised.
- Capture startup, native menus, clipboard, dialogs, browser/terminal launch, drag-and-drop, Retina scaling, and crash logs.
- Convert the current manual macOS checklist into automated CI plus bounded manual evidence.

### 3. Remove startup and platform blockers

The 2026-09-02 local run found a concrete blocker: after choosing English, prerequisite settings call `AppSettings.GetGitExtensionsFullPath()`, which rejects the `GitExtensions.Avalonia` executable under fail-fast mode. Fix executable discovery as a platform/application service rather than adding another filename special case. Re-test the earlier assembly-resolve/theme recursion report from issue #13188.

Other priority adapters:

- POSIX process groups, cancellation, and terminal launching;
- Git executable discovery without relying on a GUI shell’s `PATH`;
- macOS file/folder panels and security-scoped access only if sandboxing is later adopted;
- Keychain/Git Credential Manager and pinentry flows;
- native menu roles, standard shortcuts, focus, mnemonics, clipboard formats, notifications, and default browser;
- FSEvents/file watching behavior and refresh coalescing;
- app bundle resource lookup and stable bundle identity.

### 4. Prove the tree/browser MVP

Use a generated repository matrix rather than visual inspection alone. Verify local/remote branches, tags, stashes, worktrees, submodules, symbolic refs, detached HEAD, unborn repositories, merges, octopus merges if supported, replace/grafts if displayed, large histories, non-ASCII/ref edge cases, and external changes. The revision graph, row selection, refs labels, filters, pagination/virtualization, commit details, changed files, and diff must stay synchronized.

### 5. Prove the commit-window MVP

Verify staged/unstaged/untracked/ignored/conflicted/renamed/copied/submodule/type-change states; single/multi/all staging; partial hunk and line staging; unstage/reset/delete/ignore safety; initial, normal, amend, merge, and empty-message validation; author identity; templates/history/cleanup; hooks; signing/pinentry; file modes/symlinks; weird filenames; encodings/line endings; LFS filters; cancellation; index locking; external edits; and refresh after every mutation. Git CLI output is the semantic oracle.

### 6. Make distribution a product feature

- Publish self-contained `osx-arm64` first; decide `osx-x64` support explicitly.
- Build a stable `.app` with icon, bundle identifier, version metadata, resources, and URL/file associations only when supported.
- Add hardened runtime, nested-code signing, Developer ID Application signing, notarization, stapling, DMG/ZIP packaging, and clean-machine installation tests.
- Prefer direct distribution initially. App Sandbox restrictions conflict with arbitrary repositories, external Git, hooks, credential helpers, editors, diff tools, and terminal integration.
- Add an updater only after signing identity, channel policy, rollback, and release integrity are settled.

## Licensing, name, and governance

Git Extensions declares GPL-3.0. A port that copies or links its code is a derivative and must be distributed compatibly, including corresponding source and retained notices. Avalonia’s core is MIT, but optional/commercial controls must be reviewed separately; avoid adding a paid control dependency silently.

No formal Git Extensions trademark policy was found in the inspected first-party materials. Use “Git Extensions macOS port” as a descriptive internal label. For independent public binaries, add “unofficial” and obtain maintainer guidance before using the upstream name/logo/bundle identity.

This is an engineering analysis, not legal advice.

## Delivery estimate (planning range, not a commitment)

For one experienced .NET desktop developer **building on PR #13189** with maintainer access and responsive review:

- baseline/collaboration and startup stabilization: 1–3 developer-weeks;
- repository tree/graph parity and performance: 3–6 developer-weeks;
- commit-window parity and destructive-operation safety: 4–8 developer-weeks;
- accessibility, packaging, signing, notarization, CI, and preview release: 2–5 developer-weeks.

Expected MVP order of magnitude: **10–22 developer-weeks**, with schedule risk dominated by upstream review, the size of the draft, signing credentials, accessibility gaps, and uncovered parity defects. A greenfield native rewrite is several times larger; full Git Extensions parity is a multi-release program, not an MVP.

## Decision gates

Implementation is ready only after these owner decisions:

1. upstream contribution/fork versus independent product;
2. exact meaning of “tree” (this analysis assumes refs tree + revision graph);
3. Apple silicon only for preview versus universal binary;
4. parity-first versus intentional macOS UX changes;
5. direct-download preview versus App Store aspiration;
6. permission to contact upstream and create a public GPL fork.

Until then, the ADRs remain `Proposed`.
