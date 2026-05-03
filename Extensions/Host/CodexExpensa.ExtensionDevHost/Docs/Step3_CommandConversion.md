# Step 3 – Convert One Command (OpenCommandCatalogCommand)

## Replace Execute method

```csharp
using CodexExpensa.ExtensionDevHost.Commands.Abstractions;
using System.Windows.Forms;

public override void Execute(ICommandContext context)
{
    var owner = context.Services as Form ?? Application.OpenForms[0];

    var form = 
    
    
    
    
    
    new CommandCatalogForm(_registry, _configPath, _rebuildMenu);
    form.Show(owner);
}
```

## Notes
- Only update THIS command
- Do not remove old commands yet
- This is the first step toward full context-based execution