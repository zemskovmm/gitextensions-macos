# macOS MVP specification

## Goal

Deliver an installable Apple-silicon macOS preview in which a developer can inspect repository history through the Git Extensions tree/browser and safely create commits through a full commit window, with outcomes matching command-line Git.

## Scope definition

The word **tree** is ambiguous. This specification intentionally includes both central Git Extensions meanings:

1. **refs/repository tree:** local branches, remote branches, tags, stashes, submodules, and worktrees;
2. **revision tree/graph:** commit rows, graph lanes, ref labels, history filtering, commit details, changed files, and patch display.

A filesystem browser of every file in the checked-out directory is not a separate MVP surface; changed files are shown in commit details and the commit window.

## Personas and primary flows

### Inspect history

1. Open an existing local repository.
2. Select a ref/tree node.
3. See the matching revision graph/list without stale rows.
4. Select a commit.
5. Review metadata, parents, changed files, and a file diff.
6. Filter/search, navigate, copy IDs/paths, and refresh after external Git activity.

### Create a commit

1. Open the commit window from the repository browser.
2. Review accurate staged, unstaged, untracked, and conflicted state.
3. Inspect diffs and stage/unstage whole files or selected hunks/lines.
4. Enter or amend a commit message.
5. Run the commit through the configured real Git, including hooks/signing.
6. See actionable errors or the new commit in the graph; preserve recoverable work on failure.

## Functional acceptance criteria

### A. Startup and repository open

- Installed app launches without terminal involvement, crash, assertion, or prerequisite loop.
- The app locates a supported Git executable or explains how to select one.
- Open panel accepts normal worktrees, linked worktrees, submodule worktrees, bare repos for read-only browsing, and rejects non-repositories clearly.
- Paths containing spaces, non-ASCII characters, decomposed Unicode, leading dashes, and long components do not become command arguments or corrupt display.
- Settings/resources resolve inside the `.app` bundle.

### B. Ref tree

- Local/remote branches, tags, stashes, submodules, and worktrees match CLI-derived fixture expectations.
- Current branch, detached HEAD, upstream, ahead/behind, and dirty state are visibly and accessibly represented.
- Selection, expansion, sorting and refresh are deterministic.
- External ref changes appear after debounced refresh without resetting unrelated expansion/selection.
- Context actions are disabled unless fully implemented and safe; no decorative command silently does nothing.

### C. Revision graph/list

- Commit set, order, parents, graph edges and ref labels match the configured query and Git output.
- Handles linear, branched, merged, criss-cross where fixture supports it, shallow, replace/graft policy, unborn, and detached states without crash.
- Initial page appears responsively; further history loads without duplicate/lost rows.
- Selecting and rapidly changing refs/filters cannot let an older request overwrite the current view.
- At least 100,000 synthetic commits can be navigated with bounded memory and interactive scrolling; exact performance budgets are established during baseline rather than invented here.
- Keyboard selection, context menu, copy, search/filter, parent/child navigation and cancellation work.

### D. Commit details and diff

- Metadata, parent selection, changed paths, rename/copy/type/mode/submodule state and patch match Git ground truth.
- Text, binary, empty, deleted, very large, invalid-encoding, mixed-line-ending and no-newline cases have explicit behavior.
- Diff loading is cancellable; a cancelled/failed diff cannot replace a newer selection.
- File paths and diff text cannot inject terminal controls into logs/UI.

### E. Status and staging

- Status matches a machine-readable Git oracle for modified, added, deleted, renamed, copied, untracked, ignored-on-request, conflicted, mode, symlink, submodule and type changes.
- Stage/unstage one, many, all, hunk and line actions mutate only the selected content.
- Every mutation is followed by fresh index/worktree verification.
- Index locks, concurrent external changes and patch-context failures produce recoverable errors rather than lost changes.
- Discard/delete/reset actions show exact impact, require confirmation for destructive effects, and are tested only in fixtures.
- Refresh preserves valid file selection and never replays a mutation.

### F. Commit execution

