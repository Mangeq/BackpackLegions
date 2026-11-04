using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class CampaignMapLoader : MonoBehaviour
{
    public CampaignMap[] maps;
    public GameObject editorPreview;
    public CampaignMap ActiveMap { get; private set; }
    private void Awake()
    {
        if (maps == null || maps.Length == 0) return;
        if (editorPreview != null) editorPreview.SetActive(false);
        ActiveMap = Instantiate(maps[Mathf.Clamp(ChapterData.SelectedIndex, 0, maps.Length - 1)]);
        var waves = FindObjectOfType<WaveManager>();
        if (waves != null) waves.spawnPoint = ActiveMap.spawnPoint;
    }
}
