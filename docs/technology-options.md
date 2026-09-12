# Technology options

This is a comprehensive practical option set, not a claim that every experimental GUI toolkit ever published is listed. Options are judged against the actual goal: port Git Extensions, retain its semantics, and deliver a dense repository tree/revision graph plus a safe commit window on macOS.

## 1. Product/codebase strategies

| Strategy | Reuse | Native macOS UX | Time to MVP | Long-term risk | Verdict |
|---|---:|---:|---:|---:|---|
| Extend upstream draft PR #13189 | Very high | Medium–high | Lowest | Large draft/upstream synchronization | **Recommended** |
| Independent fork using the Avalonia branch | Very high | Medium–high | Low | Divergence and governance | Fallback only |
| New C#/Avalonia app reusing selected libraries | High | Medium–high | Medium | Duplicated composition/settings/plugins | Not justified |
| Native Swift rewrite using Git CLI | Low | Highest | High | Semantic reimplementation | New product, not a port |
| Cross-platform rewrite in another stack | Very low | Varies | Highest | Rebuild everything | Reject for stated goal |
| Wine/CrossOver wrapper around WinForms | Very high | Low | Short demo | Integration/accessibility/distribution | Prototype only |

## 2. Desktop UI frameworks

Directional scores use 1 (poor) to 5 (excellent), weighted for this port: C# reuse 30%, dense tree/grid 20%, macOS integration 15%, maturity/testing 15%, packaging 10%, runtime efficiency 10%. The score is a decision aid, not a benchmark.

| UI stack | Score | Strengths | Material drawbacks | Fit |
|---|---:|---|---|---|
| **Avalonia + C#/.NET** | **4.35** | Existing upstream port; C# reuse; desktop/window/menu APIs; custom drawing; headless tests; MIT core | Non-native controls; focus/mnemonic/accessibility parity needs proof; legacy OSS DataGrid maintenance and paid control offerings require dependency review | **Choose** |
| SwiftUI + AppKit | 3.80 | Best native menus, windows, accessibility, Keychain, file panels; `NSTableView`/`NSOutlineView` are mature | Rewrite C# UI and composition; SwiftUI alone is insufficient for the densest virtualized graph/table behaviors; macOS-only contributor pool | Best for a new native product |
| Eto.Forms + C# | 3.55 | C# and native Cocoa backend; portable API | Smaller ecosystem, less evidence for very large virtualized grids/custom graph, no existing port | Plausible fallback |
| Qt 6 (C++/QML, PySide) | 3.55 | Mature model/view, trees, custom painting, performance, accessibility, deployment tooling | No C# reuse, licensing/commercial analysis, Qt-style UX, C++/QML rewrite | Strong new-client option |
| Uno Platform + C# | 3.40 | C#, Skia desktop with AppKit shell, broad platform reach | Less direct upstream fit; non-native rendering; dense desktop parity evidence needed | Viable but inferior to existing Avalonia work |
| wxWidgets/wxOSX | 3.35 | Mature native Cocoa-oriented controls, efficient | C++ rewrite; generic-control inconsistencies; no upstream work | Viable new-client option |
| .NET MAUI / Mac Catalyst | 3.20 | C# and Microsoft stack; native Apple platform integration | Mac Catalyst is an iPad-derived model; desktop tree/grid and multi-window density are weaker; workload/tooling complexity | Reject for this desktop-heavy port |
| JavaFX | 3.20 | Mature virtualized `TreeTableView`, test tooling, macOS builds | JVM distribution; zero C# reuse; non-native feel | Viable new-client option only |
| Tauri + Rust + web UI | 3.10 | Smaller web shell than Electron; Rust sidecar/process code; DMG/updater tooling | Rebuild UI and C# services; WebView keyboard/accessibility/dense-grid complexity; IPC attack surface | Not for a parity port |
| Flutter | 2.95 | Consistent rendering, desktop integration/tests, strong tooling | Dart rewrite; non-native controls; dense tables/menus/accessibility need work | Not for this port |
| Wine/CrossOver WinForms | 2.95 | Runs much existing code | Not a true macOS app; fragile shell/credential/file-dialog integration; poor distribution and accessibility | Internal bridge only |
| Electron + TypeScript | 2.90 | Mature ecosystem, DevTools, accessibility tree, grids/editors | Chromium/Node footprint; security/IPC; C# rewrite or sidecar; non-native UX | Not justified |
| Compose Multiplatform | 2.85 | Kotlin, self-contained packages, macOS accessibility support | JVM/Skia, no C# reuse, smaller macOS desktop ecosystem | New-client option only |
| GTK 4 | 2.70 | Open source, list/column controls, accessible primitives | macOS is not its primary UX; packaging/theme/integration burden; no C# reuse | Reject |
| React Native macOS | 2.55 | JavaScript/TypeScript with native view bridge | Microsoft-community target, incomplete desktop control ecosystem, no C# reuse | Reject |
| Browser/PWA + local daemon | 2.55 | Easy delivery and UI iteration | Browser filesystem/security constraints; local service and credential complexity; not a faithful desktop app | Reject |

