
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{

    [SerializeField]
    private GameObject loadingUI;

    [SerializeField]
    private Image loadingBarFill;

    private AsyncOperation sceneToLoad = null;

    public void ShowLoadingScreen()
    {
        loadingUI.SetActive(true);
    }

    public void LoadHubScene()
    {
        sceneToLoad = SceneManager.LoadSceneAsync("Hub");
        StartCoroutine(LoadingScreen());
    }

    public void LoadArenaScene()
    {
        sceneToLoad = SceneManager.LoadSceneAsync("Arena");
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        while (!sceneToLoad.isDone){
            loadingBarFill.fillAmount = sceneToLoad.progress;
            yield return null;
        }
    }
}
