using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    private bool triggerActive;

    public void performAction()
    {
        TeleportToArena();
    }

    public void TeleportToArena()
    {
        player.transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
    }

}
