using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaTrigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    public SceneSwitch sceneSwitch;

    private bool triggerActive;

    public void performAction()
    {
        //TeleportToHub();
        sceneSwitch.LoadHubScene();
    }


    public void TeleportToHub()
    {
        
        player.transform.position = new Vector3(transform.position.x, 30.1f, transform.position.z);
    }
 
}
