# Approved victory, defeat and pause panels

Applied natively to Assets/Scenes/GameScane.unity with ResultReferenceBuilder. Existing panel objects, button names, callbacks, state changes and rewards are retained. ResultReferenceSkin changes only presentation. All panels are saved closed with persistent Sprite references, before Play Mode.

Assets/Resources/UI/ResultReference/Pause.png and Victory.png are unchanged copies of the supplied artwork. The native buttons use matching Sprite rectangles, with all old captions and old graphic panels hidden. The defeat banner reads the current ChapterData. At campaign completion the next-chapter button displays the completion message and remains disabled.

The built-in image generation tool made two derivative assets:

- Defeat.png prompt: remove only the sample chapter wording from the chapter banner and reconstruct the dark underlying texture; preserve the banner emblem, title, crest/smoke, stone frame, subtitle, three buttons and full composition.
- VictoryClean.png prompt: remove only the next-chapter lettering from the upper gold button, reconstructing its blank inner face while preserving its border, all other wording, decoration and composition. Only that blank button crop is used for the campaign-complete state; ordinary victory uses the unchanged supplied image.

Tools/Validation/result_reference_runtime.cs passed the state/callback checks, including pause/continue, settings and skill subpanels, separate victory/defeat visibility, actual chapter label and the final-chapter state. Player skill preferences were backed up and restored within the validation call. After each modal had rendered, real EventSystem raycasts passed for all eleven illustrated buttons. Additional native clicks loaded chapter two, retried chapter two and returned to MainMenu. See results.json.

Verified screenshots: pause-final.png, defeat-verified.png, victory-final.png. Earlier check captures are intermediate. before.unity.txt preserves the scene including the user's pre-existing unsaved changes, which were saved before the UI edit. Runtime test results/rewards were discarded/restored; the editor is stopped with GameManager enabled, panels closed, scene saved, and Boot.unity restored as the editor start scene.
