# Backpack Legions — Blackpine UI Kit

Blender-authored UI concept kit, designed at 1920 × 1080 in Turkish.

## Screens

1. `01_Loading.png`: crest, game title, progress bar and gameplay tip.
2. `02_Main_Menu.png`: continue, new campaign, settings and exit. The requested entrance screen is interpreted as the main menu, not account authentication.
3. `03_Chapter_Map.png`: parchment campaign map, completed/current/locked nodes, chapter information, rewards and navigation.
4. `04_Settings.png`: audio tab, sliders, toggle, defaults and save actions. Other tabs are visual navigation placeholders.

The `.blend` contains separate scenes for these screens and all components. Text remains editable Blender font objects; map routes and ornaments are editable geometry/curves. Colours are named materials. Screen images are presentation renders, not functioning Unity canvases.

## Reusable components

Transparent PNGs under `Assets/Art/BlackpineUIKit/Textures`:

- Button_Primary, Button_Secondary (text-free)
- Panel_Frame
- Shield_Badge, Star_Gold, Diamond_Currency
- Progress_Track, Progress_Fill
- Toggle_On, Toggle_Off
- Slider_Knob, Node_Selected, Icon_Settings

Keep labels as separate TextMeshPro text when implementing. Use Sliced images for frames/buttons, Simple for icons, and Filled for the progress fill. Base target is landscape 16:9. Runtime button behaviours, responsive layout, save settings and scene transitions are not implemented by this art kit.

Palette: deep forest #111C1B, panel #192724, muted gold #C8A66B, highlight #F0D399, ivory #ECE4CE, green #537C59.

Fonts: Georgia / Georgia Bold for headings, Bahnschrift for controls. Fonts are loaded from the local Windows installation and are not redistributed in this kit. Replace with appropriately licensed project fonts when shipping.

Rebuild source: `Tools/Environment/build_ui_kit.py` (Blender Python).
