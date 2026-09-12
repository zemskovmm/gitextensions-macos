# ADR 0002: Use real Git through the shared Git Extensions engine

- **Status:** Proposed
- **Date:** 2026-09-02
- **Depends on:** ADR 0001

## Context

A Git client can invoke Git CLI, embed libgit2/LibGit2Sharp, embed another implementation such as gitoxide/JGit/go-git, or combine them. The MVP must reproduce status/index/diff/commit behavior including hooks, signing, credential helpers, filters, submodules, worktrees, unusual paths and new Git features.

Git Extensions already has a mature `GitCommands` and extensibility layer built around the real Git executable. Replacing it would discard behavior and create two parity problems at once.

## Proposed decision

Use the existing Git Extensions Git engine and configured real Git executable as the MVP authority for reads and mutations.

- Preserve byte/NUL-safe machine output until typed parsing boundaries.
- Execute direct argument vectors without a shell.
- Classify operations and conservatively serialize mutations by canonical Git common directory, covering linked worktrees that share refs and object storage.
- Cancel the whole process group on macOS.
- Let real Git own hooks, signing, credential helpers, filters and config precedence.
- Read back object/index/worktree/ref state after every mutation.

Do not add libgit2, gitoxide or another Git implementation to the MVP.

## Consequences

### Positive

- Highest semantic compatibility with the user’s Git setup and Git Extensions behavior.
- Correct path for hooks, signing, filters, LFS and helpers.
- Maximum source/test reuse.
- New Git behavior becomes available through executable/version capability checks rather than library release lag.

### Negative

- Process startup and parsing cost.
- GUI-launched PATH/tool discovery and interactive prompt handling are platform problems.
- CLI output contracts must be carefully pinned to machine-readable formats.
- Cancellation requires robust POSIX process-group handling.

## Later optimization policy

A second backend is allowed only after profiling identifies a measurable read bottleneck, a compatibility matrix is written, and CLI readback remains the oracle for mutations. Any hybrid path must expose which backend produced a result and have differential fixtures.

## Acceptance gate

This ADR becomes `Accepted` with the strategy decision and an explicit Git executable source policy: use supported system/user Git, ship a bundled Git, or support both. For a standalone product, the reliability-first recommendation is a complete pinned bundled Git plus a validated external override. If upstream governance favors its existing configured-Git model, require an explicit absolute path and version/capability preflight rather than relying on `/usr/bin/git` or a GUI process’s `PATH`.
