namespace CodexExpensa.ExtensionDevHost.Models;

public sealed class AiAddinDesignSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    public string ProjectName { get; set; } = "NewAddin";
    public string? ProjectFolder { get; set; }
    public List<AiAddinDesignMessage> Messages { get; set; } = new();
    public string CurrentSpecMarkdown { get; set; } = GetDefaultSpec("NewAddin");
    public string CurrentIntegrationSpecMarkdown { get; set; } = GetDefaultIntegrationSpec("NewAddin");
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public static string GetDefaultSpec(string projectName)
    {
        return $"""
# {projectName} – Add-in Design Spec

## Purpose

Describe what this add-in should do.

## User Workflow

1. 
2. 
3. 

## Navigation / Tree Structure

```text
Root
└── Child
```

## Commands

| Command | Purpose |
|---|---|
| | |

## Forms

| Form | Purpose |
|---|---|
| | |

## Data Model

| Entity | Key Fields |
|---|---|
| | |

## Database Design

| Table | Purpose |
|---|---|
| | |

## Open Questions

- 

## Revision History

| Date | Change |
|---|---|
| | |
""";
    }

    public static string GetDefaultIntegrationSpec(string projectName)
    {
        return $"""
# {projectName} – Expensa Integration Design Spec

## Purpose

Describe how this add-in integrates with Expensa.

## Host Touchpoints

| Host Area | Integration Point | Notes |
|---|---|---|
| Main navigation tree | | |
| Menus/commands | | |
| Detail panel/content host | | |
| Database/session services | | |
| Query catalog / SqlQuery | | |
| Settings/configuration | | |

## Startup / Registration Flow

1. 
2. 
3. 

## Deployment / Update Flow

Describe how this add-in should be deployed or updated into Expensa.

## Risks / Open Questions

- 
""";
    }
}

public sealed class AiAddinDesignMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
