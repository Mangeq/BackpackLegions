# Approved UI persists in the gameplay scene

Previously both approved layouts were constructed only by Start(), so Edit Mode
still contained the legacy UI. GameScane now stores both complete native UI
hierarchies and serialized layout bindings. Generated sprites and the two TMP
fonts/materials/atlases are real assets in `Assets/Resources/UI/ApprovedScene`.
Start retains an install fallback for scenes without the approved components;
the saved gameplay scene uses its existing components and creates no duplicates.

Purchase, backpack and shop panels are saved closed. The always-active UI manager
holds direct references and initializes the closed panel controllers once. Their
later Awake/Start calls do not close a panel on its first opening. Hidden price
labels and purchase/shop panels no longer depend on GameObject.Find in this scene.

`ApprovedUISceneBuilder.Apply()` can reapply the scene setup from the Tools menu.
It is idempotent for the saved hierarchy. `before.unity.txt` preserves the scene
contents, including the user's previously unsaved edits, before the first apply.

Validation:

- Reloaded the scene from disk in Edit Mode: one layout, one purchase skin,
  persistent background/card/unit sprites, persistent fonts, both panels closed.
- `edit-mode-approved.png` renders the saved native inventory UI without entering
  Play Mode. Camera-space rendering was enabled temporarily for capture, then
  the saved scene was reloaded to restore its original overlay Canvas settings.
- Fresh Play Mode starts with both panels closed and one instance of each layout.
- All 12 existing purchase checks passed, including four actual purchases.
- All 11 existing inventory checks passed, including dragging between shop, depot
  and equipment, rotated drops, depot clipping/scrolling, reroll cost and closing.
- First-open inventory remains open after its controllers receive Start().
- No Unity console errors; stopped Play Mode without saving test purchases.
