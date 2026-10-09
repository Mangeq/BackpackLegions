using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item şekilleriyle ilgili saf matematiksel yardımcı metotlar.
/// Hiçbir Unity sahne referansı tutmaz, tamamen stateless'tır.
/// </summary>
public static class ItemShapeUtility
{
    public static int NormalizeRotation(int turns) => ((turns % 4) + 4) % 4;

    // Grid Y aşağı doğru artar; bu dönüş ekranda saat yönündedir.
    public static Vector2Int RotateCell(Vector2Int cell, Vector2Int size, int turns)
    {
        for (int i = 0; i < NormalizeRotation(turns); i++)
        {
            cell = new Vector2Int(size.y - 1 - cell.y, cell.x);
            size = new Vector2Int(size.y, size.x);
        }
        return cell;
    }

    public static IEnumerable<Vector2Int> GetRotatedShape(ItemData data, int turns)
    {
        Vector2Int size = GetBoundingSize(data.itemShape);
        foreach (var cell in data.itemShape) yield return RotateCell(cell, size, turns);
    }

    public static Vector2Int GetRotatedSize(ItemData data, int turns)
    {
        Vector2Int size = GetBoundingSize(data.itemShape);
        return NormalizeRotation(turns) % 2 == 0 ? size : new Vector2Int(size.y, size.x);
    }

    /// <summary>
    /// Verilen şeklin (item'in kendi orijinine göre, (0,0) sol-üst olacak şekilde
    /// tanımlanmış hücrelerin) kapladığı dikdörtgen bounding box'ı döndürür.
    /// L-şekilli itemlerde de doğru çalışır çünkü sadece en büyük x/y'ye bakar -
    /// item'ın kendisi dikdörtgen olmasa bile görsel boyutlandırma için
    /// dikdörtgen bir alan gerekir (Backpack Battles'daki gibi).
    /// </summary>
    public static Vector2Int GetBoundingSize(IReadOnlyList<Vector2Int> shape)
    {
        int maxX = 0;
        int maxY = 0;

        for (int i = 0; i < shape.Count; i++)
        {
            if (shape[i].x > maxX) maxX = shape[i].x;
            if (shape[i].y > maxY) maxY = shape[i].y;
        }

        return new Vector2Int(maxX + 1, maxY + 1);
    }
}
