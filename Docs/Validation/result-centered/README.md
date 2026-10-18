# Centered result and pause panels

The exterior charcoal backgrounds are replaced with actual alpha transparency in the three Cutout.png assets under Assets/Resources/UI/ResultReference. Generated using the built-in image_gen tool; the exact prompt is in results.json. Panel interiors, decorations and captions remain part of the artwork. Buttons use matching crops from those same cutouts; the existing final-campaign blank button stays inside the opaque panel interior.

ResultReferenceSkin fits each image with one uniform scale and an 80% screen-height limit, centered on its visible artwork. The gameplay world remains visible around the frame. ResultReferenceBuilder saves the persistent sprites and scene references before play.

Verified all three screens visually in Play Mode (pause.png, defeat.png, victory.png), thirteen existing game-state/callback checks and eleven native EventSystem button raycasts. Runtime rewards were backed up and restored by the validation script. Panels saved closed, GameManager enabled, editor stopped, Boot restored as start scene. before.unity.txt backs up the scene before this correction.
