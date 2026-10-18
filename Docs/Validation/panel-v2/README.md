# Approved panel and dedicated element shop

Applied to Assets/Scenes/GameScane.unity using ApprovedUISceneBuilder; all visual references persist before Play Mode. Runtime size remains 1600 x 930 at the 1920 x 1080 reference resolution.

The approved source is approved.png. Runtime art is Assets/Resources/UI/TowerPanel/Reference/PanelV2.png. The built-in image generation tool removed only sample items, numbers and dynamic captions, preserving the stone, wood, shelves, grid and buttons. Individual controls use Sprite rectangles from that texture, rather than an alternate theme. UI/PanelSelection desaturates the current targeting choice; targeting text remains independent.

Art prompt: Create an implementation-ready clean background from the approved panel. Preserve exact composition, frame, stone, wood, inventory/depot grid lines, shelves, static headings and labels, buttons and wording. Remove inventory/shop illustrations, prices, tower title, numerical stats and breakdowns, XP text/fill, shop subtitle, and targeting option text. Make target faces unselected brown. Keep the XP frame and target decorations. Fill cleared areas with the original underlying texture; no redesign or layout change.

Storage uses StorageGridUI with two columns, six visible rows and one fitted icon per item. Original item shapes and rotations are retained for returning to the backpack. Vertical scrolling is enabled only above twelve entries. No physics component remains in the saved hierarchy.

Tower owns the element shop session. Level five unlocks it once. Four random catalyst offers may repeat; paid rerolls can find a missing element. Successful purchase requires a backpack drop, closes the session and restores ordinary offers/reservations. Later level ups and normal rerolls do not reopen it. Element change explicitly opens another session. Element offers are not refreshed for free between waves. Existing edge-adjacent main-weapon fusion is retained.

Tools/Validation/panel_v2_runtime.cs exercised the actual scene, buttons, drag handlers, economy, storage and shop state in Play Mode. All 29 checks passed; see results.json. The final screenshot also verifies a second element-shop purchase immediately fuses an adjacent Earth catalyst and returns to the normal shop.

Screenshots: final-element-shop.png and final-normal-shop.png. before.unity.txt preserves the scene before this change. Validation towers, test gold and paused countdown were confined to Play Mode and were discarded on exit.
