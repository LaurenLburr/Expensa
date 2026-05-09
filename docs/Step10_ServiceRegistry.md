# Step 10 – Service Registry

## What changed

Replaced:

object Services

with:

ICommandServiceProvider Services

## Benefits

- Multiple services supported
- Strong typing
- No casting hacks

## Usage

var ui = context.Services.GetRequiredService<ICommandUiService>();

## Test

Run app and verify all commands still work.