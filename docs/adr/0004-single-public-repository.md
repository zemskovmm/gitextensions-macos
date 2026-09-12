# ADR 0004: Use one public repository for the macOS application and Cask

- **Status:** Accepted
- **Date:** 2026-09-12
- **Decision owner:** Michael
- **Approval:** Owner approved the single-public-repository migration and its boundaries.
- **Supersedes:** ADR 0003

## Context

The predecessor workspace separated project coordination, application work, and Homebrew
packaging. That arrangement made public discovery and ordinary contribution unnecessarily
dependent on a private coordinating repository and pinned owner submodules. The project now needs
one public home while preserving the application and Cask histories that matter for maintenance.

## Decision

Use `zemskovmm/gitextensions-macos` as the single public repository.

- The application source and its ordinary build/test tooling live at repository root.
- The Homebrew Cask lives at `Casks/git-extensions-avalonia.rb`.
- Preserve the application and Cask histories when integrating them into this repository. Do not
  publish the predecessor's private coordination ledger or recovery material.
- Keep public specifications, architecture, ADRs, and operating guidance under `docs/`.
- Public documentation must not depend on private report links, historical screenshots, raw
  evidence captures, or predecessor-ledger ancestry.
- The legacy submodule layout is retired and must not be recreated.

## Consequences

Contributors clone one public repository, review application and Cask changes together when a
release requires both, and retain normal Git history for each imported public concern. The Cask is
reviewed as a repository path, not as a separately pinned owner repository.

This decision accepts repository organization only. It does not accept the strategy or Git-engine
proposals in ADRs 0001 and 0002, assert upstream acceptance, claim WinForms parity, or complete
any product milestone.

## Migration and release boundary

The candidate is pending the history-preserving tap merge and its verification. Target Homebrew
version 0.1.2 has not been built or published. A completed layout migration therefore must not be
reported as a released, installed, signed, notarized, or MVP-complete application. The MVP gates
in [the MVP specification](../mvp-specification.md) remain required.
