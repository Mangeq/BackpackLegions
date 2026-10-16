if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
var chapters=Resources.LoadAll<ChapterData>("Data/Chapters").OrderBy(c=>c.chapterNumber).ToArray();
for(int c=0;c<5;c++)
{
    var scout=chapters[c].waves[0].groups[0].enemyPrefab.GetComponent<Enemy>().enemyData;scout.health=45+14*c;UnityEditor.EditorUtility.SetDirty(scout);
    for(int w=0;w<chapters[c].waves.Count;w++)
    {
        bool boss=w==chapters[c].waves.Count-1;
        foreach(var g in chapters[c].waves[w].groups)g.spawnInterval=Mathf.Max(.75f,2.15f-.1f*c-.1f*w)*(boss?2:chapters[c].waves[w].groups.Count);
    }
    UnityEditor.EditorUtility.SetDirty(chapters[c]);
}
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.AssetDatabase.Refresh();return "Opening tuned after live combat; boss strength unchanged.";
