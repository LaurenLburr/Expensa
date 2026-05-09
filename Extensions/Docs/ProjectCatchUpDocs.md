# Project Catch-Up Docs

## Change

Each registered add-in project now gets project-specific document nodes:

```text
Add-in Projects
└── Registered Projects
    └── [ProjectName]
        ├── Catch-Up Doc
        └── Design Spec
```

## Behavior

Selecting the project `Catch-Up Doc` opens or creates:

```text
Extensions/Modules/[ProjectName]/Docs/CatchUp.md
```

Selecting `Design Spec` opens or creates:

```text
Extensions/Modules/[ProjectName]/Docs/Addin_Design_Spec.md
```

These docs are attached to the selected project, not the global solution docs.
