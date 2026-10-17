using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir kulenin backpack grid mantığı. Tamamen saf C# - MonoBehaviour DEĞİL,
/// FindObjectOfType ile aranmaz. Her Tower kendi Backpack instance'ını "new" ile
/// oluşturur ve sahiplenir (1 kule = 1 backpack, sahnede paylaşılan tek bir
/// manager yok).
///
/// Grid array'i ve item listesi burada TEK bir yerde, birlikte yönetiliyor -
/// Place/Remove dışında hiçbir yol bu ikisini değiştiremez, bu yüzden ikisinin
/// senkronsuz kalması (eski sistemdeki "aynı item iki kez eklendi" bug'ı) artık
/// mimari olarak imkansız.
/// </summary>
public class Backpack
{
    public int Width { get; }
    public int Height { get; }
    public bool IsLooseStorage { get; }

    private readonly PlacedItem[,] grid;
    private readonly List<PlacedItem> items = new List<PlacedItem>();

    public IReadOnlyList<PlacedItem> Items => items;

    public Backpack(int width, int height, bool looseStorage=false)
    {
        Width = width;
        Height = height;
        IsLooseStorage=looseStorage;
        grid = new PlacedItem[width, height];
    }

    /// <summary>Verilen item, verilen origin'e yerleşebilir mi? (sınır + çakışma kontrolü, L-şekli dahil)</summary>
    public bool CanPlace(ItemData data, Vector2Int origin, int rotation = 0)
    {
        if(data==null||data.itemShape==null||data.itemShape.Count==0)return false;
        if(IsLooseStorage)return true;
        foreach (Vector2Int offset in ItemShapeUtility.GetRotatedShape(data, rotation))
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;

            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;

            if (grid[x, y] != null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Yeni eşya oluşturur; sınır/çakışma kontrolü başarısızsa null döner.
    /// Mevcut eşyayı taşırken önce Remove, sonra PlaceExisting kullanılır.
    /// Böylece aynı eşya kimliği ve üzerine takılı parçalar korunur.
    /// </summary>
    public PlacedItem Place(ItemData data, Vector2Int origin, int rotation = 0)
    {
        var placed=new PlacedItem(data,origin,rotation);
        return PlaceExisting(placed,origin,rotation)?placed:null;
    }
    public bool PlaceExisting(PlacedItem item,Vector2Int origin,int rotation=0)
    {
        if(item==null||item.Bag!=null||item.Parent!=null||!CanPlace(item.Data,origin,rotation))return false;
        item.SetPlacement(origin,rotation);item.SetBag(this);
        if(!IsLooseStorage)foreach(var cell in item.GetOccupiedCells())grid[cell.x,cell.y]=item;
        items.Add(item);return true;
    }
    public bool Contains(PlacedItem item) => item!=null && item.Bag==this;
    public bool CanAttach(PlacedItem host,PlacedItem part)
    {
        if(host==null||part==null||host==part||!items.Contains(host)||part.Bag!=null||part.Parent!=null||part.Attachments.Count>0)return false;
        if(host.Data.synergyRecipes==null)return false;
        foreach(var recipe in host.Data.synergyRecipes){
            if(recipe==null||recipe.targetItem!=host.Data||!recipe.AllowsAttachment||!recipe.Accepts(part.Data))continue;
            int count=0;foreach(var existing in host.Attachments)if(recipe.Accepts(existing.Data))count++;
            if(count<Mathf.Max(1,recipe.maxActivations))return true;
        }
        return false;
    }
    public bool TryAttach(PlacedItem host,PlacedItem part)
    { if(!CanAttach(host,part))return false;host.Attach(part);return true; }

    /// <summary>Item'ı backpack'ten kaldırır - hem grid hücrelerini hem items listesini birlikte temizler.</summary>
    public void Remove(PlacedItem item)
    {
        if(item!=null && item.Parent!=null && item.Bag==this){item.Parent.Detach(item);return;}
        if (item == null || !items.Contains(item))
            return;

        if(!IsLooseStorage)foreach (Vector2Int cell in item.GetOccupiedCells())
            grid[cell.x, cell.y] = null;

        items.Remove(item);
        item.SetBag(null);
    }

    public PlacedItem GetItemAt(Vector2Int cell)
    {
        if(IsLooseStorage)return null;
        if (cell.x < 0 || cell.x >= Width || cell.y < 0 || cell.y >= Height)
            return null;

        return grid[cell.x, cell.y];
    }

    // Ana eşya dönüşümü: önce tüm hücreleri doğrula; başarısızlıkta eski kayıt değişmez.
    public bool TryReplace(PlacedItem original, ItemData replacement)
    {
        if (original == null || !items.Contains(original) || replacement == null
            || replacement.itemShape == null || replacement.itemShape.Count == 0) return false;
        foreach (var offset in ItemShapeUtility.GetRotatedShape(replacement, original.Rotation))
        {
            var cell = original.Origin + offset;
            if (cell.x < 0 || cell.x >= Width || cell.y < 0 || cell.y >= Height) return false;
            var occupant = grid[cell.x, cell.y];
            if (occupant != null && occupant != original) return false;
        }
        Remove(original);
        Place(replacement, original.Origin, original.Rotation);
        return true;
    }

    /// <summary>Verilen item'ın bounding box'ını grid'in tam ortasına yerleştirecek origin'i hesaplar.</summary>
    public Vector2Int GetCenterOrigin(ItemData data)
    {
        Vector2Int size = ItemShapeUtility.GetBoundingSize(data.itemShape);
        int startX = Mathf.Max(0, (Width - size.x) / 2);
        int startY = Mathf.Max(0, (Height - size.y) / 2);
        return new Vector2Int(startX, startY);
    }
}