Other technically possible but strategically weak routes include AppKit bindings from Rust (`objc2`), Objective-C, Xamarin.Mac (superseded), Cocoa via Python/PyObjC, Go/Fyne, Go/Wails, C# with GTK#, Lazarus/FreePascal, JUCE, SDL/ImGui, and game engines. They add rewrite or platform-fit risk without a benefit over the leading choices.

### Avalonia control policy

- Prefer the controls already used by PR #13189 and custom-drawn/virtualized revision graph code.
- Pin and review every Avalonia package and license.
- Do not make a paid TreeDataGrid/Accelerate dependency part of the open-source MVP unless explicitly approved.
- Prove keyboard focus, selection, context menus, row virtualization, VoiceOver semantics, and Retina rendering with the exact selected controls.

## 3. Git backend options

| Backend | Semantic fidelity | Hooks/signing/filters/helpers | Integration | Verdict |
|---|---|---|---|---|
| **Existing Git Extensions `GitCommands` + real Git CLI** | Highest for this port | Uses the user’s actual Git behavior | C# reuse is maximal; platform coupling must be extracted | **Choose** |
| New structured Git CLI adapter | Highest | Natural support because Git executes them | Simple in any language, but discards mature upstream parsing/models | Good for greenfield Swift; unnecessary here |
| Hybrid CLI + libgit2/LibGit2Sharp for read acceleration | High if CLI remains mutation oracle | CLI covers missing semantics | Native library packaging and dual behavior paths | Consider post-MVP only after profiling |
| libgit2 + LibGit2Sharp | Good core operations | Differences/gaps around hooks, signing, credential helpers, filters and newly added Git behavior must be bridged | Strong C# API; macOS native binaries available | Not sole MVP authority |
| libgit2 + SwiftGit2 | Good core operations | Same libgit2 caveats | Swift API, but project cadence/API maturity is a risk | Not recommended |
| gitoxide/`gix` | Fast safe Rust; improving | Current own checklist still lacks push, hook execution, signed commits, merge/rebase/reset completeness | Rust FFI or sidecar | Research/performance option, not MVP |
| JGit | Mature Java implementation | Git-compatible but not identical to installed Git and helpers | Requires JVM/interop | Only for a Java UI |
| go-git | Useful pure-Go core | Not complete enough for Git Extensions parity | Go sidecar/FFI | Not recommended |
| isomorphic-git | Good browser/Node fundamentals | Limited parity for native Git workflows | Natural for web shells | Not recommended |
| Dulwich | Useful pure-Python core | Not full CLI parity | Python runtime/packaging | Not recommended |
| Custom Git implementation | Unknown | Every edge case becomes ours | Any language | Explicitly reject |

### Recommended Git process contract

