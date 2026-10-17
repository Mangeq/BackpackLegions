# Tower purchase panel quality fix

The shared card now uses `Assets/Resources/UI/TowerPurchase/Reference/CardHD.png`
(1016 × 1547, previously 250 × 381). Buy and close button sprites are cropped
from the same restored texture at runtime. Dynamic labels rasterize at four times
their source font size with an inverse transform scale, preserving their layout.

`UnitTabNeutralHD.png` replaces the baked gold unit tab. Selection still uses the
existing highlight state and callbacks. The static Rail image is disabled, leaving
only the four live buttons; its height ends at 229 instead of 375. The outer bounds
were shortened without moving the card on screen.

Unity Play Mode verification: nine checks passed (opening has no selection,
no lower rail, each of four selections highlights only itself, HD card loaded,
neutral unit art loaded, higher text density). No console errors. `unit-hd.png`
and `archer-hd.png` are actual game-view captures. In the archer capture the unit
tab remains neutral; in the unit capture it is correctly highlighted.

The two new textures were made with the built-in imagegen tool from the existing
project assets. The original low-resolution assets remain available. Other tower
portraits and gameplay data are unchanged. Running the original extraction script
does not overwrite these HD siblings.

## Final prompts

CardHD.png — input: `Assets/Resources/UI/TowerPurchase/Reference/Card.png`.
Transparent output.

> Edit target: the attached low-resolution blank game UI card. Restore/upscale this EXACT existing asset to high resolution, approximately 1000 by 1524 pixels. Preserve its exact silhouette, metal frame, dark brown wood, tan parchment, horizontal divider at same height, empty dark buy button with silver ends, and small X close button at upper right. Improve crispness and recover clear material edges and fine grain. No redesign, no change to proportions, colors or layout, no extra ornaments, no text or icons except the existing X. Keep all empty parchment and header areas empty, keep entire card inside image with same tight margins. Real transparent background outside the original silhouette. Production texture for same UI, faithful restoration only.

UnitTabNeutralHD.png — inputs: `UnitTab.png` (edit target) and `MortarTab.png`
(unselected frame reference) in the same resource directory.

> Edit target Image 1, a tower selection tab, using Image 2 only as reference for the unselected frame. Restore Image 1 at high resolution approximately 360x464. Change ONLY its gold selected frame and gold background into the neutral dark gray/iron unselected frame and background matching Image 2. Preserve EXACT orange-hat unit tower icon and its position, preserve BİRLİK label exactly and position; preserve rectangular proportions and tight crop. Sharpen icon and lettering to clean game UI resolution. No gold highlight, no glow, no additional content or padding. Single tab only, same original game art style. Do not copy the mortar icon or HAVAN text from Image 2.
