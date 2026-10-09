using System.Collections.Generic;
using UnityEngine;

// Her kuleye ait işlem durumu; global envanter veya sahne yöneticisi değildir.
public class TowerItemEvolution
{
    public sealed class Pair
    {
        public PlacedItem first, second;
        public ItemEvolutionRecipe recipe;
    }
    public sealed class Delivery
    {
        public ItemData Item { get; private set; }
        public Delivery(ItemData item) { Item=item; }
    }
    private readonly Tower owner;
    private readonly List<Delivery> deliveries=new List<Delivery>();
    private List<Pair> pending=new List<Pair>();
    private int activeWave=-1;
    public IReadOnlyList<Delivery> Deliveries => deliveries;
    public IReadOnlyList<Pair> Pending => pending;
    public TowerItemEvolution(Tower tower) { owner=tower; }
    public bool Contains(Delivery delivery) => deliveries.Contains(delivery);
    public bool Claim(Delivery delivery) => deliveries.Remove(delivery);
    internal void TransferDeliveriesTo(TowerItemEvolution destination)
    {
        if (destination == null || destination == this) return;
        destination.deliveries.AddRange(deliveries);
        deliveries.Clear();
        CancelWave();
    }

    public List<Pair> FindPairs()
    {
        var result=new List<Pair>();
        if(owner.Backpack==null || owner.evolutionRecipes==null) return result;
        var sorted=new List<PlacedItem>(owner.Backpack.Items);
        sorted.Sort((a,b)=> { int c=a.Origin.y.CompareTo(b.Origin.y);return c!=0?c:a.Origin.x.CompareTo(b.Origin.x); });
        var used=new HashSet<PlacedItem>();
        foreach(var recipe in owner.evolutionRecipes)
        {
            if(recipe==null || recipe.first==null || recipe.second==null || recipe.result==null ||
                owner.IsMainItem(recipe.first) || owner.IsMainItem(recipe.second) || owner.IsMainItem(recipe.result)) continue;
            if (!owner.IsItemActive(recipe.first) || !owner.IsItemActive(recipe.second) || !owner.IsItemActive(recipe.result)) continue;
            foreach(var a in sorted)
            {
                if(used.Contains(a)||a.Data!=recipe.first) continue;
                foreach(var b in sorted)
                {
                    if(a==b || used.Contains(b)||b.Data!=recipe.second || !Adjacent(a,b)) continue;
                    result.Add(new Pair{first=a,second=b,recipe=recipe});used.Add(a);used.Add(b);break;
                }
            }
        }
        return result;
    }
    public static bool Adjacent(PlacedItem a, PlacedItem b)
    {
        foreach(var x in a.GetOccupiedCells()) foreach(var y in b.GetOccupiedCells())
            if(Mathf.Abs(x.x-y.x)+Mathf.Abs(x.y-y.y)==1)return true;
        return false;
    }
    public void BeginWave(int wave)
    {
        if(activeWave==wave)return;
        pending=FindPairs();activeWave=wave;
    }
    public void CancelWave() { pending.Clear();activeWave=-1; }
    public void CompleteWave(int wave)
    {
        if(activeWave!=wave)return;
        var work=pending;pending=new List<Pair>();activeWave=-1;
        foreach(var pair in work)
        {
            if(!ContainsItem(pair.first)||!ContainsItem(pair.second)||!Adjacent(pair.first,pair.second)
                || !owner.IsItemActive(pair.first.Data) || !owner.IsItemActive(pair.second.Data) || !owner.IsItemActive(pair.recipe.result))continue;
            owner.Backpack.Remove(pair.first);owner.Backpack.Remove(pair.second);
            if(!TryPlace(owner.Backpack,pair.recipe.result,pair.first.Origin) &&
                !TryPlace(owner.Storage,pair.recipe.result,Vector2Int.zero))
                deliveries.Add(new Delivery(pair.recipe.result));
        }
        owner.CalculateStats();
        var ui=BackpackUIManager.Instance;
        if(ui!=null && ui.ActiveTower==owner)ui.RefreshItemVisuals();
    }
    private bool ContainsItem(PlacedItem item)
    {
        foreach(var p in owner.Backpack.Items)if(p==item)return true;
        return false;
    }
    private static bool TryPlace(Backpack bag, ItemData item, Vector2Int preferred)
    {
        for(int r=0;r<4;r++)if(bag.CanPlace(item,preferred,r)){bag.Place(item,preferred,r);return true;}
        for(int y=0;y<bag.Height;y++)for(int x=0;x<bag.Width;x++)for(int r=0;r<4;r++)
        {
            var pos=new Vector2Int(x,y);
            if(bag.CanPlace(item,pos,r)){bag.Place(item,pos,r);return true;}
        }
        return false;
    }
}
