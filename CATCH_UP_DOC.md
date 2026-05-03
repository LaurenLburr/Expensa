# CodexExpensa Extension Manager - Catch-Up Document

## Project Overview
**Repository:** CodexExpensa (Expensa)  
**Branch:** Extension-manager  
**Target Framework:** .NET 8  
**Tech Stack:** WinForms, C#  
**Location:** `D:\Git\CodexExpensa\`

---

## Current Architecture

### Extension Host System
The Extension Manager uses a **command-based architecture** with a centralized registry system.

#### Key Components:

1. **Command System** (`Extensions\Host\CodexExpensa.ExtensionDevHost\Commands\`)
   - `IExtMgrCommand`: Interface for all commands
   - `ExtMgrCommandBase`: Abstract base class for implementing commands
   - `CommandRegistry`: Central registry for managing all commands
   - `CommandExecutor`: Runtime execution engine
   - `DefaultCommandContext`: Context passed to commands during execution

2. **Command Abstractions** (`Commands\Abstractions\`)
   - `ICommandContext`: Provides context to executing commands (Owner form, SelectedNode, ProjectName, ActiveDatabasePath, Services)
   - `ICommandServiceProvider`: Service locator for accessing command services
   - Extension methods for common context operations

3. **Command Services** (`Commands\Services\`)
   - `ICommandUiService`: UI interaction service (WinForms dialogs)
   - `ICommandLogger`: Logging service
   - `ICommandAiService`: AI integration service
   - Implementations: `WinFormsCommandUiService`, `DebugCommandLogger`, `OpenAiCommandAiService`, `FakeCommandAiService`

4. **Core Services** (`Services\`)
   - `ExtensionProjectRegistrationStore`: Manages extension project registrations
   - `AddinProjectScaffolder`: Scaffolds new addon projects
   - `OpenAiApiKeyStore`: Manages OpenAI API keys
   - AI Services: `OpenAiAddinScaffoldAiService`, `OpenAiAddinDesignerService`, `OpenAiDocumentReviewService`

---

## Current Project Structure

### Projects in Solution:
```
✓ CodexExpensa.ExtensionDevHost (Host Application)
✓ CodexExpensa.Feature.Websites (Module)
✓ CodexExpensa.Navigation.Abstractions (Core)
✓ CodexExpensa.Navigation.Hosting (Core)
✓ CodexExpensa.Navigation.Hosting.Tests (Tests)
```

### Main Entry Points:
- **MainForm.cs**: Primary WinForms window
- **Program.cs**: Application startup

### Command Implementations:
| Command | Purpose | Menu Location |
|---------|---------|---------------|
| `OpenAiApiKeyCommand` | Manage OpenAI API credentials | Project → OpenAI API Key |
| `NewAddinProjectCommand` | Create new addon projects | (Scaffolding) |
| `OpenCommandCatalogCommand` | View/manage command catalog | ? |
| `OpenQueryCatalogCommand` | Query management interface | ? |
| `OpenManageExtensionsCommand` | Extension management UI | ? |
| `AiTestCommand` | AI service testing | ? |
| `AiScaffoldFilesCommand` | AI-powered file scaffolding | ? |
| `OpenAiAddinDesignerCommand` | AI addon designer | ? |

---

## UI Components

### Forms:
- **MainForm.cs**: Main application window
- **CommandCatalogForm.cs**: Command discovery and management
- **OpenAiApiKeyForm.cs**: API key configuration
- **NewAddinProjectForm.cs** / **NewAddinProjectForm.ChatGpt.cs**: Addon project creation
- **ManageExtensionsForm.cs**: Extension management
- **AiAddinDesignerForm.cs**: AI-assisted addon design
- **DocsEditorForm.cs**: Documentation editor
- **ProjectSpaceForm.cs**: Project workspace

---

## Data & Configuration

### Data Models:
- `ExtensionProjectRegistration`: Extension project metadata
- `ExtensionRegistrationRecord`: Database record format
- `NewAddinProjectRequest`: Request for creating new addons
- `AddinScaffoldAiRequest` / `AddinScaffoldAiResult`: AI scaffolding contracts
- `AiAddinDesignSession`: State for AI design sessions
- `CommandMenuConfigEntry` / `CommandMenuConfigDocument`: Menu configuration

### Database:
- `ExtensionManagerDatabase.cs`: Database abstraction
- `JsonCommandMenuConfigStore.cs`: JSON-based menu config persistence

---

## Current Features Implemented

### ✅ Command Framework
- Base command infrastructure with menu metadata
- Command registry and execution pipeline
- Context passing (owner form, selected node, project/database info)
- Menu configuration (text, order, separators)

### ✅ OpenAI Integration
- API key management and storage
- Addon project scaffolding via AI
- AI-powered addon designer
- Document review service

### ✅ Extension Management
- Extension project registration
- New addon project creation
- Extension catalog UI

### ✅ UI Services
- WinForms dialog hosting
- Command logging
- AI service integration

---

## Key Design Patterns

1. **Command Pattern**: All operations are commands implementing `IExtMgrCommand`
2. **Service Locator**: `ICommandServiceProvider` provides runtime service access
3. **Context Object**: `ICommandContext` passes shared state to commands
4. **Template Method**: `ExtMgrCommandBase` provides common command lifecycle
5. **Configuration as Code**: Menu metadata can be stored and loaded from JSON

---

## File Organization Summary

```
Extensions\
├── Host\
│   └── CodexExpensa.ExtensionDevHost\
│       ├── Commands\
│       │   ├── Abstractions\        (Interfaces & contracts)
│       │   ├── Services\            (Command services)
│       │   ├── Runtime\             (Execution engine)
│       │   └── *.cs                 (Command implementations)
│       ├── Services\
│       │   ├── Ai\                  (AI service implementations)
│       │   └── *.cs                 (Core services)
│       ├── Models\                  (Data contracts)
│       ├── Data\                    (Database & persistence)
│       ├── UI\                      (WinForms dialogs)
│       └── *.cs                     (Main forms & app logic)
├── Core\
│   ├── CodexExpensa.Navigation.Abstractions\
│   └── CodexExpensa.Navigation.Hosting\
├── Modules\
│   └── CodexExpensa.Feature.Websites\
└── Tests\
    └── CodexExpensa.Navigation.Hosting.Tests\
```

---

## Next Steps / Known Areas

- **Extension catalog**: Expand command discovery
- **AI service refinement**: Improve scaffolding accuracy
- **Performance**: Monitor menu loading and command execution
- **Testing**: Expand test coverage (only Navigation.Hosting.Tests exists currently)

---

## Development Notes

- Use `ICommandContext.Services` to access runtime services
- Commands inherit from `ExtMgrCommandBase` and override abstract members
- UI dialogs use `ICommandUiService.ShowDialog()`
- Configuration is stored in JSON and loaded via `JsonCommandMenuConfigStore`
- All new commands should be registered in `CommandRegistry`

---

*Generated: Current workspace snapshot on Extension-manager branch*
