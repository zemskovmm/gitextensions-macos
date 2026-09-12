# ADR 0001: Build on the upstream Avalonia port

- **Status:** Proposed
- **Date:** 2026-09-02
- **Decision owners:** Michael and Git Extensions maintainers for upstream acceptance

## Context

Git Extensions is a GPL-3.0 C#/WinForms application with substantial Windows-specific project and UI coupling. An active upstream draft PR, [#13189](https://github.com/gitextensions/gitextensions/pull/13189), already introduces .NET 10/Avalonia projects and ports the main browse/tree/graph and commit surfaces. The inspected draft is large, not yet macOS-validated to MVP quality, and currently has at least one real macOS startup blocker.

The alternatives are an independent Avalonia derivative, a native Swift rewrite, or another cross-platform rewrite.

## Proposed decision

Contribute to and harden the upstream Avalonia port. Use this single public repository as the implementation location while preserving upstream history and attribution. Keep macOS fixes and framework-neutral extractions small enough for upstream review and preserve source mapping/parity evidence.

Do not start a greenfield Swift or web-shell implementation while this decision stands.

## Consequences

### Positive

- Reuses the existing Git engine, graph logic, models, settings, translations, tests, plugins and already-ported UI.
- Fastest route to a credible MVP.
- Keeps behavior aligned with Git Extensions rather than creating a similarly named client.
- Mac hardware/testing can fill an explicit gap in the current upstream effort.

### Negative

- Work depends on upstream coordination and review capacity.
- The current draft’s size and compatibility-shim strategy create maintenance risk.
- Avalonia does not automatically provide native-quality focus, menu, accessibility or performance behavior.
- Public source distribution must comply with GPL-3.0.

## Guardrails

- Pin exact upstream SHAs.
- Preserve commit history and attribution.
- Separate framework-neutral refactors from platform behavior where practical.
- Require real Mac and installed-app evidence.
- Label independent previews unofficial unless maintainers approve branding.
- Revisit this ADR if upstream rejects the direction or the draft cannot be reduced/maintained.

## Alternatives rejected for now

- **Independent Avalonia fork:** duplicated governance and synchronization without technical benefit.
- **SwiftUI/AppKit:** best native result, but a new product and much larger semantic rewrite.
- **Electron/Tauri/Flutter/Qt/Java:** no useful advantage over existing C#/Avalonia work for this port.
- **Wine wrapper:** not a maintainable first-class macOS application.

## Acceptance gate

This ADR becomes `Accepted` only after Michael confirms the strategy and permission to contact/fork upstream publicly, and the upstream collaboration route is recorded.
