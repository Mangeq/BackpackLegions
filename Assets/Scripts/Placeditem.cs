using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bir Backpack içinde o an yerleşik olan tek bir item'ın runtime kaydı.
/// ItemData (ScriptableObject, paylaşılan tasarım verisi) ile pozisyon (runtime state)
/// burada bilinçli olarak ayrılır - ItemData asla mutasyona uğramaz, bu yüzden aynı
/// asset'i paylaşan başka kuleler/itemler birbirini etkilemez.
/// </summary>
public class PlacedItem
{
    public ItemData Data { get; }
    public Vector2Int Origin { get; private set; }
    public int Rotation { get; private set; }
    public Backpack Bag { get; private set; }
    public PlacedItem Parent { get; private set; }
    private readonly List<PlacedItem> attachments = new List<PlacedItem>();
    public IReadOnlyList<PlacedItem> Attachments => attachments;
    internal void SetBag(Backpack bag) { Bag=bag;foreach(var part in attachments)part.SetBag(bag); }
    internal void SetPlacement(Vector2Int origin,int rotation) { Origin=origin;Rotation=ItemShapeUtility.NormalizeRotation(rotation); }
    internal void Attach(PlacedItem part) { attachments.Add(part);part.Parent=this;part.SetBag(Bag); }
    internal void Detach(PlacedItem part) { if(!attachments.Remove(part))return;part.Parent=null;part.SetBag(null); }

    public PlacedItem(ItemData data, Vector2Int origin, int rotation = 0)
    {
        Data = data;
        Origin = origin;
        Rotation = ItemShapeUtility.NormalizeRotation(rotation);
    }

    /// <summary>Bu item'ın grid üzerinde kapladığı tüm hücreler (grid koordinatında).</summary>
    public IEnumerable<Vector2Int> GetOccupiedCells()
    {
        foreach (Vector2Int offset in ItemShapeUtility.GetRotatedShape(Data, Rotation))
            yield return Origin + offset;
    }

    public Vector2Int BoundingSize => ItemShapeUtility.GetRotatedSize(Data, Rotation);

    /// <summary>internal: sadece Backpack sınıfı çağırabilir, dışarıdan pozisyon değiştirmek yasak.</summary>
    internal void MoveTo(Vector2Int newOrigin)
    {
        Origin = newOrigin;
    }
}
