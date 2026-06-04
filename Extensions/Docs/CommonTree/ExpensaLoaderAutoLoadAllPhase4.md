# Expensa Loader Auto Load All Phase 4

The Expensa loader test no longer requires double-clicking or multiple manual loads.

## Changes

- The form automatically runs `Load All` on first show.
- `Load All` is the default AcceptButton.
- The button order now puts `Load All` before `Load Selected`.
- After loading, the first add-in node is selected automatically.
- Status text shows how many add-ins loaded.

This makes the loader test closer to the final Expensa integration behavior.
