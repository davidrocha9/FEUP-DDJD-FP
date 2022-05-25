
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject enemiesHolder;

    private float startTime;

    private bool round_active = false;

    private int numWaves = 0;
    private float waveTimeout = 60f;
    private int waveSize = 5;

    Vector2 xLimits = new Vector2(-30, 30);
    Vector2 zLimits = new Vector2(-30, 30);

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (numWaves <= 0){
            round_active = false;
        }
    }

    void FixedUpdate()
    {
        if (round_active){
            if ( (Time.time - startTime) >= waveTimeout || enemiesHolder.transform.childCount <= 0){
                startTime = Time.time;
                numWaves--;
                ShowWaveStartUI(numWaves);
                SpawnHorde(waveSize);
            }
        }
    }

    bool checkIfInBounds(float x, float y, float centerX, float centerY, float radius){

        return Mathf.Pow((x-centerX), 2) + Mathf.Pow((y-centerY), 2) < Mathf.Pow(radius, 2);

    }

    public void StartRound(int roundNumber)
    {
        if (round_active) return;
        ShowRoundStartUI(roundNumber);
        round_active = true;
        numWaves = roundNumber;

    }

    void SpawnHorde(int numEnemies)
    {
        int num_spawned = 0;
        while (num_spawned < numEnemies)
        {
            float x = Random.Range(xLimits[0], xLimits[1]);
            float z = Random.Range(zLimits[0], zLimits[1]);
            if (checkIfInBounds(x, z, 0, 0, 37)){
                Vector3 position = new Vector3(x, 3, z);
                GameObject enemy = Instantiate(enemyPrefab, position, new Quaternion(0, 0, 0, 0));
                enemy.transform.SetParent(enemiesHolder.transform);
                num_spawned++;
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


}
