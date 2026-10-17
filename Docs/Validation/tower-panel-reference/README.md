# Approved panel integration

The approved input is copied to `Assets/Resources/UI/TowerPanel/Reference/Original.png`.
`Tools/UI/extract_reference_panel.py` extracts the original artwork, button faces,
stat icons and item cutouts. No image-generation service is used for this skin.
`parts.json` records the source rectangles. Reassembling Header, Inventory,
Storage and Shop reproduces the original PNG with zero pixel differences.

The live interface uses `Background.png` under existing Unity inventory and shop
components. Only changing data regions are cleared using empty pixels from the
same source. Numeric values, tower names, shop contents and item positions follow
the actual game state; this is not a static screenshot covering the game.

The reference only shows two storage columns. A clipped horizontal ScrollRect
retains access to all three columns of existing game storage without changing
capacity or multi-cell placement rules. The interface closes with the existing
Escape action.

Validation in Unity Play Mode: all 11 checks in
`Tools/Validation/reference_panel_runtime.cs` passed, including purchase into
storage, stat application on equip, rotated transfer, clipped drop coordinates,
access to the third storage column, reroll cost, and closing the panel. No Unity
console errors were present after the checks. Tests use runtime-only cloned data.

`reference-reassembled.png` validates source crop fidelity. `approved-panel-final.png`
shows the running interface with real, changing game data. The zero-difference
claim applies to the reassembled source crops, not the rendered game's live text.
