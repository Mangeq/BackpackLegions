# Transparent arched tower selection buttons

Final project assets (built-in imagegen edits of the existing textures):

- `Assets/Resources/UI/TowerPurchase/Reference/MagicTabArch.png`
- `Assets/Resources/UI/TowerPurchase/Reference/MortarTabArch.png`
- `Assets/Resources/UI/TowerPurchase/Reference/ArcherTabArch.png`
- `Assets/Resources/UI/TowerPurchase/Reference/UnitTabArch.png`

All four use an arched iron frame, including the unit button. Exterior upper corner alpha samples at 2%/98% x and 2% y, and 10%/90% x and 5% y, are zero for every sprite. Interior backgrounds remain opaque. Unity imports use input alpha, alpha-is-transparency, no mipmaps, uncompressed textures and Clamp wrapping.

The old square selected-rim graphic stays disabled. A UI Outline follows each sprite's alpha silhouette and is enabled only for the actual selection. Original selection and purchase callbacks remain connected. Five Play Mode checks passed: opening has no highlight and all four types load their arched sprite and enable only their matching outline. `transparent-arches.png` is the verified game-view capture. No Unity console errors.

## Final prompts

### MagicTabArch.png

Precise background extraction edit of this existing standalone game selection button. Preserve the EXACT existing metal arch silhouette, tower icon, lettering, positions, colors and all details INSIDE the metal border. Change ONLY pixels OUTSIDE the OUTER metal border to genuine alpha transparency: especially the large upper left and upper right corners around the arch, plus thin margins on left/right/bottom. Black INSIDE the arch remains opaque black. No black rectangle outside the arched frame, no background, no cast shadow outside, no added padding, no redesign, no crop into metal. Output sharp high resolution about 720 pixels wide, same aspect ratio. Keep the entire original metal edge intact, tightly cropped standalone game UI sprite.

### MortarTabArch.png

Precise background extraction edit of this existing standalone game selection button. Preserve the EXACT existing metal arch silhouette, tower icon, lettering, positions, colors and all details INSIDE the metal border. Change ONLY pixels OUTSIDE the OUTER metal border to genuine alpha transparency: especially the large upper left and upper right corners around the arch, plus thin margins on left/right/bottom. Black INSIDE the arch remains opaque black. No black rectangle outside the arched frame, no background, no cast shadow outside, no added padding, no redesign, no crop into metal. Output sharp high resolution about 720 pixels wide, same aspect ratio. Keep the entire original metal edge intact, tightly cropped standalone game UI sprite.

### ArcherTabArch.png

Precise background extraction edit of this existing standalone game selection button. Preserve the EXACT existing metal arch silhouette, tower icon, lettering, positions, colors and all details INSIDE the metal border. Change ONLY pixels OUTSIDE the OUTER metal border to genuine alpha transparency: especially the large upper left and upper right corners around the arch, plus thin margins on left/right/bottom. Black INSIDE the arch remains opaque black. No black rectangle outside the arched frame, no background, no cast shadow outside, no added padding, no redesign, no crop into metal. Output sharp high resolution about 720 pixels wide, same aspect ratio. Keep the entire original metal edge intact, tightly cropped standalone game UI sprite.

### UnitTabArch.png

Edit target Image 1: BİRLİK unit tower selection tab. Image 2 is the exact frame/style template. Replace ONLY the square frame/background of Image 1 with the SAME narrow gray iron arch-top frame, flat bottom, proportions, black interior and clean illustrated quality of Image 2. Keep unit tower orange conical hat, silver round armor body, orange circular pedestal and exact BİRLİK text, in same icon and label areas as template. Sharpen unit icon to match the clean illustrated rendering of Image 2. Neutral unselected iron, no gold. ONE standalone game button, about 720x928 pixels. IMPORTANT: genuine alpha transparency OUTSIDE the metal silhouette, including both large upper corners of the arch and the thin exterior margins. Black only INSIDE the arch. No rectangular black backdrop, no shadows outside, no padding, no checkerboard. Preserve original unit identity, no additional items.
