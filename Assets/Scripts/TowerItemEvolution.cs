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
        public ItemData Item => Content.Data;
        public PlacedItem Content { get; private set; }
        public Delivery(ItemData item) { Content=new PlacedItem(item,Vector2Int.zero,0); }
        public Delivery(PlacedItem item) { Content=item; }
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
    public void RestoreDelivery(Delivery delivery) { if(delivery!=null&&!deliveries.Contains(delivery))deliveries.Add(delivery); }
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
        var evolved=new List<PlacedItem>();
        var ui=BackpackUIManager.Instance;
        foreach(var pair in work)
        {
            if(!ContainsItem(pair.first)||!ContainsItem(pair.second)||!Adjacent(pair.first,pair.second)
                || !owner.IsItemActive(pair.first.Data) || !owner.IsItemActive(pair.second.Data) || !owner.IsItemActive(pair.recipe.result))continue;
            if(ui && ui.ActiveTower==owner)ui.EvolutionMotion(pair.first,pair.second);
            LegionFx.Ring(owner.transform.position,.65f,LegionFx.Gold,.55f,true);
            ReleaseAttachments(pair.first);ReleaseAttachments(pair.second);
            owner.Backpack.Remove(pair.first);owner.Backpack.Remove(pair.second);
            var item=TryPlace(owner.Backpack,pair.recipe.result,pair.first.Origin)
                ?? TryPlace(owner.Storage,pair.recipe.result,Vector2Int.zero);
            if(item==null)
            {
                var delivery=new Delivery(pair.recipe.result);
                deliveries.Add(delivery);
                item=delivery.Content;
            }
            evolved.Add(item);
        }
        owner.CalculateStats();
        if(ui!=null && ui.ActiveTower==owner){ui.RefreshItemVisuals();foreach(var result in evolved)ui.HighlightEvolved(result);}
    }
    // Attachments are never consumed by a host sale, main-item replacement or evolution.
    public void ReleaseAttachments(PlacedItem host)
    {
        if(host==null)return;
        foreach(var part in new List<PlacedItem>(host.Attachments)){
            host.Detach(part);
            StoreOrDeliver(part);
        }
    }
    public void StoreOrDeliver(PlacedItem item)
    {
        if(item==null||item.Bag!=null||item.Parent!=null)return;
        bool placed=false;
        for(int y=0;y<owner.Storage.Height&&!placed;y++)for(int x=0;x<owner.Storage.Width&&!placed;x++)for(int r=0;r<4&&!placed;r++)
            placed=owner.Storage.PlaceExisting(item,new Vector2Int(x,y),r);
        if(!placed)deliveries.Add(new Delivery(item));
    }
    private bool ContainsItem(PlacedItem item)
    {
        foreach(var p in owner.Backpack.Items)if(p==item)return true;
        return false;
    }
    private static PlacedItem TryPlace(Backpack bag, ItemData item, Vector2Int preferred)
    {
        for(int r=0;r<4;r++)if(bag.CanPlace(item,preferred,r))return bag.Place(item,preferred,r);
        for(int y=0;y<bag.Height;y++)for(int x=0;x<bag.Width;x++)for(int r=0;r<4;r++)
        {
            var pos=new Vector2Int(x,y);
            if(bag.CanPlace(item,pos,r))return bag.Place(item,pos,r);
        }
        return null;
    }
}
