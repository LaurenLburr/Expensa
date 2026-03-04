
# 📄 Expensa 2 — ChatGPT Catch-Up

## Purpose
Codex Expensa is a WinForms desktop app for managing Banks and Accounts with:
- strict layered architecture
- SQLite persistence
- in-memory working DB seeded from file + explicit Save() flush
- Windows Credential Manager for secrets
- TreeView navigation + docked child forms

This file is the single source of truth for continuing work in a new chat.

---

## Hard Rules (Do Not Violate)

### Layering
**CodexExpensa.Core**
- Domain models + interfaces only
- NO SQLite
- NO WinForms
- NO infrastructure

**CodexExpensa.Data.Sqlite**
- Owns all SQLite access
- Implements repository interfaces
- Implements migrations
- Implements the database session

**CodexExpensa.App.WinForms**
- Composition root + all UI
- NO SQL
- No direct SQLite usage
- Uses only Core abstractions/interfaces

### Database session
UI uses **IDatabaseSession** only.
Two modes:
- File DB
- In-memory DB seeded from file; Save() flushes memory → disk

### Secrets
Credentials are NEVER stored in SQLite.
Stored in Windows Credential Manager via:
- ICredentialStore → WindowsCredentialStore
- Key generation via CredentialKeys
Current scope: bank-level credentials.

### UI model
MainForm is a shell.
All screens are docked child forms.
TreeView is the system spine.

---

## Solution Structure

### CodexExpensa.Core

#### Abstractions
**IDatabaseSession**
- void Save()
- bool IsInMemory
- string? PersistedFilePath

**ICredentialStore**
- void Save(string key, string username, string password)
- bool TryGet(string key, out string? username, out string? password)
- void Delete(string key)

**CredentialKeys** (static)
- string Bank(string bankId)
- string Account(string accountId)
- (optional future) BankForUser(bankId, userId)

#### Domain
**Bank**
- BankId
- BankName
- RoutingNumber
- Url
- IsActive

**Account**
- AccountId
- AccountNickname
- SortIndex
- AccountNumber
- BankId
- BankName (read model projection)
- RoutingNumber (read model projection)
- Url (read model projection)
- AccountType
- IsActive

**AccountType**
- Checking
- DebitAccount

#### Repository Interfaces
**IBankRepository**
- IReadOnlyList<Bank> GetAll()
- Bank? GetById(string bankId)
- void Add(Bank bank)
- void Update(Bank bank)
- void Delete(string bankId)

**IAccountRepository**
- IReadOnlyList<Account> GetAll()
- Account? GetById(string accountId)
- void Add(Account account)
- void Update(Account account)
- void Delete(string accountId)

---

### CodexExpensa.Data.Sqlite

#### Database Session
**SqliteDatabase** : IDatabaseSession, IDisposable
- OpenFile(dbPath)
- OpenMemorySeededFromFile(dbPath)
- Save() flushes memory → file when in-memory mode
- Query<T>(sql, map, params, tx)
- ExecuteNonQuery(sql, params, tx)
- ExecuteInTransaction(Action<SqliteTransaction>)

#### Repositories
**SqliteBankRepository** : IBankRepository
- full CRUD for Bank

**SqliteAccountRepository** : IAccountRepository
- full CRUD for Account
- read joins Account → Bank to populate projection fields for UI
- Add resolves bank (by BankId, or via GetOrCreate path depending on implementation state)

#### Migrations
**MigrationRunner**
- ApplyPendingMigrations(dbSession or sqlite db, depending on current API)
**SqliteMigrationStatusProvider** : IMigrationStatusProvider
- Provides IReadOnlyList<MigrationStatusRow> GetStatus(IDatabaseSession)

(Keep interfaces returning Core-owned types; Data.Sqlite adapts.)

---

### CodexExpensa.App.WinForms

#### Composition Root
**AppBootstrapper** (or equivalent)
- determines DB file path under LocalAppData\CodexExpensa\db\codexexpensa.db
- creates IDatabaseSession using SqliteDatabase.OpenMemorySeededFromFile(dbPath)
- applies migrations
- constructs MainForm with injected dependencies:
  - IDatabaseSession
  - IAccountRepository
  - IBankRepository
  - Func<Form> createDbStatusForm
- hooks FormClosing to Save() and Dispose session

#### Navigation Model (TreeView)