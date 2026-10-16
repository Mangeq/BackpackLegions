using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// A persistent overlay only during a scene transition. Boot uses the same prefab.
public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField] private string loadOnStart;
    [SerializeField] private UnityEngine.UI.Slider progress;
    [SerializeField] private TMPro.TMP_Text status;
    [SerializeField] private UnityEngine.UI.Button retry;
    private static SceneLoadingUI instance;
    public static bool IsLoading { get; private set; }
    private string target;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() { instance = null; IsLoading = false; }

    private void Start() { if (!string.IsNullOrEmpty(loadOnStart) && !IsLoading) Begin(loadOnStart); }

    public static void LoadScene(string sceneName)
    {
        if (IsLoading) return;
        if (instance == null)
        {
            var prefab = Resources.Load<SceneLoadingUI>("UI/SceneLoadingUI");
            if (prefab == null) { Debug.LogError("SceneLoadingUI prefab eksik."); return; }
            instance = Instantiate(prefab);
        }
        instance.Begin(sceneName);
    }

    private void Begin(string sceneName)
    {
        instance = this; target = sceneName; IsLoading = true;
        DontDestroyOnLoad(gameObject); Time.timeScale = 1f;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        progress.value = 0; retry.gameObject.SetActive(false);
        status.text = target == "MainMenu" ? "KARARGÂH HAZIRLANIYOR" : "BÖLÜM HAZIRLANIYOR";
        yield return null; // Let the overlay render before starting scene work.
        if (!Application.CanStreamedLevelBeLoaded(target)) { Fail(); yield break; }
        AsyncOperation operation = null;
        try { operation = SceneManager.LoadSceneAsync(target); } catch (System.Exception exception) { Debug.LogException(exception); }
        if (operation == null) { Fail(); yield break; }
        operation.allowSceneActivation = false;
        while (operation.progress < .9f) { progress.value = Mathf.Clamp01(operation.progress / .9f); yield return null; }
        progress.value = 1; status.text = "HAZIR";
        yield return null;
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;
        IsLoading = false; instance = null; Destroy(gameObject);
    }

    private void Fail() { status.text = "YÜKLEME BAŞLATILAMADI"; IsLoading = false; retry.gameObject.SetActive(true); }
    public void Retry() { if (!IsLoading) Begin(target); }
    private void OnDestroy() { if (instance == this) { instance = null; IsLoading = false; } }
}
