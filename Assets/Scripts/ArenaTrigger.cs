using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaTrigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    public SceneSwitch sceneSwitch;

    private bool triggerActive;

    public void performAction()
    {
        if(player.currencyCounter != 0){
            string arenaCurrency = sceneSwitch.ArenaName+"Currency";
            int currentCurrency = PlayerPrefs.GetInt(arenaCurrency);
            
            PlayerPrefs.SetInt(arenaCurrency, currentCurrency + player.currencyCounter);
            Debug.Log("Successfully extracted extra " + player.currencyCounter + " " + arenaCurrency);
            Debug.Log("Current " + arenaCurrency + ": " + PlayerPrefs.GetInt(arenaCurrency));
        }

        sceneSwitch.setArenaName("Hub");
        sceneSwitch.LoadArenaScene();
    }


    public void TeleportToHub()
    {
        player.transform.position = new Vector3(transform.position.x, 30.1f, transform.position.z);
    }
 
}
