# Tower purchase card

The shared card, parchment, close button, purchase button and four selection tabs
are extracted from the approved reference. Original source pixels and rectangles
are preserved in `ArtSource/TowerPurchaseReference`. Rebuild the pieces with
`Tools/UI/extract_tower_purchase_panel.py` and the original PNG.

`TowerPurchaseSkin` adapts the existing UI without replacing its purchase or
selection callbacks. `UİManager.RefreshPurchaseCard` continues to provide each
tower's title, description, data prices and starting statistics. The unit portrait
comes from the approved reference; other portraits use their existing game model
renders with transparent margins trimmed. Text remains dynamic.

The right-aligned panel scales with the Canvas. It shows unit production fields
for barracks, range for archer/magic towers, and explosion radius for the mortar.

All 12 checks in `Tools/Validation/reference_purchase_runtime.cs` passed in Unity
Play Mode: four distinct data panels, event callbacks, screen bounds, insufficient
gold, slot release on closing, and purchasing exactly one of each tower at its data
price. `results.json` contains the results. No Unity console errors were present.

`all-four.png` is a side-by-side crop of four actual game-view captures. The later
portrait-only adjustment trims transparent model-render margins for consistent
framing; it does not alter selection, purchase logic or the source card artwork.
