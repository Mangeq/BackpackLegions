using TMPro;
using UnityEngine;

public static class GameFonts
{
    public static TMP_FontAsset Title { get { return Resources.Load<TMP_FontAsset>("UI/Fonts/CinzelBold SDF"); } }
    public static TMP_FontAsset Body { get { return Resources.Load<TMP_FontAsset>("UI/Fonts/AlegreyaMedium SDF"); } }
    public static Font TitleSource { get { return Resources.Load<Font>("UI/Fonts/Cinzel-Bold"); } }
    public static Font BodySource { get { return Resources.Load<Font>("UI/Fonts/Alegreya-Medium"); } }
}
