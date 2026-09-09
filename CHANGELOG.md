# Changelog
All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog
and this project follows Semantic Versioning.

---
## [1.1.1.6] - 2026-09-09
- Documentation and build fixes
### Added
- Added support for asynchronous database operations in CRUD methods.
- added support for asynchronous execution of stored procedures.
- Added XML documentation comments for public API surface to remove missing XML comment warnings and improve IntelliSense:
- StoredProcedures/SpParam.cs — documented Name, Value and constructor.
- CRUD/ICrudRepository.cs — documented Execute, ExecuteAsync, Query, QueryAsync, InsertAndReturnId, SelectList/SelectListAsync, QueryList/QueryListAsync and related parameters/return values.
- Extensions/DataTableExtensions.cs — documented class and ToList extension method.
- Verified solution builds successfully after changes.

## [1.1.1.5] - 2026-01-01
### Added
•	Asynchronous stored-procedure support: ExecuteAsync (DataTable), ExecuteNonQueryAsync, ExecuteScalarAsync, ExecuteListAsync.
•	Multiple-result-set support for stored procedures:
•	ExecuteMultiple returning DataSet.
•	ExecuteMultiple<T1,T2> returning typed tuple (List<T1>, List<T2>).
•	DataTable async loading and tuple mapping implemented in StoredProcedures/StoredProcedureExecutor.cs.

## [1.1.1.4] - 2026-01-01
### Added
•	High-performance DbDataReader -> List<T> mapper (DataReaderMapper) to map query results to strongly-typed lists.
•	New List<T>-oriented APIs:
•	CRUD: QueryList<T>, QueryListAsync<T>, SelectList<T>, SelectListAsync<T>.
•	Stored procedures: ExecuteList<T>, ExecuteListAsync<T>.
•	DataMapper implementation: DataMapper/DataReaderMapper.cs.

## [1.1.1.3] - 2026-01-01
### Added
•	ExecuteScalar and ExecuteScalarAsync methods to CRUD API to run scalar queries and return single values.
•	InsertAndReturnId method to CRUD to insert a record and return the generated identity value.
•	Implementations added in CRUD/CrudRepository.cs (ExecuteScalar, ExecuteScalarAsync, InsertAndReturnId).

## ## Fixed
•	Exception handling improved in CRUD operations (DataLiteException wrapping).

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