- executable path and version discovery;
- repository working directory and sanitized environment;
- immutable argument vector (no shell);
- stdin mode, stdout/stderr byte streams, encoding policy;
- cancellation that sends SIGINT to the whole process group, allows a bounded grace period, then escalates without orphaning hooks;
- progress and credential/pinentry mediation;
- exit code plus redacted diagnostics;
- read/write classification and conservative mutation lock keyed by canonical Git common directory;
- refresh invalidation after mutation;
- test seam for deterministic command/result fixtures.

Use stable machine formats where Git provides them: `status --porcelain=v2 -z`, NUL-delimited paths, explicit `--format`, explicit color settings, and locale-independent environment. Never parse the human default output when a plumbing or machine-readable form exists.

## 4. Language/runtime/build choices

### Recommended

- C# 14 / .NET 10 to match the current upstream draft.
- SDK-style projects and central package management already used upstream.
- AXAML plus existing code-behind structure for reviewable 1:1 WinForms mapping.
- Small C/Objective-C or Swift helpers only where macOS process/platform APIs cannot be handled reliably in managed code.

### Alternatives

- Swift 6 + Swift Concurrency for a native rewrite.
- Rust for a Tauri/native service or future read-performance module.
- C++20/Qt or wxWidgets for a cross-platform rewrite.
- TypeScript/React for Electron/Tauri UI.
- Kotlin/Compose or Java/JavaFX.
- Dart/Flutter.

Do not mix runtimes without a measured requirement; each FFI/sidecar boundary multiplies packaging, crash, signing, and debugging work.

## 5. UI and editor building blocks

- Revision graph: retain Git Extensions graph/lane algorithms; render with Avalonia drawing primitives and cache only immutable geometry.
- Revision list: virtualized rows, stable selection by object ID, incremental loading, sortable optional columns.
- Ref tree: hierarchical collection model; lazy submodule/worktree nodes; stable expansion state; keyboard context actions.
- Changed-files/status tree: list/tree modes, grouping, multi-selection, tri-state staging state, conflict stages.
- Diff viewer: upstream AvaloniaEdit integration initially; support unified and side-by-side later, binary/image placeholders, syntax highlighting only when it does not compromise raw diff fidelity.
- Commit editor: plain-text model, commit-template and cleanup rules, metadata autocomplete, spell checking as optional behavior.
- Icons: upstream assets initially; SF Symbols only behind a macOS adapter and with redistribution rules respected.

Possible third-party components must be evaluated for license, accessibility, virtualization, IME, RTL, large-file behavior, and maintenance. Candidates include AvaloniaEdit, DiffPlex for presentation-only diff models, TextMate grammars, Reactive Extensions already used upstream, and native Quick Look for post-MVP previews.

## 6. State, settings, and persistence

Recommended MVP:

- Git repository remains the source of truth; do not mirror commits/index in a database.
- Immutable in-memory snapshots keyed by repository generation.
- Existing Git Extensions settings format where safely reusable.
- macOS application settings under Application Support (or upstream’s portable abstraction).
- Keychain/Git Credential Manager for secrets; never JSON/plist secrets.
- Small JSON schema only for macOS-specific non-secret settings/evidence if upstream format cannot represent them.

SQLite is optional post-MVP for recent-repository metadata, caches, full-text search, or operation history. It is not needed for Git state.

## 7. macOS integration technologies

- AppKit-backed Avalonia desktop lifetime and native menu integration.
- `NSOpenPanel`/`NSSavePanel` adapter for repository/file selection.
- macOS Keychain and Git Credential Manager.
- FSEvents or Avalonia/.NET filesystem watcher abstraction with debounce and rescan fallback.
- `NSWorkspace` for browser/file reveal/default-app launch.
- Pasteboard adapter for text, HTML, paths, and drag-and-drop.
- UserNotifications for long remote operations, if justified.
- Uniform Type Identifiers and document/URL handlers only after behavior exists.
- Apple Accessibility API exposure through Avalonia, verified with Accessibility Inspector and VoiceOver.
- Unified logging/signposts or structured local logs with opt-in diagnostics and redaction.

