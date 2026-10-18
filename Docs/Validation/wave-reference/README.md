# Live next-wave panel

The existing WavePreviewUI hover behavior is retained. Its new WavePreviewSkin reads WaveRound.GetGroups and EnemyData, merges repeated enemy types, and displays names/counts, exact health, ground/air movement, elements, weaknesses/resistances, actual normal damage multiplier, boss escape warning and total enemy count. It follows UpcomingWaveIndex, including while another wave is running. Panel height adapts to the number of enemy types; no sample rows or sample values are displayed.

Assets/Resources/UI/WaveReference/Panel.png is the cleaned approved artwork. Built-in image generation prompt: preserve the exact stone frame, title swords, three row frames and faint landscape backgrounds, portrait wells and backpack icon; remove all sample wording, numbers and goblin portraits and reconstruct their underlying textures without redesign or layout changes. Unity Sprite assets provide the sliced outer frame, live row face and blank body cover.

The 30 portraits were rendered from the actual campaign prefabs with Unity PreviewRenderUtility and the same elemental tint as Enemy.Start. These prefabs currently use geometric placeholder models (including cubes); the UI faithfully represents those models instead of implying new goblin artwork was installed. Portraits and sprite references are persisted in the gameplay scene and do not need runtime editor APIs.

Tools/Validation/wave_reference_runtime.cs exercised all 45 campaign waves. Eighteen grouped checks passed; see results.json. Covers live values, all portraits, source-data immutability, row counts, duplicate-group aggregation, next-wave selection during combat, final-wave state, boss warning, hover close and pause hide. Unity console had no errors during final validation.

Saved scene: Assets/Scenes/GameScane.unity. Previous scene: before.unity.txt. Screenshots: first-wave.png and final-fourth-wave.png. Test changes to wave indices/countdown were Play Mode only and discarded. Editor startup was restored to Boot.unity.
