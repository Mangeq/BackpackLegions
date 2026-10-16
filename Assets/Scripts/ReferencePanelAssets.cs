using System.Collections.Generic;
using UnityEngine;

/// <summary>Original raster pieces, with no replacement theme or generated textures.</summary>
public static class ReferencePanelAssets
{
    static readonly Dictionary<string, Sprite> sprites=new Dictionary<string, Sprite>();
    public static Sprite Sprite(string name)
    {
        if(sprites.TryGetValue(name,out Sprite result)&&result!=null)return result;
        var texture=Resources.Load<Texture2D>("UI/TowerPanel/Reference/"+name);
        if(texture==null)throw new System.InvalidOperationException("Missing reference UI crop: "+name);
        result=UnityEngine.Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        result.name="ApprovedReference_"+name;sprites[name]=result;return result;
    }
    public static Sprite IconFor(ItemData item)
    {
        if(item==null)return null;
        if(item.element==ElementType.Normal)
        {
            switch(item.itemID)
            {
                case "v1_piercing_arrows":return Sprite("PiercingArrows");
                case "v1_scout_hood":return Sprite("ScoutHood");
                case "v1_archer_gloves":return Sprite("LeatherGloves");
                case "v1_ranger_boots":return Sprite("RangerBoots");
            }
        }
        return item.itemIcon;
    }
}
