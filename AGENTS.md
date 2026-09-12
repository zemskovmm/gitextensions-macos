# GitExtensions macOS Repository Instructions

## Repository purpose and boundaries

- This is the single public repository for the macOS application, its public decisions, and its Homebrew Cask.
- Application source and ordinary build/test tooling live at repository root (`src/`, `eng/`, `tests/`, and solution files). The Cask is `Casks/git-extensions-avalonia.rb`; do not recreate separate owner submodules for either concern.
- Preserve application and Cask history when moving or integrating their files. Private recovery material, raw reports, screenshots, and predecessor-ledger ancestry stay local-only and must not enter public history or public documentation.
- Treat `docs/adr/0001-build-on-upstream-avalonia-port.md` and `docs/adr/0002-use-git-cli-through-shared-engine.md` as proposals until the owner accepts them.
- Keep product specifications, architecture, ADRs, and source ledgers under `docs/`.

## Upstream-first workflow

- Inspect the current state of upstream issue `gitextensions/gitextensions#13188` and draft PR `#13189` before planning or implementing port work.
- Record the exact upstream commit SHA used for every build, test, review, or parity claim.
- Prefer small prerequisite refactors and macOS fixes that can be reviewed upstream over a long-lived divergent rewrite.
- Never claim WinForms parity from build success alone. Require behavioral evidence against the same repository fixture and operation matrix.
- Keep all third-party code, assets, translations, copyright notices, and GPL obligations traceable.

## Architecture constraints

- Preserve the real Git CLI as the authority for mutating semantics in the MVP.
- Execute Git directly with structured argument lists; never interpolate repository-controlled text into a shell command.
- Keep UI code independent from process execution through interfaces and immutable result models.
- Serialize mutations conservatively per canonical Git common directory so linked worktrees sharing refs/object storage cannot race; reads may be concurrent only when Git and repository state permit it.
- Treat repository paths, filenames, commit messages, config, hooks, diffs, remotes, submodules, and terminal escape sequences as untrusted input.
- Keep credentials in Git Credential Manager or macOS Keychain-backed flows. Never persist or log secrets.
- Keep platform behavior behind adapters: file dialogs, clipboard, notifications, browser/shell launch, credentials, menus, key commands, filesystem watching, and app updates.

## Working rules

- Read the relevant upstream source, tests, port map, parity ledger, and current macOS evidence before changing behavior.
- Preserve unrelated dirty work; do not reset, clean, stash, or overwrite it.
- Keep each change to one testable outcome.
- Do not commit, push, open/close issues, publish packages, or modify upstream unless explicitly requested. The initial repository creation request is the sole standing exception for its bootstrap commit and push.
- Separate confirmed observations, hypotheses, and decisions in every report.
- Public documentation may link only public sources or sanitized public evidence; do not make it depend on private local reports or recovery paths.

## Testing policy

- Tests are evidence for durable behavior, not a target count.
- Add the smallest stable coverage that protects Git semantics, path/encoding edge cases, cancellation, concurrency, platform integration, and confirmed regressions.
- Prefer end-to-end fixture repositories for tree and commit workflows; add lower-level tests only when they isolate difficult logic.
- Never test private implementation shape or the absence of code.
- Destructive Git tests must operate only in disposable repositories with local identity and isolated HOME/XDG settings.

## Required MVP verification

Before calling the macOS MVP complete:

1. build the reviewed upstream/source SHA with zero errors and no newly accepted warnings;
2. pass focused and relevant upstream tests;
3. pass the fixture matrix in `docs/mvp-specification.md` on Apple silicon;
4. verify the refs tree, revision graph, commit details, changed files, and diff behavior against CLI ground truth;
5. verify staging, unstaging, partial staging, amend, hooks, signing behavior, conflicts, filenames/encodings, cancellation, and external refresh;
6. run keyboard-only and VoiceOver checks;
7. verify an installed, signed, notarized app from a clean user account;
8. record exact commands, SHA, environment, results, and evidence paths in governed evidence; public documentation may cite only vetted, non-sensitive evidence.

## Planning and decisions

- Every saved plan has one goal, explicit non-goals, acceptance criteria, risks, verification, and a short plain-language summary.
- When scope or parity is unclear, inspect upstream and obtain reproducible evidence before asking the owner.
- Record accepted durable decisions as ADRs or update the owning document; do not leave decisions only in chat or a transient report.
