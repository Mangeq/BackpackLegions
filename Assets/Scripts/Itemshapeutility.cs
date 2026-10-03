using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item şekilleriyle ilgili saf matematiksel yardımcı metotlar.
/// Hiçbir Unity sahne referansı tutmaz, tamamen stateless'tır.
/// </summary>
public static class ItemShapeUtility
{
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