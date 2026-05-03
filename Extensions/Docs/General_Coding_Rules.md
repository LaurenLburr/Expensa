# General Coding Rules

Note: Use zip files for generated code.

## Core Development Philosophy

- Prefer simple, maintainable architectures over clever abstractions.
- Design first; generate code second.
- Preserve transparency and debuggability.
- Avoid hidden "magic" behavior whenever possible.
- Ensure the system remains understandable months later.

## Language / Platform Standards

- Primary language: C#.
- Primary UI framework: Windows Forms (WinForms).
- Preferred IDE: Visual Studio.
- SQLite is the preferred embedded database.
- Use the modern Visual Studio Extensibility SDK only for Visual Studio extension work.
- Avoid legacy VSIX/VSSDK APIs unless explicitly approved.

## Source of Truth Rules

- SQL queries should live in the SQL catalog or the database, not embedded throughout application code.
- Documentation belongs inside the solution/project structure.
- Design decisions should be captured in specs and catch-up documents.

## File Delivery Rules

- Always provide complete, updated files.
- Avoid partial snippets when modifying production files.
- Zip files should preserve the solution-root structure.
- Designer-based WinForms forms should include the following files:
  - `.cs`
  - `.Designer.cs`
  - `.resx`

## UI Standards

- Prefer split layouts for navigation-heavy workflows.
- Toolbars should use recognizable icons.
- Forms should remain designable in Visual Studio designer mode.
- Avoid cramped layouts and clipped controls.
- Use docking and anchoring correctly.
- Enable word wrap where appropriate for documentation editing.

## AI Workflow Standards

AI should assist with:
- Design discussions.
- Requirement clarification.
- Spec refinement.
- Documentation review.
- Code generation.
- Revision workflows.

- AI-generated code should remain editable and understandable by humans.
- Design conversations should persist per project.
- Generated documentation should export into the project structure.

## Testing Standards

- Avoid mocks whenever possible.
- Prefer integration-style tests.
- Tests should reflect real runtime behavior.
- Use environment variables for secrets in tests.
- Do not store secrets in source control.

## Architecture Standards

- Keep UI layers thin.
- Keep core logic headless when possible.
- Prefer guard clauses and early returns.
- Prefer explicit models over dynamic structures.
- Prefer composition over deep inheritance trees.

## Database Standards

- SQLite is preferred for local storage.
- Table names should generally be singular.
- Use migration scripts for schema evolution.
- Store enums by name rather than by integer value.
- Preserve backward compatibility where practical.

## Documentation Standards

Each add-in project should contain:

```text
Docs/
├── CatchUp.md
├── Addin_Design_Spec.md
├── Expensa_Integration_Design_Spec.md
└── AI_Conversation_Summary.md
```

## Naming Standards

- Use meaningful names.
- Avoid abbreviations unless industry-standard.
- Prefer explicit command names.
- Keep namespace organization clean and predictable.

## Error Handling

- Fail loudly during development.
- Provide meaningful exception messages.
- Avoid silent catches.
- Log important operations.

## Project Workflow

1. Discuss the design.
2. Refine requirements.
3. Create or update specs.
4. Review architecture.
5. Generate scaffolding.
6. Review generated code.
7. Revise with AI assistance.
8. Test incrementally.
9. Update catch-up documents.

## Future Direction

The long-term goal is a conversational project-design system where:

```text
Talk to the AI until the software exists.
```

The AI should function as:
- Architect.
- Reviewer.
- Scaffolder.
- Documentation assistant.
- Revision assistant.

All while leaving the developer in control.