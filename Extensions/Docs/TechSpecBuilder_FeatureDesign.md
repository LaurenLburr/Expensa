# Tech Spec Builder Feature - Design Document

**Created:** 2026-04-26  
**Target:** CodexExpensa Extension Manager  
**Location:** Tech Spec Builder (New Feature)  
**Framework:** .NET 8 WinForms  

---

## Feature Overview

### What is the Tech Spec Builder?

A guided, two-conversation AI experience that helps addon developers build comprehensive technical specifications for their addons.

**Users:** Addon developers (using the Extension Manager)  
**Deliverable:** Tech spec document saved to `Extensions\Modules\WebsitesAddin\Docs\TechSpec.md`  
**AI Integration:** Guided conversations using OpenAI  

---

## User Flow

### Conversation 1: Vision & Requirements
1. User launches **"Build Tech Spec"** command
2. Form opens: **"Tech Spec Builder - Conversation 1"**
3. User describes their addon vision in a text area
   - *"I want to build an addon that manages website content..."*
4. User clicks **"Analyze with AI"**
5. AI responds with clarifying questions:
   - *"What specific website management tasks?"*
   - *"Who are the end users?"*
   - *"What integrations needed?"*
6. User answers questions in a conversational interface
7. AI generates **Section 1: Vision & Requirements** of the tech spec
8. User can refine/regenerate
9. User clicks **"Continue to Conversation 2"**

### Conversation 2: Implementation Strategy
1. Form opens: **"Tech Spec Builder - Conversation 2"**
2. Section 1 is shown as reference
3. User describes implementation approach/concerns
4. AI responds with follow-up questions:
   - *"What architecture pattern?"*
   - *"Data persistence approach?"*
   - *"Performance considerations?"*
5. User answers/discusses
6. AI generates **Section 2: Implementation Strategy** of the tech spec
7. Full tech spec is compiled (Section 1 + Section 2)
8. User clicks **"Save Tech Spec"**
9. Tech spec is persisted to `Extensions\Modules\WebsitesAddin\Docs\TechSpec.md`

### Revisit/Edit
- User can reopen the spec, edit it, or restart the conversation flow
- Each save creates a versioned backup

---

## Architecture Design

### 1. New Command

**File:** `Commands\BuildTechSpecCommand.cs`

```csharp
public sealed class BuildTechSpecCommand : ExtMgrCommandBase
{
    public override string CommandKey => "Tools.BuildTechSpec";
    public override string TopLevelMenu => "Tools";
    protected override string GetDefaultMenuText() => "Build Tech Spec";
    public override void Execute(ICommandContext context) { /* ... */ }
}
```

**Menu Location:** Tools → Build Tech Spec

---

### 2. New UI Forms

#### Form 1: TechSpecBuilderForm.cs (Main Container)
- Navigation between Conversation 1 & 2
- Tech spec preview
- Save/Export options

#### Form 2: TechSpecConversation1Form.cs
- Vision description text area
- AI prompting/questioning
- Conversation history panel
- "Analyze" button

#### Form 3: TechSpecConversation2Form.cs
- Implementation discussion area
- Conversation history panel
- Reference to Section 1 (readonly)
- "Generate" button

---

### 3. Data Model

**File:** `Models\TechSpec.cs`

```csharp
public class TechSpec
{
    public string AddonName { get; set; }
    public string ProjectName { get; set; }
    public string Version { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModified { get; set; }

    // Conversation 1 outputs
    public string VisionStatement { get; set; }
    public List<ConversationTurn> Conversation1 { get; set; }
    public string Section1_VisionAndRequirements { get; set; }

    // Conversation 2 outputs
    public List<ConversationTurn> Conversation2 { get; set; }
    public string Section2_ImplementationStrategy { get; set; }

    // Methods
    public string GenerateMarkdown() { /* ... */ }
    public void SaveToFile(string path) { /* ... */ }
    public static TechSpec LoadFromFile(string path) { /* ... */ }
}

public class ConversationTurn
{
    public string Role { get; set; } // "User" or "AI"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
```

---

