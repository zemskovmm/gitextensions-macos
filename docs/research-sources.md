# Research source ledger

**Retrieved/inspected:** 2026-09-02. Prefer pinned source links for code claims; documentation links may evolve.

## Git Extensions first-party sources

| Source | Use |
|---|---|
| [Git Extensions repository](https://github.com/gitextensions/gitextensions) | Canonical source, GPL license, project structure |
| [Pinned main snapshot](https://github.com/gitextensions/gitextensions/tree/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1) | Windows application baseline inspected locally |
| [Cross-platform proposal #13188](https://github.com/gitextensions/gitextensions/issues/13188) | Motivation, architecture proposal, maintainer/author discussion, known macOS report |
| [Draft Avalonia port PR #13189](https://github.com/gitextensions/gitextensions/pull/13189) | Existing implementation, status, checks and scope |
| [Pinned Avalonia PR snapshot](https://github.com/gitextensions/gitextensions/tree/32ced348a2ee115a1d65960258479d3b8d6b8dd1) | Exact source exercised on macOS |
| [`Directory.Build.props` on main](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/Directory.Build.props) | Global WinForms/target framework coupling |
| [`GitUI.csproj` on main](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitUI/GitUI.csproj) | Windows-specific UI dependencies |
| [`GitCommands.csproj` on main](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitCommands/GitCommands.csproj) | Shared Git layer dependencies |
| [`IGitModule`](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitExtensions.Extensibility/Git/IGitModule.cs) | Existing Git repository contract |
| [`IGitCommandRunner`](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitExtensions.Extensibility/Git/IGitCommandRunner.cs) | Existing cancellable process boundary |
| [`IExecutable`](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitExtensions.Extensibility/IExecutable.cs) and [`IProcess`](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitExtensions.Extensibility/IProcess.cs) | Existing process lifecycle/stream contracts |
| [WinForms commit form](https://github.com/gitextensions/gitextensions/blob/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitUI/CommandsDialogs/FormCommit.cs) | Commit parity baseline |
| [WinForms revision grid](https://github.com/gitextensions/gitextensions/tree/40e2457de1a02c5da1ea88d4c8b4ee10eed601e1/src/app/GitUI/UserControls/RevisionGrid) | Tree/graph parity baseline |
| [Avalonia application project](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/src/app/GitExtensions.Avalonia/GitExtensions.Avalonia.csproj) | Cross-platform app target and macOS helper build |
| [Avalonia UI project](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/src/app/GitUI.Avalonia/GitUI.Avalonia.csproj) | Ported UI composition and dependencies |
| [Avalonia commit form](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/src/app/GitUI.Avalonia/CommandsDialogs/FormCommit.axaml.cs) | Existing commit-window implementation |
| [Avalonia repository tree](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/src/app/GitUI.Avalonia/LeftPanel/RepoObjectsTree.axaml.cs) | Existing refs-tree implementation |
| [Avalonia revision grid](https://github.com/gitextensions/gitextensions/tree/32ced348a2ee115a1d65960258479d3b8d6b8dd1/src/app/GitUI.Avalonia/UserControls/RevisionGrid) | Existing graph/list implementation |
| [macOS smoke checklist](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/eng/avalonia/macos-smoke-checklist.md) | Upstream’s intended physical-Mac validation |
| [macOS app packager](https://github.com/gitextensions/gitextensions/blob/32ced348a2ee115a1d65960258479d3b8d6b8dd1/eng/avalonia/package-macos-app.sh) | Current `.app` ZIP assembly and bundle metadata |
| [Avalonia parity tooling](https://github.com/gitextensions/gitextensions/tree/32ced348a2ee115a1d65960258479d3b8d6b8dd1/eng/avalonia) | Port map, ledger, tests and platform gates |
| [Git Extensions manual: Browse Repository](https://git-extensions-documentation.readthedocs.io/en/release-3.4/browse_repository.html) | User-facing browse/tree behavior |
| [Git Extensions manual: Commit](https://git-extensions-documentation.readthedocs.io/en/release-3.4/commit.html) | User-facing commit dialog behavior |

## UI/framework sources

| Technology | First-party source |
|---|---|
| Avalonia supported platforms | https://docs.avaloniaui.net/docs/overview/supported-platforms |
| Avalonia macOS deployment | https://docs.avaloniaui.net/docs/deployment/macOS |
| Avalonia native menus | https://docs.avaloniaui.net/docs/reference/controls/nativemenu |
| Avalonia accessibility | https://docs.avaloniaui.net/docs/app-development/accessibility |
| Avalonia headless testing | https://docs.avaloniaui.net/docs/concepts/headless/ |
| Avalonia source/license | https://github.com/AvaloniaUI/Avalonia |
| Apple SwiftUI `Table` | https://developer.apple.com/documentation/SwiftUI/Table |
| Apple SwiftUI `OutlineGroup` | https://developer.apple.com/documentation/swiftui/outlinegroup |
| AppKit `NSOutlineView` | https://developer.apple.com/documentation/appkit/nsoutlineview |
| AppKit `NSTableView` | https://developer.apple.com/documentation/appkit/nstableview |
| .NET MAUI supported platforms | https://learn.microsoft.com/dotnet/maui/supported-platforms |
| .NET MAUI Mac Catalyst | https://learn.microsoft.com/dotnet/maui/mac-catalyst/ |
| Uno Platform supported platforms | https://platform.uno/docs/articles/supported-platforms.html |
| Eto.Forms project | https://github.com/picoe/Eto |
| Qt model/view | https://doc.qt.io/qt-6/model-view-programming.html |
| Qt for macOS | https://doc.qt.io/qt-6/macos.html |
| wxWidgets | https://www.wxwidgets.org/ |
| GTK macOS | https://www.gtk.org/docs/installations/macos/ |
| Electron supported platforms | https://www.electronjs.org/docs/latest/tutorial/support |
| Electron accessibility | https://www.electronjs.org/docs/latest/tutorial/accessibility |
| Electron distribution | https://www.electronjs.org/docs/latest/tutorial/application-distribution |
| Tauri macOS distribution | https://v2.tauri.app/distribute/macos-application-bundle/ |
| Tauri DMG | https://v2.tauri.app/distribute/dmg/ |
| Flutter desktop | https://docs.flutter.dev/platform-integration/desktop |
| Flutter macOS build/release | https://docs.flutter.dev/deployment/macos |
| Compose Multiplatform desktop | https://www.jetbrains.com/help/kotlin-multiplatform-dev/compose-desktop-components.html |
| JavaFX | https://openjfx.io/openjfx-docs/ |
| React Native macOS | https://microsoft.github.io/react-native-windows/docs/rnm-getting-started |

## Git backend sources

| Source | Use |
|---|---|
| Git `status` porcelain v2 | https://git-scm.com/docs/git-status#_porcelain_format_version_2 |
| Git `diff` | https://git-scm.com/docs/git-diff |
| Git `apply` | https://git-scm.com/docs/git-apply |
| Git `commit` | https://git-scm.com/docs/git-commit |
| Git hooks | https://git-scm.com/docs/githooks |
| Git signing formats | https://git-scm.com/docs/git-config#Documentation/git-config.txt-gpgformat |
| Git credential API | https://git-scm.com/docs/gitcredentials |
| Git worktrees | https://git-scm.com/docs/git-worktree |
| Git LFS | https://github.com/git-lfs/git-lfs |
| libgit2 project/features | https://libgit2.org/ |
| LibGit2Sharp | https://github.com/libgit2/libgit2sharp |
| SwiftGit2 | https://github.com/SwiftGit2/SwiftGit2 |
| gitoxide status/missing features | https://github.com/GitoxideLabs/gitoxide |
| JGit | https://www.eclipse.org/jgit/ |
| go-git | https://github.com/go-git/go-git |
| isomorphic-git | https://isomorphic-git.org/ |
| Dulwich | https://www.dulwich.io/ |

## Apple distribution/security sources

| Source | Use |
|---|---|
| Notarizing macOS software | https://developer.apple.com/documentation/security/notarizing-macos-software-before-distribution |
| Hardened runtime | https://developer.apple.com/documentation/security/hardened-runtime |
| Code-signing workflow | https://developer.apple.com/library/archive/documentation/Security/Conceptual/CodeSigningGuide/Procedures/Procedures.html |
| App Sandbox | https://developer.apple.com/documentation/security/app-sandbox |
| Distributing outside the Mac App Store | https://developer.apple.com/documentation/xcode/distributing-your-app-for-beta-testing-and-releases |
| File-system events | https://developer.apple.com/documentation/coreservices/file_system_events |
| Keychain Services | https://developer.apple.com/documentation/security/keychain-services |
| macOS accessibility | https://developer.apple.com/accessibility/macos/ |

## Notes on evidence quality

- Source inspection and local execution are stronger than search snippets.
- PR/issue state is time-sensitive and must be re-read before execution.
- Framework documentation establishes capability, not that Git Extensions parity has been achieved.
- Licensing and name conclusions require project-owner/legal confirmation before public distribution.
