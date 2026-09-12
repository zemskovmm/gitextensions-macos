# ADR 0003: Consolidate the macOS workspace using pinned submodules

- **Status:** Superseded by [ADR 0004](0004-single-public-repository.md)
- **Date:** 2026-09-05
- **Decision owner:** Michael

## Historical context

This decision described a predecessor coordination layout that used pinned application and
Homebrew submodules. That layout was an intermediate workspace organization, not an application
or distribution architecture.

## Supersession

Owner approval of ADR 0004 replaces the predecessor layout with one public repository containing
the application at its root and the Cask at
`Casks/git-extensions-avalonia.rb`. Instructions in this ADR to create, update, or synchronize
separate owner submodules are no longer active and must not be reused.

## Record boundary

The predecessor's detailed recovery material, raw reports, screenshots, and original ledger
ancestry are local-only and intentionally absent from public documentation. This supersession does
not accept ADRs 0001 or 0002, claim product parity, publish a release, or waive any MVP gate.
