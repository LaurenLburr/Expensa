# Tech Spec Builder Feature - Implementation Plan

**Status:** 🟢 Ready to Build  
**Created:** 2026-04-26  
**Target Framework:** .NET 8  

---

## Feature Summary

**Tech Spec Builder** helps addon developers document their work through two guided AI conversations.

**Access:** Select an addon project in the treeview → Properties panel shows "Build Tech Spec" button  
**Output:** Saves to `[AddonPath]\Docs\TechSpec.md`  
**Conversations:** Fresh each time (no persistence)  

---

## Architecture

### 1. New Navigation Tag

Add to `MainForm.cs`:

```csharp
private sealed record TechSpecBuilderNavigationTag(string ProjectName, string ProjectFolder);
```

### 2. Update BuildAddinProjectsNode()

When `ProjectNavigationTag` is selected, show properties panel with:
- Project name
- Project folder path
- **"Build Tech Spec" button**

### 3. New Service: TechSpecBuilderService

**File:** `Services\TechSpecBuilderService.cs`

Responsibilities:
- Manage tech spec state
- Coordinate AI conversations
- Generate prompts for AI
- Compile final tech spec
- Save to addon's Docs folder

### 4. New Forms

#### TechSpecBuilderMainForm.cs
- Container for conversation flow
- Navigation between conversations
- Tech spec preview
- Save & close buttons

#### TechSpecConversation1Form.cs
- Vision description textarea
- AI analysis & clarifying questions
- Conversation history panel
- "Continue to Conversation 2" button

#### TechSpecConversation2Form.cs
- Implementation discussion textarea
- Reference to Section 1 (readonly)
- AI follow-up questions
- "Generate Tech Spec" button
- Final spec preview

### 5. Data Model

**File:** `Models\TechSpec.cs`

```csharp
public class TechSpec
{
    public string AddonName { get; set; }
    public string ProjectFolder { get; set; }
    public DateTime CreatedDate { get; set; }

    // Conversation 1
    public string VisionStatement { get; set; }
    public List<ConversationMessage> Conversation1 { get; set; } = new();
    public string Section1_VisionAndRequirements { get; set; }

    // Conversation 2
    public List<ConversationMessage> Conversation2 { get; set; } = new();
    public string Section2_ImplementationStrategy { get; set; }

    public string GenerateMarkdown() { /* compile full spec */ }
    public void SaveToFile(string path) { /* save as markdown */ }
}

public class ConversationMessage
{
    public string Role { get; set; } // "User" or "AI"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### 6. AI Integration

Extend `ICommandAiService`:

```csharp
// Conversation 1 prompts
Task<string> AnalyzeTechSpecVisionAsync(string visionText);
Task<string> AskTechSpecClarifyingQuestionsAsync(string visionText, List<string> userAnswers);
Task<string> GenerateTechSpecVisionSectionAsync(string visionText, List<ConversationMessage> turns);

// Conversation 2 prompts
Task<string> AskTechSpecImplementationQuestionsAsync(List<ConversationMessage> conversation1, string implementationText);
Task<string> GenerateTechSpecImplementationSectionAsync(List<ConversationMessage> allTurns);
```

---

## Implementation Phases

### Phase 1: Properties Panel & Navigation
- [x] Add `TechSpecBuilderNavigationTag` record
- [x] Update `MainForm.BuildAddinProjectsNode()` to show button
- [x] Handle "Build Tech Spec" button click

### Phase 2: Data Model
- [x] Create `TechSpec.cs` model
- [x] Create `TechSpecBuilderService.cs`
- [x] Implement markdown generation & file save

### Phase 3: Conversation 1 UI
- [x] Create `TechSpecBuilderMainForm.cs` (container)
- [x] Create `TechSpecConversation1Form.cs`
- [x] Implement vision input & AI prompting
- [x] Test conversation flow

### Phase 4: Conversation 2 UI
- [x] Create `TechSpecConversation2Form.cs`
- [x] Implement implementation discussion & AI prompting
- [x] Spec compilation & preview
- [x] Save functionality

### Phase 5: AI Integration
- [ ] Extend `ICommandAiService`
- [ ] Implement prompts in `OpenAiCommandAiService`
- [ ] Test AI conversations

### Phase 6: Testing & Polish
- [ ] End-to-end testing
- [ ] Error handling
- [ ] UI/UX refinement
- [ ] Documentation

---

## Files to Create

```
Extensions\Host\CodexExpensa.ExtensionDevHost\
├── Services\
│   └── TechSpecBuilderService.cs (NEW)
├── Models\
│   └── TechSpec.cs (NEW)
└── UI\
    ├── TechSpecBuilderMainForm.cs (NEW)
    ├── TechSpecConversation1Form.cs (NEW)
    └── TechSpecConversation2Form.cs (NEW)
```

## Files to Modify

1. **MainForm.cs**
   - Add `TechSpecBuilderNavigationTag` record
   - Update `BuildAddinProjectsNode()` to show properties panel
   - Handle "Build Tech Spec" button click

2. **Commands\Abstractions\ICommandContext.cs**
   - May need to extend for addon context (optional)

3. **Commands\Services\ICommandAiService.cs**
   - Add tech spec specific AI methods

4. **Commands\Services\OpenAiCommandAiService.cs**
   - Implement tech spec AI methods

---

## Success Criteria

✅ **Functional**
- [ ] Select addon → properties panel appears with button
- [ ] Click "Build Tech Spec" → conversation 1 form opens
- [ ] Complete conversation 1 → section generated
- [ ] Move to conversation 2 → implementation discussion
- [ ] Complete conversation 2 → full spec generated & saved

✅ **Output**
- [ ] Tech spec saved to `[AddonPath]\Docs\TechSpec.md`
- [ ] Markdown format matches expected structure
- [ ] Full spec includes both conversations & AI analysis

✅ **UX**
- [ ] Clear navigation between conversations
- [ ] AI prompts guide users effectively
- [ ] Conversation history visible
- [ ] Easy to review before saving

---

## Next Steps

1. ✅ Design complete
2. 🔄 Phase 1: Add navigation & properties panel
3. 🔄 Phase 2: Create data models
4. 🔄 Phase 3: Build conversation forms
5. 🔄 Phase 4: Integrate AI
6. 🔄 Phase 5: Test & polish

**Ready to start Phase 1!**

---

*This plan will be updated as implementation progresses.*
