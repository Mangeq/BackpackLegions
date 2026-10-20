# Five reference HUD and information panels

Applied natively to Assets/Scenes/GameScane.unity with HudReferenceBuilder. Existing economy, wave, preparation, hover detection and game-speed ownership are unchanged. Existing Unity UI Text fields stay present but visually disabled so their original Awake/name lookups and update calls continue to work. HudReferenceSkin mirrors those values into readable native TMP labels. Original speed buttons and speedHighlights arrays are retained; only the selected cell receives the gold face.

Panels:

- Item/tower/enemy tooltip: transparent exterior, preserved stone/blue inset, real native icon/name/rarity/body; header and corners are preserved by a sliced Sprite while long text can expand the body. InfoPanelUI keeps its existing screen-edge clamping and calls InfoReferenceSkin after generating the description.
- Gold: actual EconomyManager balance, including spend and reward updates.
- Wave: current/total counts from the original wave label.
- Speed: existing 1x/2x/3x callbacks, combat gating, pause lock and selected-state highlight.
- Preparation: real countdown or pause caption, hidden automatically in combat.

Left HUD uses compact bars and moves the existing Start and hearts controls down to avoid overlap. The timer stays at the top center. Tooltip remains cursor-driven. All exterior charcoal backgrounds were replaced with alpha transparency; panel interiors remain opaque.

Sixteen live checks passed; see results.json. Verified economy changes, native wave updates, countdown/pause state, original item content across all ItemData assets, screen-corner clamping, real Start/combat transition, each speed callback and highlight, EventSystem raycasts for all speed buttons and alpha exteriors. Screenshots: hud-preparation.png, hud-item-tooltip.png, hud-speed-3.png. Test gameplay was discarded on stopping; editor restored to Boot stopped and clean. before.unity.txt preserves the gameplay scene before this change.

Saved built-in image_gen derivative assets under Assets/Resources/UI/HudReference (Tooltip.png, Gold.png, Wave.png, Countdown.png, Speed.png). Native .asset Sprites use alpha bounds; Speed.png provides a neutral whole frame and reusable selected tile. Prompts:

## Tooltip

> Use case: background-extraction and precise-object-edit. This exact game UI is the edit target. Remove the exterior charcoal background to genuine transparent alpha; keep the central panel interior opaque. Preserve its stone, metal corner plates, frame, texture, spikes, decorations and the original art style, not a redesign. Keep this portrait tooltip card exactly the same. Erase all sample lettering (title, subtitle and body) and erase ONLY the wand icon from its small top-left inset, preserving the inset's dark blue empty interior and stone frame. The whole card is empty for native game data. Keep original full landscape canvas and the card in its original x277..752,y46..532 placement. No text, no crop, transparent outside the card.

## Gold

> Use case: background-extraction and precise-object-edit. This exact game UI is the edit target. Remove the exterior charcoal background to genuine transparent alpha; keep the central panel interior opaque. Preserve its stone, metal corner plates, frame, texture, spikes, decorations and the original art style, not a redesign. Keep the gold coin stack icon unchanged. Erase ONLY 850, reconstruct the dark stone interior beneath it. No other changes. Keep original full 1024:572 canvas composition and central panel placement. Transparent outside, blank right side for a live number.

## Wave

> Use case: background-extraction and precise-object-edit. This exact game UI is the edit target. Remove the exterior charcoal background to genuine transparent alpha; keep the central panel interior opaque. Preserve its stone, metal corner plates, frame, texture, spikes, decorations and the original art style, not a redesign. Keep horn, chain and sparks unchanged. Erase ONLY all sample WAVE: and 1/5 lettering, reconstruct dark stone beneath it. Keep original full 1024:572 composition and central panel placement. Transparent outside; empty right side for native live wave text.

## Countdown

> Use case: background-extraction and precise-object-edit. This exact game UI is the edit target. Remove the exterior charcoal background to genuine transparent alpha; keep the central panel interior opaque. Preserve its stone, metal corner plates, frame, texture, spikes, decorations and the original art style, not a redesign. Keep hourglass, chains, smoke and sparks intact. Erase ONLY all the sample Savaş 29 sn sonra başlıyor lettering, reconstruct dark stone beneath it. Keep original full 1024:572 composition and central panel placement. Transparent outside; empty right space for the live countdown.

## Speed

> Use case: background-extraction and precise-object-edit. This exact game UI is the edit target. Remove the exterior charcoal background to genuine transparent alpha; keep the central panel interior opaque. Preserve its stone, metal corner plates, frame, texture, spikes, decorations and the original art style, not a redesign. Prepare TWO independent sprite parts on one transparent square canvas, with large empty gutters between them. UPPER HALF: the complete original horizontal 3-cell speed panel with its frame/spikes, but all three cells dark and UNSELECTED, no orange/gold highlight, erase all 1x 2x 3x lettering. Preserve the three stone cells and separators. LOWER HALF: a separate isolated copy of only the original highlighted golden first button face with its ornamental gold rim and orange interior/glow, erase its 1x lettering, no outer big stone frame. This blank selected button tile will be reused over any of the three cells. Keep panel fully inside upper half and golden button fully inside lower half, separated by transparent gutters. No lettering or digits anywhere. Do not redesign these original parts.
