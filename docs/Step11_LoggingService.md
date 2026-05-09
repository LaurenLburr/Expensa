# Step 11 – Logging Service

## Add Service

Commands can now log using:

var logger = context.Services.GetRequiredService<ICommandLogger>();
logger.Log("Command executed");

## Wire in CommandRegistry

Add:

services.Register<ICommandLogger>(new DebugCommandLogger());

## Test

Run app and open:
Tools → Command Catalog

Check Output Window (Debug) for log message.