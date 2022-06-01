
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

    public string ArenaName;

    void Awake(){
        if(!PlayerPrefs.HasKey("ColliseumCurrency")){
            PlayerPrefs.SetInt("ColliseumCurrency", 0);
        }
        if(!PlayerPrefs.HasKey("FactoryCurrency")){
            PlayerPrefs.SetInt("FactoryCurrency", 0);
        }
        if(!PlayerPrefs.HasKey("ForestCurrency")){
            PlayerPrefs.SetInt("ForestCurrency", 0);
        }
        if(!PlayerPrefs.HasKey("RumbleCurrency")){
            PlayerPrefs.SetInt("RumbleCurrency", 0);
        }
        PlayerPrefs.Save();
    }

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
        sceneToLoad = SceneManager.LoadSceneAsync(ArenaName);
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        while (!sceneToLoad.isDone){
            loadingBarFill.fillAmount = sceneToLoad.progress;
            yield return null;
        }
    }

    public void setArenaName(string name){
        ArenaName = name;
    }
}
