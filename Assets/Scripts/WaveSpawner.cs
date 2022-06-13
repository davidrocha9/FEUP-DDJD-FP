using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject enemiesHolder;

    private float startTime;
    private bool startRoundText;

    [SerializeField]
    private TextMeshProUGUI startRoundTextUI;

    [SerializeField]
    private GameObject numEnemiesAliveUI;

    private Text numEnemiesAliveText;

    private bool round_active = false;
    private bool last_round = false;

    private int numWaves = 0;
    private float waveTimeout = 30f;
    private int waveSize = 10;
    private int roundNr = 0;
    private float[] enemiesHeights = { 6.4f, 11.6f, 16f, 20.6f };
    private float elapsedTime = 0f, roundTime = 0f;
    Vector2 xLimits = new Vector2(-30, 30);
    Vector2 zLimits = new Vector2(-30, 30);

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        numEnemiesAliveText = numEnemiesAliveUI.GetComponent<Text>();
    }

    void Update()
    {
        //increment round time with time delta if round active is true
        if (round_active)
        {
            roundTime += Time.deltaTime;
            //if round time is greater than wave timeout, spawn wave
            if (roundTime > waveTimeout)
            {
                SpawnHorde(waveSize + roundNr * 5);
                roundTime = 0f;
            }
        }
        
        //increment elasped time with time delta
        elapsedTime += Time.deltaTime;
        
        // if elapsed time bigger than 3 and not round active, set round active to true, reset elapsed time and call startround function
        if (elapsedTime > 3f && !round_active)
        {
            round_active = true;
            elapsedTime = 0f;
            StartRound();
        }

        if (startRoundText){
            if (startRoundTextUI.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime == 1.0f){
                startRoundTextUI.gameObject.SetActive(false);
            }
        }

        if (numWaves <= 0){
            last_round = true;
            //round_active = false;
        }
    }

    // euclidean distance function
    float Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2));
    }

    void FixedUpdate()
    {
        if (round_active){
            /*if (!last_round){
                if ((Time.time - startTime) >= waveTimeout || enemiesHolder.transform.childCount <= 0){
                    startTime = Time.time;
                    numWaves--;
                    Debug.Log("New Wave Now");
                    ShowWaveStartUI(numWaves);
                    SpawnHorde(waveSize);
                }
            } else {
                if (enemiesHolder.transform.childCount <= 0){
                    round_active = false;
                }
            }*/

            if (enemiesHolder.transform.childCount <= 0){
                round_active = false;
            }

            UpdateNumEnemiesAlive();
        }
    }

    public void UpdateNumEnemiesAlive()
    {
        numEnemiesAliveText.text = enemiesHolder.transform.childCount.ToString();
    }

    bool checkIfInBounds(float x, float y, float centerX, float centerY, float radius){

        return Mathf.Pow((x-centerX), 2) + Mathf.Pow((y-centerY), 2) < Mathf.Pow(radius, 2);

    }

    public int StartRound()
    {
        roundNr++;
        /*if (round_active) return roundNumber;
        last_round = false;*/
        ShowRoundStartUI();
        if (roundNr % 5 != 0)
            SpawnHorde(waveSize + roundNr * 5);
        else {
            // TODO: spawn boss
        }

        /*return roundNumber + 1;*/
        return 0;
    }

    void SpawnHorde(int numEnemies)
    {
        roundTime = 0f;
        int num_spawned = 0;

        // instantiate list of vector3 with x, yand z coordinates
        List<Vector3> spawnPositions = new List<Vector3>();

        while (num_spawned < numEnemies)
        {   
            switch(SceneManager.GetActiveScene().name)
            {
                case "Colliseum":
                    float angle = Random.Range(0, 360);
                    float x = 38 * Mathf.Sin(angle);
                    float y = enemiesHeights[Random.Range(0,3)];
                    float z = 38 * Mathf.Cos(angle);
                    bool pass = true;

                    // iterate spawncoords and check if distance between old tuples and new tuples is bigger than 3
                    foreach (Vector3 coords in spawnPositions)
                    {
                        if (Distance(new Vector3(x, y, z), coords) < 3)
                        {
                            pass = false;
                        }
                    }

                    if (checkIfInBounds(x, z, 0, 0, 38) && pass){
                        Vector3 position = new Vector3(x, y, z);
                        GameObject enemy = Instantiate(enemyPrefab, position, new Quaternion(0, 0, 0, 0));
                        enemy.transform.SetParent(enemiesHolder.transform);
                        num_spawned++;

                        // add tuple x and z to list of tuples
                        spawnPositions.Add(new Vector3(x, y, z));
                    }
                    break;
                case "Factory":
                    // TODO: spawn enemies in factory
                    break;
            }
        }
        UpdateNumEnemiesAlive();
    }


    void ShowWaveStartUI(int wave_num)
    {
        Debug.Log("Wave " + wave_num.ToString());
    }

    void ShowRoundStartUI()
    {
        startRoundText = true;
        startRoundTextUI.text = "Round " + roundNr.ToString();

        if (roundNr % 5 == 0)
            startRoundTextUI.text = "BOSS FIGHT";

        startRoundTextUI.gameObject.GetComponent<Animator>().Play("RoundStartTextFadeOut", -1, 0f);
        //Debug.Log("Round " + round_num.ToString());
    }


}
