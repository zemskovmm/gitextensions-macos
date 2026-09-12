# Component inventory

This inventory covers the whole product so MVP boundaries do not accidentally hard-code dead ends. Each component is marked **MVP**, **MVP-support**, or **Later**.

## 1. Application shell

| Component | Scope | Responsibility |
|---|---|---|
| App bootstrap/lifetime | MVP-support | Single instance policy, CLI arguments, startup errors, shutdown |
| Prerequisite validation | MVP-support | Git discovery/version, repository validation, identity/config warnings |
| Window coordinator | MVP-support | Browse and commit windows, ownership, restore, multi-repo behavior |
| Native menu/commands | MVP-support | macOS roles, standard shortcuts, command enablement |
| Command palette/quick actions | Later | Searchable action invocation |
| About/help/update UI | Later | Version, notices, documentation, release channels |

## 2. Repository entry and workspace

| Component | Scope | Responsibility |
|---|---|---|
| Open-repository panel | MVP-support | Pick folder, discover worktree root, handle bare/invalid repos |
| Recent repositories | MVP-support | Open/remove/reveal recent paths without secret metadata |
| Repository context | MVP-support | Canonical paths, worktree/common-dir, Git version/config snapshot |
| Dashboard/favorites/groups | Later | Organize many repositories |
| Init/clone | Later | Create and clone repositories |
| Multi-window/workspace | Later | Multiple independent repositories and restored sessions |

## 3. Repository browser (“tree” MVP)

### 3.1 Ref/repository tree — MVP

- root/current repository node;
- local branches with current/upstream/ahead/behind state;
- remote branches grouped by remote;
- tags;
- stashes;
- submodules recursively and their status;
- linked worktrees;
- special pseudo-nodes as upstream defines them;
- lazy loading, sorting, filtering, expansion persistence;
- single/multi-selection where actions permit;
- keyboard navigation, context menu, rename/delete confirmation;
- selection drives revision filtering without race/stale updates.

### 3.2 Revision graph/list — MVP

- revision query builder and reader;
- graph lane calculation, colors, merge edges, virtual commits;
- row virtualization/incremental loading;
- columns: graph, subject, refs, author, date, abbreviated ID; optional columns deferred;
- ref labels, HEAD, tag and remote styling;
- selection by immutable object ID;
- filters by refs/path/date/author/message and clear-filter behavior;
- find/quick search/navigation history;
- loading/empty/error/cancel states;
- large-history performance and stale-result suppression;
- context actions limited to safe MVP-supported commands.

### 3.3 Commit details — MVP

- object ID, parents/children, author/committer, dates, subject/body, notes/signature state;
- ref labels and copy actions;
- changed-file list with status, path, old path, mode/submodule metadata;
- selected-file patch/diff;
- binary/large/missing object states;
- parent selection for merge commits;
- raw commit information diagnostics.

### 3.4 Browser actions

- open commit window — MVP;
- checkout local branch/ref — MVP only if needed to make the ref tree meaningfully functional and proven safe;
- create/rename/delete branch, tag, stash, worktree, submodule — Later unless adopted from the existing PR with complete safety tests;
- fetch/pull/push — Later for MVP definition, although upstream already has initial ports;
- cherry-pick/revert/reset/rebase/merge — Later.

## 4. Commit window — MVP

### 4.1 Status/index model

- porcelain-v2/NUL-safe status reader or equivalent existing parser;
- staged, unstaged, untracked, ignored-on-demand, conflicted, assume-unchanged/skip-worktree indicators;
- add/delete/rename/copy/type-change/file-mode/symlink/submodule states;
- conflict stages 1/2/3 and unresolved-state blocking;
- stable path identity using raw bytes where Git can expose names not representable losslessly;
- refresh generation and external-change detection.

### 4.2 File/status view

- staged and unstaged sections;
- tree and flat list presentation if parity requires both;
- multi-selection and tri-state aggregate selection;
- filters, sorting, grouping, path copy/reveal/open;
- selection-preserving refresh;
- keyboard and accessible labels for status—not color/icon alone.

### 4.3 Diff/patch review

- working tree ↔ index and index ↔ HEAD diff modes;
- text, binary, submodule and mode-only diffs;
- whitespace display options;
- encoding and line-ending indicators;
- large-diff cancellation/truncation with explicit state;
- hunk and line selection model;
- safe patch generation/application with context validation;
- rejected-patch recovery without corrupting index/worktree.

### 4.4 Staging mutations

- stage/unstage one, many, and all;
- partial hunk/line stage and unstage;
- intent-to-add if exposed;
- delete/reset/discard with irreversible-action confirmation and preconditions;
- add to `.gitignore`, `.git/info/exclude`, or global ignore only through an explicit destination;
- mutation serialization, index-lock handling, cancellation policy, and post-mutation refresh;
- no shell interpolation for arbitrary paths.

### 4.5 Commit message and metadata

- subject/body editor, Unicode/IME, undo/redo, paste;
- repository commit template;
- cleanup/comment-char behavior;
- message history where upstream behavior exists;
- author/committer identity discovery and actionable missing-identity error;
- amend mode loads the correct parent/message and clearly signals history rewrite;
- sign-off and author override only if explicitly selected;
- character/line guidance as guidance, not destructive rewriting;
- optional metadata autocomplete and spellchecking.

### 4.6 Commit execution

