using System.Collections;
using UnityEngine;

public class HealingOverTime : MonoBehaviour
{

    [Header("Health Parameters")]

    [SerializeField]
    private float healingWaitTime = 3f;

    [SerializeField]
    private float healingTickTime = 1f;

    [SerializeField]
    private float healAmount = 1f;

    private float timeAccumulator = 0f;

    private bool coroutineActive = false;


    private StarterAssets.ThirdPersonController player;

    private IEnumerator healingCoroutine;

    void Start()
    {
        player = GetComponent<StarterAssets.ThirdPersonController>();
        healingCoroutine = TriggerHeal();
    }

    // Update is called once per frame
    void Update()
    {
        timeAccumulator += Time.deltaTime;

        if (timeAccumulator >= healingWaitTime && !coroutineActive){
            healingCoroutine = TriggerHeal();
            StartCoroutine(healingCoroutine);
            coroutineActive = true;
        }

    }

    public void PlayerTookDamage()
    {
        StopCoroutine(healingCoroutine);
        coroutineActive = false;
        timeAccumulator = 0;
    }

    IEnumerator TriggerHeal() {
        while(true) {
            player.Heal(healAmount);
            yield return new WaitForSeconds(healingTickTime);
        }
 }
}
