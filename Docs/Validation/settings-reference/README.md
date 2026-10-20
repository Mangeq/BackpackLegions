# Reference settings panel

The supplied design is applied to Assets/Prefabs/UI/SettingsUI.prefab and the SettingsPanelUI instances in MainMenu.unity and GameScane.unity. SettingsReferenceSkin fits the whole 1024:874 card at the screen center, leaving a visible game/menu border. Existing native close/cancel/apply and resolution/quality button callbacks remain intact. Language arrows are visibly disabled and have no button or localization behavior.

Implemented settings:

- Master volume uses AudioListener.volume; SFX has an independent persistent SoundEffectsVolume gain that preserves each AudioSource's authored loudness without compounding and also handles new scene sources. Existing Music mixer groups are exempt from effects gain. The current project has no active music sources.
- Resolution choices deduplicate monitor-supported Screen.resolutions plus current window size, with a fallback list. Screen.SetResolution applies the selected size and window mode; Windows players use exclusive fullscreen for selectable fullscreen resolution.
- Quality applies Unity's existing Performant/Balanced/High Fidelity levels, translated as PERFORMANS/DENGELİ/YÜKSEK.
- FPS choices: 30, 60, 90, 120, 144, 165, 240, unlimited. VSync disables the FPS arrows and shows VSYNC; Apply sets vSyncCount and clears the conflicting application frame cap.
- All edits are draft-only until Apply. Cancel, X, outside dismissal and Escape discard the draft. Apply saves BL.Settings values and closes. Saved values reapply before scene load; the existing saved volume/display/quality keys are retained.

Seventeen checks passed in the live Unity Editor; see results.json. Tests covered native percent captions, draft cancellation, resolution/quality arrows, eight FPS choices, VSync arbitration, all nine enabled button raycasts, actual slider pointer input, Apply engine state and preferences, noncompounding effects gain on an AudioSource, saved-settings startup replay, a real menu-to-game scene transition and paused-game open/close behavior. Fullscreen mode and resolution calls were checked through their native API path and saved selections; physical monitor switching requires a standalone player because Unity Editor owns its Game View. The test backed up and restored all eight settings preference keys and engine quality/volume/frame state. No campaign/skill rewards were modified. Editor stopped at the original Boot scene, clean.

Saved images under Assets/Resources/UI/SettingsReference:

- Original.png: unchanged supplied reference.
- SettingsEmpty.png: matching panel with dynamic values, slider knobs and checkbox tick cleared.
- SettingsParts.png: transparent bronze knob and gold checkmark cutouts.

Derivative artwork used the built-in image_gen tool. Prompts:

> Use case: precise-object-edit. Edit this exact settings panel for a game; preserve original 1024:874 composition and every texture, frame, divider, title and static label unchanged. Clear only dynamic values: erase the 71% and 55% numbers to the right of the two volume sliders; remove their circular draggable knobs while retaining empty slider tracks; remove 1920 × 1080, DENGELİ and 60 from the three lower selector boxes, leaving clean dark blank interiors; remove the gold check from the VSync checkbox leaving both checkbox sockets empty. Preserve ALL other words exactly, including TÜRKÇE, AYARLAR, labels, VAZGEÇ and UYGULA, and preserve all arrow buttons, X close button, language row, metal/wood textures. Do not redesign, move, rescale, crop, add labels or alter the framing. Opaque full canvas. These cleared values will be native interactive UI, not baked into the background.

> Use case: background-extraction. Create two faithful transparent cutouts from this settings reference, for native Unity UI controls. On a transparent 1024x512 canvas put TWO isolated components, one centered in each 512x512 half with large transparent gutters: LEFT the small round bronze/copper slider thumb knob visible on the volume sliders, preserve its round beveled metal rim and darker brown center, no track or background. RIGHT the gold glowing checkmark from the VSync checkbox, preserve the tick's warm golden light and shape, but remove its checkbox socket and all surrounding background. No text, no other objects, no redesign. Actual transparent alpha outside the two small components.

Verified screenshots: settings-menu.png and settings-game.png. before.prefab.txt, before-menu.unity.txt and before-game.unity.txt preserve previous UI assets.
