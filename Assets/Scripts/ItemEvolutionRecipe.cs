using UnityEngine;

[CreateAssetMenu(fileName="EvolutionRecipe", menuName="Backpack/Evolution Recipe")]
public class ItemEvolutionRecipe : ScriptableObject
{
    public ItemData first;
    public ItemData second;
    public ItemData result;
    // İlk sürümün kuralı: bir başarılı dalga. Çok dalgalı ilerleme henüz desteklenmez.
}
