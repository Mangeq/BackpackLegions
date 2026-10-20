# Reference chapter-selection map

Applied to Assets/Scenes/MainMenu.unity with ChapterReferenceBuilder. The existing five ChapterData assets and MainMenuManager button callbacks are retained. MainMenuManager.SelectChapter delegates presentation to ChapterReferenceSkin when present. Back still invokes ShowHome; OYNA still invokes PlaySelectedChapter and SceneLoadingUI.

Native UI consists of five selectable numbered markers, independent saved-result stars, dynamic parchment labels and chapter details. Selected marker uses a gold-backed shield. Right panel reads wave count, actual enemy count and ground/air movement, preparation timings, starting gold and saved best result. Top star count is the sum of saved chapter results out of 15; the coin-style counter explicitly displays available talent points as YP. No example currency, enemy list, chapter names or future locked levels are presented as real game data. No new unlocking restrictions were introduced.

Image assets under Assets/Resources/UI/ChapterReference:

- Original.png: unchanged supplied reference.
- ChapterMap.png: clean map with dynamic fields removed.
- ChapterParts.png: transparent marker, selected shield, parchment label and star sprites.

Derived artwork generated with the built-in image_gen tool. Prompts:

> Use case: precise-object-edit. Edit this exact chapter-selection UI artwork, preserve the full original 1024:572 composition and style. Prepare a clean background for real interactive game UI: remove ALL eight map level markers (round medallions, shields, locks, stars and the golden glow around the selected shield), and ALL their parchment name labels; reconstruct terrain and dotted roads beneath these so the map is continuous. Clear ONLY changing data from the right wood information panel: remove its heading, chapter name, three stars, all five text rows AND their small row icons; leave a completely empty opaque wood interior, keep its metal/stone frame, vines, and OYNA button unchanged. At top-right remove only the example text 11/30 and 150, keep star and coin icons and counter dark backing intact. Keep BÖLÜM SEÇİMİ, GERİ button, OYNA button, detailed forest, mountains, river, castles, campfire, map parchment, torches, chains and outer frame exactly intact. Do not redesign, shift, crop, add levels, add text or change colors. Landscape full canvas, opaque.

> Use case: background-extraction / game UI sprite sheet. Derive four separate reusable cutouts from this exact reference design, faithfully matching its hand-painted metal and parchment textures. Transparent PNG, square 1024x1024 canvas, 2x2 grid of equal 512px cells with plenty of transparent gutters; keep every component wholly inside its own cell, no overlap. TOP LEFT: one round metal-and-brown chapter medallion from level 1 or 2, with matching crossed metal details, erase its numeral, NO stars, NO text, NO scenery. TOP RIGHT: the selected silver/bronze shield from level 5, including its little sword/crossed weapons and soft golden selection glow, erase numeral 5, NO text, NO stars, NO scenery. BOTTOM LEFT: one blank horizontal tan parchment nameplate matching the chapter labels, width approximately400,height80 within the cell, erase all lettering. BOTTOM RIGHT: one small gold five-point completion star matching the reference, centered in its cell. These are four independent isolated transparent sprites, not a new UI mockup. No words, no digits, no new decorations. Preserve original colors/material/style. Actual transparent alpha outside components.

The Editor builder creates native Sprite assets using source rectangles and alpha bounds. The canvas uses a uniform scale to preserve the complete original 1024:572 frame. Scene saved with the chapter panel closed.

Eleven checks passed: initial visibility, native opening, each of five chapter selections, all seven visible button raycasts, saved star/talent counters, Back and real fifth-chapter loading. See results.json. Verified screenshots chapter-1.png and chapter-5.png. Player progress was read but never changed. Original Boot scene restored stopped and clean. before.unity.txt backs up the previous MainMenu scene.
