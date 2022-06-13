using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SceneSwitch : MonoBehaviour
{
    public static SceneSwitch Instance;

    [SerializeField]
    private GameObject _loaderCanvas;

    [SerializeField]
    private Slider _progressBar;

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

        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void ShowLoadingScreen()
    {
        //loadingUI.SetActive(true);
    }

    public async void LoadArenaScene()
    {
        _loaderCanvas.SetActive(true);
        var scene = SceneManager.LoadSceneAsync(ArenaName);
        scene.allowSceneActivation = false;

        do {
            await Task.Delay(100);
            //_progressBar.value = scene.progress;
            _progressBar.value += 0.05f;
        } while (scene.progress < 0.9f || _progressBar.value < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
    }

    void Update(){
        if (SceneManager.GetActiveScene().name != ArenaName)
            return;

        if (GameObject.FindWithTag("LoadingCanvas") == null || GameObject.FindWithTag("LoadingBar") == null)
            return;

        _loaderCanvas = GameObject.FindWithTag("LoadingCanvas");
        _progressBar = GameObject.FindWithTag("LoadingBar").GetComponent<Slider>();

        switch(ArenaName) {
            case "Factory":
            case "Colliseum":
                if (_loaderCanvas.active)
                {
                    _loaderCanvas.GetComponent<Animator>().Play("LoadingFadeOut");
                }
                break;
        
            default:
                break;
        }
    }

    IEnumerator LoadingScreen()
    {
        /*while (!sceneToLoad.isDone){
            loadingBarFill.fillAmount = sceneToLoad.progress;
            yield return null;
        }*/
        yield return null;
    }
    
    public void setArenaName(string name){
        ArenaName = name;
    }
}