### 4. Service Layer

**File:** `Services\TechSpecBuilderService.cs`

Responsibilities:
- Manage tech spec state
- Orchestrate AI conversations
- Generate prompts for AI
- Parse AI responses into structured data
- Persist tech specs to disk

---

### 5. AI Integration

**Extend:** `ICommandAiService` (already exists)

Add new methods:
```csharp
Task<string> AnalyzeVisionAsync(string visionText);
Task<string> AskClarifyingQuestionsAsync(string visionText, List<string> userAnswers);
Task<string> GenerateVisionSectionAsync(string visionText, List<ConversationTurn> turns);
Task<string> GenerateImplementationSectionAsync(List<ConversationTurn> allTurns);
```

---

### 6. Persistence

**Storage Location:** `Extensions\Modules\WebsitesAddin\Docs\TechSpec.md`

**Format:** Markdown with structured sections

```markdown
# WebsitesAddin - Technical Specification

**Version:** 1.0  
**Created:** 2026-04-26  
**Last Modified:** 2026-04-26  

## Section 1: Vision & Requirements

[Content from Conversation 1]

## Section 2: Implementation Strategy

[Content from Conversation 2]

---

*Tech Spec built with CodexExpensa Tech Spec Builder*
```

---

## Implementation Phases

### Phase 1: Core Infrastructure
1. Create `TechSpec` data model
2. Create `TechSpecBuilderService`
3. Create `BuildTechSpecCommand`
4. Create `TechSpecBuilderForm` (main container)

### Phase 2: Conversation 1 UI
1. Create `TechSpecConversation1Form`
2. Implement AI prompting logic
3. Test conversation flow

### Phase 3: Conversation 2 UI
1. Create `TechSpecConversation2Form`
2. Implement section generation
3. Full spec compilation

### Phase 4: Persistence & Polish
1. Implement save/load logic
2. File handling & versioning
3. Edit existing specs
4. Export options (PDF, etc.)

---

## Success Criteria

✅ **Functional:**
- Two-conversation flow completes successfully
- Tech spec is saved to correct location
- Can reopen and edit existing specs
- AI prompts guide user effectively

✅ **User Experience:**
- Clear, intuitive navigation
- Conversation history visible
- Tech spec preview before save
- Error handling & validation

✅ **Technical:**
- Integrates with existing command framework
- Uses existing AI service infrastructure
- No breaking changes to core system
- All code follows project conventions

---

## Files to Create

```
Extensions\
├── Host\CodexExpensa.ExtensionDevHost\
│   ├── Commands\
│   │   └── BuildTechSpecCommand.cs (NEW)
│   ├── Services\
│   │   └── TechSpecBuilderService.cs (NEW)
│   ├── Models\
│   │   └── TechSpec.cs (NEW)
│   └── UI\
│       ├── TechSpecBuilderForm.cs (NEW)
│       ├── TechSpecConversation1Form.cs (NEW)
│       └── TechSpecConversation2Form.cs (NEW)
└── Modules\CodexExpensa.Feature.Websites\
    └── Docs\
        └── TechSpec.md (GENERATED)
```

---

## Files to Modify

1. **CommandRegistry.cs** - Register new command
2. **ICommandAiService.cs** - Add new AI prompting methods
3. **OpenAiCommandAiService.cs** - Implement AI methods
4. **Program.cs** or DI container - Register TechSpecBuilderService

---

## Questions to Address

- Should we support multiple tech specs per addon?
- Do we need version control for specs?
- Should specs be editable in the UI or external editor?
- Export formats needed (PDF, HTML, etc.)?
- Should spec be part of addon package?

---

## Next Steps

1. **Conversation 1 with User:** Refine vision, confirm requirements
2. **Create Data Model:** Implement TechSpec class
3. **Build Core Service:** TechSpecBuilderService
4. **Design UI/UX:** Create forms with conversation flow
5. **Implement AI Prompts:** Design effective prompts
6. **Test & Iterate:** Full flow testing
7. **Documentation:** Update docs and help

---

*This document will be updated as the feature design evolves.*
