# Reference main menu and skill tree

Both Assets/Scenes/MainMenu.unity and Assets/Scenes/GameScane.unity use the approved frontend presentation. The shared Assets/Prefabs/UI/SkillTreeUI.prefab is updated too. Changes are persisted before Play Mode; no runtime-only skin installation is required.

Assets/Resources/UI/FrontendReference/MainMenu.png is an unchanged copy of the supplied main-menu image. Native Play, Skill Tree, Settings and Quit buttons retain their callbacks and use matching Sprite rectangles over the artwork. Chapter selection and settings behavior are unchanged.

SkillTreeReferenceSkin presents existing SkillTreeData and SkillTreeProgress. Node coordinates continue to come from the data assets, and all nodes remain available in all four tabs. Only presentation hooks were added to SkillTreePanelUI. Selecting a node updates its frame, live icon, title, description and unlock conditions. Skill saves and point costs were not changed.

Project art files:

- Assets/Resources/UI/FrontendReference/SkillPieces.png: clean source pieces from the approved design.
- Assets/Resources/UI/FrontendReference/SkillBackground.png: tree/background with movable node cards removed.
- Assets/Resources/UI/FrontendReference/NodeFaces.png: neutral and selected frames with real transparent corners.
- Assets/Resources/UI/FrontendReference/Sprites/: persistent Sprite assets sliced by the Unity Editor; no handwritten metadata changes.

The built-in image generation tool was used for these three derivative images. Prompts:

1. Preserve the exact stone border, golden tree, decorative circular medallions, sidebar frames, detail panel, dividers and static headings. Remove only dynamic node text/icons, branch heading/subtitle, point count and dynamic detail fields so live Unity controls can draw them.
2. Remove only the eleven rectangular node cards from that clean texture, reconstructing the golden tree underneath. Preserve the rest of the layout and decoration unchanged.
3. Extract the neutral stone node frame and selected gold/green node frame as two side-by-side blank sprites with transparent outer corners. Preserve the original texture/style and remove surrounding branches, text and icons.

Tools/Validation/frontend_reference_runtime.cs passed 31 navigation and data-binding checks (results.json), including actual EventSystem raycasts at all four illustrated menu buttons, all four tabs and their complete node lists, node/detail selection, unlock eligibility, settings, chapter navigation and unchanged player skill-save contents. The gameplay scene was separately inspected for a persistent new skin and closed modal.

Screenshots: main-menu.png and final-archer-tree.png. Earlier check images are intermediate validation. Scene and prefab contents from before this work are kept in *.before.txt. Main-menu art remains exactly the user-provided file; skill artwork was separated into live pieces to avoid frozen text, selections or example point values.
