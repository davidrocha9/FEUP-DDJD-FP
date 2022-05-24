
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    [SerializeField]
    private HordeSpawner hordeSpawner;

    private float startTime;

    private bool round_active = false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        if (round_active){
            if ( (Time.time - startTime) >= 1){
                startTime = Time.time;
                SpawnHorde(5);
            }
        }
    }


    void ShowWaveStartUI(int wave_num)
    {
        Debug.Log("Wave " + wave_num.ToString());
    }

    void ShowRoundStartUI(int round_num)
    {
        Debug.Log("Round " + round_num.ToString());
    }

    void StartRound(int round_num){
        ShowRoundStartUI(round_num);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
