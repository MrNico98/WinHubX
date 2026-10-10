# Security engineering and risk controls

This document records engineering controls used to reduce WinHubX risks. NIS2 is used as a risk-management reference; this document is not a legal assessment, certification, or claim that WinHubX or its maintainers are in scope or compliant. Applicability must be determined separately for the relevant entity and Italian implementing law.

## Risk-based controls

| Risk area | WinHubX controls | Evidence to retain |
|---|---|---|
| Dependency and supplier compromise | Central NuGet versions in `Directory.Packages.props`; Dependabot monitors the repository root; review package provenance and transitive changes before merging | Lock/restore output, reviewed dependency PR, vulnerability audit |
| Vulnerable code or regressions | Release build treats warnings as errors; automated tests and CodeQL run in GitHub Actions | CI run, test results, CodeQL alerts and remediation |
| Startup failure in the distributed build | A Windows CI smoke test starts the published executable, checks that the main window is created, then terminates the test process | CI run log for the publish/startup check |
| Localization tampering or stale UI resources | Only allowlisted `it`/`en` catalogs embedded in the application are loaded; no mutable translation copies are read from the profile | Localization contract tests and packaged-application smoke test |
| Malicious or substituted downloads | HTTPS-only allowlists, explicit user consent, SHA-256 validation, temporary files and atomic replacement where supported | Manifest, expected and computed digest, release provenance |
| Privilege escalation or unintended system changes | Application runs as the current user; privileged operations require explicit user action and visible elevation; no silent Defender exclusions or remote activation scripts | Code review, UAC behavior, security regression tests |
| Incident or release failure | Vulnerabilities are reported privately; release/update paths validate metadata and retain rollback behavior | Security advisory, affected-version range, mitigation and release notes |
| Resilience and recovery | Back up or preserve user/system state before high-impact operations; report partial failure rather than claiming success | Operation logs, restore guidance and recovery tests |

## Review procedure

For each material change, record the affected assets and dependencies, plausible abuse or failure cases, likelihood and impact, mitigations, test evidence, and remaining risk. Reassess the controls after dependency updates, changes to elevation/download/update behavior, and security incidents. Do not treat a clean vulnerability scan as proof that a package or release is trustworthy.

This approach is informed by Article 21 of [Directive (EU) 2022/2555](https://eur-lex.europa.eu/eli/dir/2022/2555/oj/eng) and ENISA's [NIS2 Technical Implementation Guidance](https://www.enisa.europa.eu/publications/nis2-technical-implementation-guidance). ENISA describes its guidance as non-binding and targeted to specified sectors; it is used here only as an engineering reference.