- initial and normal commit;
- amend commit;
- merge/cherry-pick/revert continuation states when repository state requires them;
- allow-empty and empty-message only when explicitly supported;
- pre-commit/prepare-commit-msg/commit-msg/post-commit hook behavior through real Git;
- GPG/SSH signing and pinentry through user Git configuration;
- deterministic progress/error output with secrets redacted;
- successful result returns new object ID, refreshes graph/status, and never silently pushes;
- failed/cancelled commit preserves message and recoverable selections.

## 5. Git/application services

| Component | Scope | Responsibility |
|---|---|---|
| Git executable locator | MVP-support | Configured path, GUI app PATH differences, version/capabilities |
| Process runner | MVP-support | Args, cwd, env, bytes, input, timeout, cancellation/process group |
| Git command factory | MVP-support | Machine-readable, injection-safe command construction |
| Repository discovery | MVP-support | Worktree root, bare repo, common dir, submodules, linked worktrees |
| Revision service | MVP | Query/log/refs/graph inputs |
| Object/diff service | MVP | Commit metadata, changed files, blobs, patches |
| Status/index service | MVP | Status snapshots and staging mutations |
| Commit service | MVP | Message/config validation and commit execution |
| Config service | MVP-support | Layered system/global/local/worktree config |
| Operation coordinator | MVP-support | Read cancellation and conservative write serialization per canonical Git common directory |
| Refresh/invalidation service | MVP-support | File watch, debounce, generation tokens, manual refresh |
| Credential/prompt broker | MVP-support | GCM/Keychain/pinentry/askpass without logging secrets |
| Long-operation progress | MVP-support | Structured progress, cancellation and output |
| Remote/branch/tag/stash/worktree/submodule services | Later | Full repository operations |

## 6. Platform adapters

- application/bundle paths and resources — MVP-support;
- process groups and termination — MVP-support;
- native menu roles and shortcuts — MVP-support;
- open/save/folder panels — MVP-support;
- clipboard and drag/drop — MVP-support;
- Keychain/credential prompts — MVP-support;
- FSEvents/filesystem watcher — MVP-support;
- browser, Finder reveal, terminal/editor/diff-tool launch — MVP-support where exposed;
- app activation, Dock badge/progress, notifications — Later;
- URL/document handlers and Finder services — Later;
- updater — Later.

## 7. State and concurrency

- `RepositoryId`/canonical path identity;
- immutable `RepositorySnapshot`, `RevisionPage`, `CommitDetails`, `StatusSnapshot`;
- generation IDs preventing old async results from replacing new state;
- selection models keyed by object ID/path identity rather than row index;
- bounded caches for avatars, commit details, blobs/diffs and graph geometry;
- mutation queue per canonical Git common directory—covering linked worktrees that share refs/object storage—and global limits for expensive Git reads;
- operation journal for diagnostics, excluding secrets and diff contents by default;
- crash recovery for commit message drafts only, with explicit privacy policy.

## 8. Security and trust

- untrusted repository-content boundary;
- argument-injection prevention and `--` separators;
- terminal escape/control-character neutralization in logs and UI;
- symlink/path traversal and case-sensitivity handling;
- hooks are code: show that Git will execute them; never execute previews separately;
- credential/askpass/pinentry isolation and redaction;
- signed-update and release integrity;
- plugin trust/isolation policy (Later);
- safe external URL and custom protocol handling;
- privacy-preserving diagnostics/telemetry off by default;
- dependency/SBOM and license notices.

## 9. Accessibility and localization

- complete keyboard traversal and standard macOS key equivalents;
- stable focus after refresh, modal close, stage/unstage, and commit errors;
- VoiceOver roles/names/values/actions for tree, graph/list, status, diff and buttons;
- non-color status/graph cues and sufficient contrast;
- reduced motion/transparency and system text scale where supported;
- screen-reader announcement for async refresh and operation results without chatter;
- retained upstream translations and translation-key parity;
- RTL layout, Unicode normalization, grapheme-safe truncation, IME/dead-key input;
- localized display but locale-independent Git parsing.

## 10. Observability and support

- structured local logs with levels and correlation IDs;
- command name/exit/timing without secret args or content;
- optional sanitized support bundle with user preview;
- crash capture policy and explicit opt-in upload;
- performance counters for first paint, revision page load, status refresh, diff render, stage, commit;
- exact app/upstream/Git/macOS/architecture versions in reports;
- reproducible fixture generator and parity evidence ledger.

## 11. Build, release, and operations

- pinned SDK and dependencies;
- restore/build/test scripts;
- macOS arm64 CI build and tests;
- optional x64/universal build matrix;
- app bundle generation and resource verification;
- code signing, hardened runtime, notarization, stapling;
- DMG/ZIP and checksums;
- SBOM, licenses, provenance/attestation;
- preview/stable channels, rollback and retention;
- Homebrew Cask after stable preview;
- updater only post-MVP.

## 12. Later full-product modules

- dashboard/repository management, clone/init;
- fetch/pull/push and remote management;
- branch/tag/stash/worktree/submodule lifecycle;
- merge/rebase/cherry-pick/revert/reset/bisect/reflog;
- conflict resolver, mergetool/difftool integrations;
- file history, blame, search/grep, patch/email workflows;
- hosting providers, pull requests, CI/build status;
- scripts and external tools;
- settings catalog, hotkey editor, themes/fonts;
- plugin discovery/contracts/UI adapters and isolation;
- shell/Finder integration;
- update service and release channels;
- analytics only if opt-in and accepted.
