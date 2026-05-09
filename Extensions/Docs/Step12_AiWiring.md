# Step 12 – AI Wiring (Test Version)

## What this does

Adds a simple AI service and a test command.

## Add to CommandRegistry.Invoke

services.Register<ICommandAiService>(new FakeCommandAiService());

## Test

Run:
Tools → AI Test

You should see a generated message box.

## Next

Replace FakeCommandAiService with your OpenAI service.