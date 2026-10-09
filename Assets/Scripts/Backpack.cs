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

    private readonly PlacedItem[,] grid;
    private readonly List<PlacedItem> items = new List<PlacedItem>();

    public IReadOnlyList<PlacedItem> Items => items;

    public Backpack(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new PlacedItem[width, height];
    }

    /// <summary>Verilen item, verilen origin'e yerleşebilir mi? (sınır + çakışma kontrolü, L-şekli dahil)</summary>
    public bool CanPlace(ItemData data, Vector2Int origin, int rotation = 0)
    {
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
    /// Item'ı yerleştirir. Bilinçli olarak CanPlace'i içeride otomatik çağırmıyor -
    /// "taşıma" senaryosunda çağıran taraf önce Remove ile eski pozisyonu boşaltıp
    /// SONRA CanPlace kontrol etmesi gerekiyor (yoksa item kendi eski hücresine
    /// çarpar). Bu yüzden check + act adımları caller'a bırakıldı.
    /// </summary>
    public PlacedItem Place(ItemData data, Vector2Int origin, int rotation = 0)
    {
        PlacedItem placed = new PlacedItem(data, origin, rotation);

        foreach (Vector2Int cell in placed.GetOccupiedCells())
            grid[cell.x, cell.y] = placed;

        items.Add(placed);
        return placed;
    }

    /// <summary>Item'ı backpack'ten kaldırır - hem grid hücrelerini hem items listesini birlikte temizler.</summary>
    public void Remove(PlacedItem item)
    {
        if (item == null || !items.Contains(item))
            return;

        foreach (Vector2Int cell in item.GetOccupiedCells())
            grid[cell.x, cell.y] = null;

        items.Remove(item);
    }

    public PlacedItem GetItemAt(Vector2Int cell)
    {
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