## 8. Packaging, signing, updating

Practical choices:

- `.app` bundle inside notarized ZIP: simplest preview artifact.
- DMG: familiar direct distribution; no installer privileges needed.
- PKG: only if privileged/shared installation becomes necessary.
- Homebrew Cask: useful after stable signed releases.
- Mac App Store: defer; sandbox constraints are hostile to arbitrary repositories, hooks, external Git/editor/diff tools, terminal launch, and credential helpers.

Required tools and services:

- `dotnet publish -r osx-arm64 --self-contained true` (and `osx-x64` if promised);
- deterministic app-bundle assembly;
- Developer ID Application certificate, hardened runtime, entitlements, `codesign` verification;
- `notarytool` submission and `stapler` validation;
- SBOM and third-party notices;
- GitHub Releases and signed checksums;
- updater options after MVP: Sparkle via a macOS bridge, NetSparkle, Velopack, or a small signed feed/updater. Choose only one after threat modeling and rollback design.

The currently available local identity is Apple Development, not a Developer ID distribution identity, so notarized external release is not yet unblocked.

### Git distribution policy

A standalone product should prefer a **complete, version-pinned Git distribution** inside the app, not just a copied `git` binary. The bundle must include its exec path, templates, certificates/configuration expectations, notices, corresponding-source offer obligations, and optional separately distributed tools such as Git LFS; every nested executable must be signed. Provide a validated “Use external Git…” override for advanced users.

Upstream Git Extensions may instead require compatibility with its existing configured-external-Git model. If maintainers choose that route, the macOS app must use an absolute configured path, verify version/capabilities, explain how to install/select Git, and never assume Finder supplies a shell `PATH`. This remains a Definition-of-Ready decision rather than a silent implementation default.

## 9. Testing and quality technologies

- Existing xUnit/NUnit-style upstream suites as applicable.
- Avalonia.Headless for control and rendered-state tests.
- Disposable real Git repositories for end-to-end operation tests.
- Golden graph images only for stable rendering primitives; do not use snapshots as the sole semantic proof.
- macOS Accessibility Inspector and VoiceOver manual scripts.
- Instruments for CPU, allocations, hangs, and signposts.
- `dotnet-trace`, `dotnet-counters`, crash reports, and sanitized diagnostic bundles.
- GitHub Actions macOS runners for build/unit/fixture gates; real hardware/self-hosted runner for GUI, multiple displays, Keychain/pinentry, signing, and installed-app checks.
- ShellCheck for packaging scripts, dependency/SBOM scanning, CodeQL/Sonar only when findings are triaged rather than waived wholesale.

## 10. Recommended bill of materials

| Layer | Selection |
|---|---|
| Upstream | `gitextensions/gitextensions` + current Avalonia branch/PR |
| Runtime/language | .NET 10, C# 14 |
| UI | Avalonia core/Desktop/Themes + AvaloniaEdit already selected upstream |
| Architecture | Existing composition with explicit platform ports/adapters |
| Git semantics | Existing `GitCommands` invoking real Git; reliability-first product default is a complete pinned bundled Git with a validated external-Git override, subject to the upstream decision gate |
| Concurrency | `async`/`await`, cancellation tokens, mutation coordinator keyed by canonical Git common directory |
| State | Immutable snapshots + invalidation, no Git-state database |
| Settings | Existing abstractions; macOS adapter; Keychain/GCM for secrets |
| Test | Existing suites, Avalonia.Headless, disposable repository fixtures, CLI oracle |
| Delivery | Self-contained arm64 `.app`, notarized ZIP/DMG, GitHub Releases |
| CI | GitHub Actions macOS build/test + bounded real-hardware smoke |
| Observability | Structured redacted local logs; explicit opt-in diagnostics |

## Sources

See [research-sources.md](research-sources.md) for the first-party source ledger and retrieval dates.
