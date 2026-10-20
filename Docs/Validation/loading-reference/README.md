# Reference loading screen

Applied to `Assets/Resources/UI/SceneLoadingUI.prefab` and the inherited instance in `Assets/Scenes/Boot.unity`. Existing `SceneLoadingUI` async progress, failure, retry, scene activation and singleton cleanup are unchanged. `LoadingReferenceSkin` fits the 1024:565 composition with one uniform scale so the complete frame stays visible on different aspect ratios.

Saved artwork:

- `Assets/Resources/UI/LoadingReference/LoadingOriginal.png`: supplied image, unchanged.
- `Assets/Resources/UI/LoadingReference/LoadingEmpty.png`: background with empty chain and cleared status caption.
- `Assets/Resources/UI/LoadingReference/LoadingChain.png`: alpha sprite for the complete glowing chain. Builder creates a native Sprite rectangle around its visible alpha; Slider drives horizontal fill from zero to one.

The built-in image_gen tool made the two derivatives. Prompts:

> Use case: precise-object-edit. This is the exact loading-screen artwork to implement in a game, not a redesign. Change ONLY two tiny bottom details: erase the text BÖLÜM HAZIRLANIYOR at normalized x415..611 y487..507 (on a 1024x565 canvas), reconstruct the scenery underneath; and turn the orange glowing chain loading bar at x294..734 y512..537 completely dark/unlit, preserving its metal chain links, outline, and location. The whole loading bar should appear empty, a dark metal chain, with NO orange glow. Keep absolutely everything else unchanged: landscape, castle, forest, parchment, logo BACKPACK LEGIONS, all parchment Turkish words, full exterior metal frame/chains. Preserve exact layout, original 1024:565 aspect ratio and framing. No cropping or moving. Opaque image.

> Use case: background-extraction. Extract ONLY the small horizontal loading-chain bar from the bottom center of this reference into a transparent PNG sprite. Keep its original exact design: two rows of small metal chain links, rounded/tapered ends, fiery orange and yellow molten glow. Illuminate the ENTIRE bar (including its formerly dark right segment) so it represents 100% progress. No scenery, text, frame or other objects, no black rectangle. Make actual transparent alpha outside the chain and glow. Keep original full 1024:565 canvas ratio, bar located in its original x294..734 y510..538 position with fully transparent padding everywhere else. Do not enlarge the bar to fill the canvas; preserve its original location and dimensions.

Eight runtime checks passed; see results.json. Actual Boot -> MainMenu, failed-load retry -> GameScane and GameScane -> MainMenu transitions completed and removed the persistent loader correctly. Native slider values 0, 0.5 and 1 matched Image.fillAmount. Screenshots: loading-75.png and loading-error.png. Editor restored to the original gameplay scene stopped, with its pre-existing dirty state and editor start-scene preference preserved. No skill/player rewards were modified.
