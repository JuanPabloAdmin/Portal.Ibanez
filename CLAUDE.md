# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Portal.Ibanez` — an ABP Framework 9 / .NET 9 layered DDD monolith (MVC + Razor Pages, LeptonX Lite theme, EF Core + PostgreSQL). It is a machine documentation portal: customers own machines, machines have folder trees of PDF documents, and QR codes give anonymous public access to a folder subtree.

UI strings, comments and commit messages are in Spanish. Keep new user-facing text in Spanish.

## Commands

```bash
dotnet build Portal.Ibanez.sln
```

```bash
dotnet run --project src/Portal.Ibanez.Web
```
Runs at `https://localhost:44363`.

Create/update the database and seed initial data (must be run after adding any migration):
```bash
dotnet run --project src/Portal.Ibanez.DbMigrator
```

Add a migration (EF Core tools must run from the EF project, which has `IbanezDbContextFactory`):
```bash
dotnet ef migrations add MyMigration --project src/Portal.Ibanez.EntityFrameworkCore
```

Tests (xUnit + Shouldly + NSubstitute, in-memory SQLite via `IbanezTestBase`):
```bash
dotnet test Portal.Ibanez.sln
```

Single test / filtered:
```bash
dotnet test test/Portal.Ibanez.Application.Tests --filter FullyQualifiedName~SampleAppService_Tests
```

Client-side libs (only needed if `wwwroot/libs` is missing or package.json changed), run in `src/Portal.Ibanez.Web`:
```bash
abp install-libs
```

## Layer flow — where a change goes

A new feature normally touches four projects in this order:

1. `Domain` — entity, a `FullAuditedAggregateRoot<Guid>` with a protected parameterless ctor plus a public ctor taking `id` + required fields. Anemic style: public setters, no domain services so far.
2. `Domain.Shared` — consts, localization JSON (`Localization/Ibanez/*.json`), error codes.
3. `Application.Contracts` — DTOs (`CreateUpdate<X>Dto`, `<X>Dto`, `Get<X>ListInput : PagedAndSortedResultRequestDto`) and the `I<X>AppService` interface.
4. `Application` — an `[Authorize] CrudAppService<Entity, Dto, Guid, ListInput, CreateUpdateDto>`, overriding `GetListAsync` to apply filters via `Repository.GetQueryableAsync()` + `AsyncExecuter`. Register mappings in `IbanezApplicationAutoMapperProfile`.
5. `EntityFrameworkCore` — add a `DbSet` and a `builder.Entity<X>` block in `IbanezDbContext.OnModelCreating` (`ToTable(IbanezConsts.DbTablePrefix + "Xs", IbanezConsts.DbSchema)` then `ConfigureByConvention()`), then a migration.
6. `Web` — Razor Pages under `Pages/<Feature>/` following the `Index` + `CreateModal` + `EditModal` convention, backed by `IbanezPageModel`. `HttpApi`/`HttpApi.Client` are auto-generated ABP API controllers; you rarely edit them.

## Domain model

- `Country` → `Customer` (FK `CountryId`, `DeleteBehavior.Restrict`; nullable en BD por los clientes previos a los países, pero obligatorio en `CreateUpdateCustomerDto`). La vista `/Customers` muestra primero los países y luego los clientes de uno (`?countryId=` / `?sinPais=true`).
- `Customer` → `Machine` (also references `MachineType`) → `DocumentFolder` (self-referencing tree via `ParentFolderId`, `DeleteBehavior.Restrict`) → `MachineDocument`.
- `QrCode` has a unique `Code` and points at a `MachineId` + optional `DocumentFolderId` root.
- All tables are prefixed `App` (`IbanezConsts.DbTablePrefix`), no schema.

## Two access paths — this is the main security boundary

- **Admin path**: everything under `Pages/` except `PublicQr`. App services are `[Authorize]`; the main menu (`IbanezMenuContributor`) is hidden entirely unless the user is in the `admin` role.
- **Public QR path**: `Pages/PublicQr/*` and `PublicQrAppService` are anonymous. `PublicQrAppService.FolderBelongsToRootAsync` walks `ParentFolderId` upward (with a visited-set cycle guard) to prove the requested folder is inside the QR's root folder. Any new anonymous endpoint must go through that containment check — otherwise a guessed folder/document id leaks another machine's documents.

Custom ABP permissions are **not** used: `IbanezPermissions` / `IbanezPermissionDefinitionProvider` are still the empty template stubs, and authorization is role-checks plus `[Authorize]`.

## File storage

PDFs are written to disk, not to a blob provider, and the DB row only stores the name:

`wwwroot/uploads/machines/{machineId}/{documentFolderId}/{guid:N}.pdf`

`MachineDocument.StoredFileName` is that GUID filename, `FileName` is the original. Upload (`MachineDocuments/CreateModal`, `UploadFolderModal`), download (`MachineDocuments/Download`, `PublicQr/Download`) and machine duplication (`Machines/DuplicateModal`, which copies files on disk) all rebuild this path independently — change one and you must change the others. Only PDFs are accepted.

`MachineDocumentAppService.UpdateAsync` bumps `Version` automatically when `StoredFileName` changes.

## Notes

- Multi-tenancy is compiled off via `MultiTenancyConsts.IsEnabled = false`; tenant menu items are removed accordingly.
- `appsettings.json` in `Web` and `DbMigrator` currently contain a real PostgreSQL host and credentials — don't propagate them into new files or logs.
- `.vs/`, `bin/`, `obj/` and `wwwroot/libs` are checked in or present locally; ignore them when searching.
