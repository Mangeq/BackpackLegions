# Element inventory, offer reservations and loose depot

The approved panel now renders at the old 1600×930 size on the 1920×1080 Canvas, scaling down for smaller canvases. Element selection buttons and the baked row are hidden/covered; the targeting controls remain available. The saved scene includes the new button, background and StoragePileUI component before Play Mode.

At level five, unreserved shop offers can include Fire, Water, Earth and Air catalyst items (80 gold, one cell). Each refresh at level five or above guarantees a catalyst in an available unreserved slot. Reserved slots are never overwritten to satisfy this guarantee. Below five catalysts are excluded. XP progression continues without waiting for element selection.

During preparation, an equipped catalyst touching a main weapon along an edge consumes itself and replaces that weapon with TowerData's matching elemental main item. Diagonal/nonadjacent contact does not fuse. Replacement validates the grid first; failed replacement consumes nothing. Attached parts are released into storage rather than lost. Existing projectile/impact elemental damage is used; elemental weapons receive a matching inventory tint. The old direct/paid element-selection APIs cannot bypass the catalyst.

Right-click a live shop offer to reserve/unreserve it. A REZERVE badge identifies that offer. The reservation belongs to that tower and slot, survives paid rerolls and turn refreshes, and clears when the offer is purchased. It does not spend gold. Whole-shop freezing is removed from the available controls.

Tümünü depoya taşı moves all equipped root items, including the main weapon and its attachments, into the tower's depot without charging gold or changing identity. The main weapon can be dragged back onto the equipment grid. Depot items grant no equipped bonuses. The depot model has no grid occupancy or fifteen-cell capacity limit. Its UI simulates unscaled gravity and collisions using fitted image bounds; piles settle even while preparation pauses combat time. Overflow extends the vertically scrollable depot, and positions are kept per runtime item when the UI is reopened.

Validation: 20 checks in Tools/Validation/element_inventory_runtime.cs passed, plus all 16 tower/element combinations and the actual bulk button callback (37 checks). These cover gate/progression, fusion/consumption, fire projectile damage, real shop drag/drop, charges, reservation renewal, main weapon round trip, capacity and physics. No Unity console errors. The scene was reloaded from disk after stopping Play Mode: 1600×930, four catalyst assets, removed element controls, native bulk button and physics component persisted; panels remained closed. final-panel.png is an actual game-view capture. before.unity.txt preserves the scene before this feature setup.

The four new transparent icons use the built-in imagegen tool. Final assets are Assets/Resources/UI/Elements/{Fire,Water,Earth,Air}.png; matching ItemData assets are Assets/Resources/Data/Items/Elements/{Fire,Water,Earth,Air}.asset. ElementInventoryBuilder.Apply() sets up these data assets and the saved scene through Unity APIs.

## Final image prompts

### Fire.png

One standalone fantasy inventory catalyst item icon for a wood-and-parchment tower defense backpack game: a compact orange-red fireball with a bright yellow molten core and short curling flames. Clean hand-painted game illustration, thick readable silhouette, modest highlights, centered large subject, no pedestal, no frame, no letters, no text, no UI, real transparent background. Square image, designed to remain recognizable at 64 pixels. Match the polished illustrated tower item style in the conversation.

### Water.png

One standalone fantasy inventory catalyst item icon for a wood-and-parchment tower defense backpack game: a luminous blue water orb with one curling splash. Clean hand-painted game illustration, thick readable silhouette, modest highlights, centered large subject, no pedestal, no frame, no letters, no text, no UI, real transparent background. Square image, designed to remain recognizable at 64 pixels. Match the polished illustrated tower item style in the conversation.

### Earth.png

One standalone fantasy inventory catalyst item icon for a wood-and-parchment tower defense backpack game: a chunky brown-gold crystal rock orb. Clean hand-painted game illustration, thick readable silhouette, modest highlights, centered large subject, no pedestal, no frame, no letters, no text, no UI, real transparent background. Square image, designed to remain recognizable at 64 pixels. Match the polished illustrated tower item style in the conversation.

### Air.png

One standalone fantasy inventory catalyst item icon for a wood-and-parchment tower defense backpack game: a pale cyan swirling wind orb. Clean hand-painted game illustration, thick readable silhouette, modest highlights, centered large subject, no pedestal, no frame, no letters, no text, no UI, real transparent background. Square image, designed to remain recognizable at 64 pixels. Match the polished illustrated tower item style in the conversation.
