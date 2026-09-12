# Delivery roadmap

## Program goal

Ship a dependable macOS preview of Git Extensions whose repository tree/revision graph and commit window are behaviorally safe and useful.

## Workspace status — 2026-09-12

The application source, documentation and Homebrew cask share one public repository; see
[workspace operations](workspace.md) and accepted [ADR 0004](adr/0004-single-public-repository.md).
Only upstream third-party dependencies remain submodules. Application and tap histories are
preserved; the former private ledger history is retained locally, not published.
The source includes startup, selected-line staging, actionable Git repair and official-icon fixes.
The old preview download disappeared with its predecessor repository; a new versioned preview
must pass fresh build, package, download, installation and launch checks before being called usable.
The preview is ad-hoc signed, not Developer ID signed or notarized.
These slices do not complete any broader MVP phase: refs-tree correctness, accessibility,
comprehensive mutation behavior, and clean-user signed/notarized distribution remain gated.

## Phase 0 — decision and upstream handshake

**Outcome:** one accepted implementation route and review shape.

- Confirm the MVP interpretation and non-goals.
- Discuss ownership with draft PR #13189’s author and Git Extensions maintainers.
- Decide upstream contribution versus independent fork, public naming, and logo use.
- Agree which portability extractions land as separate PRs.
- Confirm Apple-silicon/minimum-macOS policy and direct distribution.
- Identify Developer ID/notarization owner.

**Exit:** ADR 0001/0002 accepted or replaced; a public implementation repository/branch exists only with explicit approval.

## Phase 1 — reproducible macOS baseline

**Outcome:** one pinned SHA builds/tests/launches repeatably on real hardware.

- Fork/fetch exact upstream draft head with submodules.
- Pin .NET SDK and document prerequisites.
- Automate isolated HOME/XDG/disposable-repository setup.
- Reproduce and fix the executable-name/resource-path startup blocker.
- Re-test the reported assembly-resolve/theme recursion stack overflow.
- Add macOS CI build and focused test gate.
- Capture failures as small upstream issues/PRs rather than burying fixes in the huge draft.

**Exit:** app reaches repository browser under fail-fast mode; no prerequisite/startup crash.

## Phase 2 — repository tree and revision graph

**Outcome:** complete, responsive history-inspection workflow.

Work packages:

1. repository/ref tree data parity;
2. ref selection and filter synchronization;
3. revision loading, graph lanes, ref labels and virtualization;
4. commit details and changed files;
5. diff viewer correctness/cancellation;
6. external-change refresh and stale-result suppression;
7. keyboard/focus/VoiceOver behavior;
8. large-history performance baseline.

**Exit:** all browser/tree acceptance criteria and fixtures pass on arm64 macOS.

## Phase 3 — commit window

**Outcome:** safe end-to-end commit creation.

Work packages:

1. status model and all path/status classes;
2. stage/unstage whole-file operations;
3. hunk/line staging and rejected-patch recovery;
4. destructive reset/delete/ignore safeguards;
5. message templates/history/cleanup and identity;
6. initial/normal/amend/continuation commit states;
7. hooks and signing/pinentry;
8. mutation serialization, index locks and external changes;
9. keyboard/focus/VoiceOver behavior.

**Exit:** every commit fixture produces verified object/index/worktree state and no data-loss blocker remains.

## Phase 4 — macOS productization

**Outcome:** installable preview artifact.

- Native menu roles, panels, clipboard, drag/drop, Finder/browser/terminal adapters.
- Bundle identity, icons, resources, versioning and third-party notices.
- Self-contained arm64 publish.
- Publish each accepted preview through a versioned GitHub release and repository-owned Homebrew
  Cask. An unsigned cask may be labeled as a development preview, but does not satisfy the MVP
  signing, notarization, or clean-account Gatekeeper gate.
- Hardened runtime, nested signing, notarization and stapling.
- Clean-account and Gatekeeper test.
- Redacted opt-in diagnostic bundle.
- Preview release notes with known limitations and exact source commit.

**Exit:** signed/notarized installed app passes the same primary flows.

## Phase 5 — preview feedback and stabilization

**Outcome:** evidence-based decision to broaden scope.

- Publish explicit supported/unsupported list.
- Collect opt-in crash/performance reports and user issues.
- Triage focus, accessibility, graph, status and commit correctness ahead of new features.
- Decide Intel/universal support.
- Decide whether to merge/coexist upstream and how to maintain the branch.

## Post-MVP order

1. fetch/pull/push and credentials;
2. branch/tag/stash operations;
3. conflict resolution and merge/rebase/cherry-pick/revert/reset;
4. worktree/submodule lifecycle;
5. settings completeness and translations;
6. file history/blame/search/patch flows;
7. portable plugins and hosting integrations;
8. updater and release channels;
9. optional Mac App Store investigation only after an explicit sandbox feasibility spike.

## Suggested issue slices

Each issue should have one observable outcome. Initial backlog:

1. Fix Avalonia executable/application-path discovery on macOS.
2. Add isolated macOS startup smoke fixture.
3. Reproduce/guard assembly-resolve recursion on macOS.
4. Verify configured Git discovery from Finder-launched app.
5. Add macOS ref-tree fixture parity.
6. Add macOS revision-graph golden/semantic parity fixture.
7. Add generation guard for rapid ref/filter changes.
8. Verify changed-files/diff cancellation and stale suppression.
9. Add complete status-class fixture.
10. Add whole-file stage/unstage E2E.
11. Add hunk/line stage/unstage E2E.
12. Add commit hooks/identity/amend/signing fixture.
13. Fix macOS native menus, key equivalents and focus.
14. Add keyboard-only/VoiceOver checklist and automation where stable.
15. Produce self-contained `.app` with stable resource lookup.
16. Add Developer ID signing/notarization pipeline.
17. Publish the Apple-silicon development preview through an immutable release and repository-owned
    Homebrew Cask.

Do not open these remotely until the upstream/fork strategy and tracker location are accepted.

## Risk register

| Risk | Impact | Mitigation |
|---|---|---|
| Huge draft PR cannot be reviewed/merged | Critical | Extract prerequisite changes; maintain parity ledger; coordinate early |
| One primary port developer | High | Document/run macOS harness; recruit owners; small reviewable changes |
| Wrong Git/index mutation loses work | Critical | Real Git, disposable E2E fixtures, serialized writes, readback, no auto-retry |
| Avalonia focus/accessibility gaps | High | Make keyboard/VoiceOver release gates; upstream minimal repros |
| GUI app cannot locate user Git/helpers | High | Explicit locator and environment diagnostics; Finder-launch tests |
| Hooks/signing/credential UI hangs or leaks | Critical | Real interactive flows, prompt broker, redaction, cancellation tests |
| App bundle resource assumptions | High | Bundle path adapter and installed-app tests |
| App Store sandbox conflicts | High | Direct distribution first; separate feasibility spike |
| GPL/name/branding mistakes | High | Preserve history/notices/source; maintainer guidance; no unapproved logo claim |
| Performance on large repositories | High | Virtualization, cancellation, generation guards, profile before new backend |
| Intel/universal expansion doubles matrix | Medium | Arm64 preview first; explicit later gate |

## Progress evidence rule

A phase is complete only when its outcome is tied to an immutable commit, exact environment, commands, machine-readable results, and installed-app evidence where applicable. A green checklist without readback is not completion.
