# Changelog
All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog
and this project follows Semantic Versioning.

---
## [1.1.1.2] - 2026-01-01
## Fix
- Insert and Update issue of parameters solved.

## [1.1.1.1] - 2026-01-01
## Added
- Exception Handlling

## [1.1.1] - 2026-01-01
## Added
- Execute Scalar
- Execute Scalar Async
- Insert and return Identity

## [1.1.0] – 2025-12-23

### Added
- Generic, database-agnostic CRUD operations:
  - Insert
  - Update
  - Delete
  - Select
- Stored procedure execution support.
- Transaction management via `DbTransactionScope`.
- Optional SQL logging via `IDbLogger`.
- Centralized database configuration using `DbConfig`.

### Improved
- Fully provider-based design using `DbProviderFactories`.
- Clean separation of concerns (Core, CRUD, SP, Transactions, Logging).
- Null-safety improvements and compiler warning fixes.
- XML documentation added for all public APIs (IntelliSense support).

### Fixed
- Interface and implementation mismatches.
- Nullable reference warnings in CRUD and logging layers.

---

## [1.0.0] – 2025-12-22 - Initial Release

### Added
- Core database abstraction (`IDatabase`)
- Provider-based connection handling