- Normal, initial and amend commits create the expected tree, parent(s), author/committer and message.
- Repository commit templates and cleanup rules are honored.
- Missing identity, empty message, unresolved conflicts, index lock, hook rejection and signing failure are actionable and preserve the draft.
- Pre-commit, prepare-commit-msg, commit-msg and post-commit semantics come from real Git.
- GPG/SSH signing follows user Git configuration; secrets and passphrases are never captured or logged.
- Success shows the exact new object ID, clears only appropriate draft state, refreshes graph/status, and never pushes unless a separately named action is chosen after MVP.

### G. macOS behavior

- Standard app menu roles and `⌘` shortcuts work; text editing uses standard Cocoa conventions where Avalonia permits.
- No keyboard-focus traps; opening/closing menus, filters, dialogs and diffs returns focus predictably.
- Native folder/file panels, clipboard text/HTML/path formats, drag/drop, default browser and Finder reveal work where exposed.
- Retina scale and appearance changes do not hide graph lanes, blur text, misplace popups, or lose hit targets.
- App survives sleep/wake and repository volume disconnect with an explicit state.

### H. Accessibility/localization

- All primary flows complete keyboard-only.
- VoiceOver identifies tree nodes, revision rows, refs, file status, staged state, diff context and commit action.
- State is never communicated only by color.
- English is release-blocking; retained upstream translations must not regress key coverage, but linguistic QA of all locales is post-MVP.
- IME/dead keys and Unicode commit messages work.

### I. Packaging and supportability

- Self-contained arm64 app runs on the declared minimum macOS version and a clean account without SDK installation.
- Bundle has stable identifier, icon, versions and third-party notices.
- Release is Developer ID signed, hardened-runtime compatible, notarized and stapled; Gatekeeper verification passes.
- Support bundle is opt-in, previewable and redacts credentials, remote secrets, environment secrets and repository content by default.

## Required fixture matrix

Every release candidate runs against generated disposable repositories covering:

1. unborn/empty and one-commit repositories;
2. clean linear history;
3. branches, tags, remote-tracking refs, detached HEAD, ahead/behind;
4. two-parent and octopus merge graphs;
5. stash, linked worktree, nested submodule;
6. shallow clone and alternates/large object behavior where supported;
7. every staged/unstaged status class and conflict stages;
8. whole-file and partial hunk/line staging;
9. mode bit, symlink, rename/copy, binary, LFS pointer/filter behavior;
10. filenames with spaces, tabs, newlines, leading dashes, Unicode NFC/NFD and invalid UTF-8 where filesystem permits;
11. commit template, missing identity, rejecting hooks, signing success/failure/pinentry;
12. external concurrent edits/ref changes/index lock;
13. large history and large working tree performance fixtures.

For each mutation, assertions inspect the resulting Git object/index/worktree—not only UI text.

## Quality gates

1. exact SHA recorded and checkout clean;
2. submodules initialized and build clean;
3. focused and relevant upstream test suites pass;
4. fixture matrix passes on arm64 macOS;
5. no open P0/P1 data-loss, wrong-commit, credential, command-injection, startup, focus-trap, or accessibility blocker;
6. CLI/WinForms parity deviations documented and explicitly accepted;
7. installed signed/notarized smoke passes from a clean account;
8. maintainer review strategy and GPL/source distribution are satisfied.

## Non-goals

- full settings catalog;
- plugin UI compatibility;
- complete clone/fetch/pull/push/remote management;
- merge/rebase/cherry-pick/bisect and conflict-resolution UI beyond commit continuation safety;
- hosting-provider and pull-request integration;
- auto-update;
- Mac App Store;
- Intel support unless explicitly promoted into scope;
- redesigning Git Extensions behavior while port parity is still being established.

## Definition of Ready

Before implementation starts:

- owner accepts or changes the strategy ADR;
- “tree” scope is confirmed;
- upstream collaboration/public-fork permission is explicit;
- target macOS versions/architectures are explicit;
- parity versus native redesign policy is explicit;
- Git executable source policy is explicit;
- Developer ID/notarization ownership is identified;
- baseline SHA and fixture/test commands are reproducible.

## Definition of Done

The MVP is done only when every in-scope acceptance criterion is backed by reproducible evidence for one immutable release commit, and the installed artifact—not merely the build output—passes the release gates. Public documentation may cite only vetted, non-sensitive evidence.
