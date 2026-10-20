# Reference Start Game button

Replaced the existing StartBTN in Assets/Scenes/GameScane.unity. Existing onClick callbacks and preparation/combat/pause gating are unchanged. The native button Image uses Assets/Resources/UI/HudReference/StartGame.png through StartGame.asset; old label and graphics are hidden. Preserve-aspect rendering keeps the supplied art proportions. Speed and hearts were moved down slightly to avoid overlap. HudReferenceBuilder reapplies the same style on later HUD rebuilds.

Verified the rendered button receives the real EventSystem hit, and a native pointer click starts combat, disables Start and closes preparation countdown. See results.json and start-button.png. Editor returned stopped to its original scene/start preference. before.unity.txt preserves the previous scene.

Built-in image_gen removed only the outside background. Prompt:

> Use case: background-extraction. Remove ONLY the exterior flat dark charcoal background around this exact Start Game button, output genuine transparent alpha outside the irregular frame and in gaps around chains. Keep the entire central button interior opaque. Preserve the exact Start Game lettering, sword/shield emblem, gold/orange textured face, stone frame, metal corner plates, chains, hook, spikes, sparks and translucent smoke. No redesign, retyping, recoloring, rescaling or moving of any element. Keep the original full 1024:572 landscape canvas and same central placement with transparent padding.
