using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    private bool triggerActive;

    public SceneSwitch sceneSwitch;

    public void performAction()
    {
        //TeleportToArena();
        sceneSwitch.LoadArenaScene();
    }

    public void TeleportToArena()
    {
        player.transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
    }

}
