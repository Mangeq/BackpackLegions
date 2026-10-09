var rows=new System.Collections.Generic.List<object>();
var d=UnityEngine.ScriptableObject.CreateInstance<ItemData>();
d.itemShape=new System.Collections.Generic.List<UnityEngine.Vector2Int>{new UnityEngine.Vector2Int(0,0),new UnityEngine.Vector2Int(0,1),new UnityEngine.Vector2Int(0,2),new UnityEngine.Vector2Int(1,2)};
d.centerOffset=new UnityEngine.Vector2Int(0,1);
var original=new System.Collections.Generic.List<UnityEngine.Vector2Int>(d.itemShape);
var expected=new[]{new[]{"0,0","0,1","0,2","1,2"},new[]{"2,0","1,0","0,0","0,1"},new[]{"1,2","1,1","1,0","0,0"},new[]{"0,1","1,1","2,1","2,0"}};
for(int r=0;r<4;r++){var actual=new System.Collections.Generic.HashSet<string>();foreach(var c in ItemShapeUtility.GetRotatedShape(d,r))actual.Add(c.x+","+c.y);rows.Add(new{test="L shape rotation "+r,passed=actual.SetEquals(expected[r])});var b=new Backpack(5,5);rows.Add(new{test="bounds "+r,passed=b.CanPlace(d,UnityEngine.Vector2Int.zero,r)&&!b.CanPlace(d,new UnityEngine.Vector2Int(4,4),r)});var p=b.Place(d,new UnityEngine.Vector2Int(1,1),r);bool mapped=true;foreach(var c in p.GetOccupiedCells())mapped &= b.GetItemAt(c)==p;rows.Add(new{test="occupancy and collision "+r,passed=mapped&&!b.CanPlace(d,p.Origin,r)});b.Remove(p);rows.Add(new{test="remove rotated "+r,passed=b.Items.Count==0&&b.CanPlace(d,new UnityEngine.Vector2Int(1,1),r)});}
rows.Add(new{test="four turns and reverse normalize",passed=ItemShapeUtility.NormalizeRotation(4)==0&&ItemShapeUtility.NormalizeRotation(-1)==3&&ItemShapeUtility.NormalizeRotation(-4)==0});
var a=new Backpack(5,5);var aa=a.Place(d,UnityEngine.Vector2Int.zero,1);var bb=new Backpack(5,5).Place(d,UnityEngine.Vector2Int.zero,0);
bool unchanged=true;for(int n=0;n<original.Count;n++)unchanged &= original[n]==d.itemShape[n];
rows.Add(new{test="shared asset and second instance unchanged",passed=unchanged&&aa.Rotation==1&&bb.Rotation==0});
UnityEngine.Object.DestroyImmediate(d);return rows;
