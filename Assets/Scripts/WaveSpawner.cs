using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

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
    private int extraEnemyCount;
    private int enemiesToDefeat;
    private int numWaves = 0;
    private float waveTimeout = 60f;
    private int waveSize = 15;
    private int roundNr = 0;
    private float elapsedTime = 0f, roundTime = 0f, spawnEnemyTime = 0f;
    Vector2 xLimits = new Vector2(-30, 30);
    Vector2 zLimits = new Vector2(-30, 30);

    private List<Vector3> spawnCoords = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        numEnemiesAliveText = numEnemiesAliveUI.GetComponent<Text>();
        
        spawnCoords.Add(new Vector3(-5.7f, 6.4f, 39.8f));
        spawnCoords.Add(new Vector3(16.7f, 6.4f, 36.3f));
        spawnCoords.Add(new Vector3(40.3f, 6.4f, 0f));
        spawnCoords.Add(new Vector3(33.7f, 6.4f, -21.8f));
        spawnCoords.Add(new Vector3(0.2f, 6.4f, -40.2f));
        spawnCoords.Add(new Vector3(-38.3f, 6.4f, -11.4f));
        spawnCoords.Add(new Vector3(-33.8f, 6.4f, 21.6f));

        spawnCoords.Add(new Vector3(5.7f, 11.6f, 39.4f));
        spawnCoords.Add(new Vector3(26f, 11.6f, 30f));
        spawnCoords.Add(new Vector3(21.4f, 11.6f, -33.2f));
        spawnCoords.Add(new Vector3(-11f, 11.6f, -38f));
        spawnCoords.Add(new Vector3(-38f, 11.6f, 11f));
        spawnCoords.Add(new Vector3(-21.6f, 11.6f, 33f));
        
        spawnCoords.Add(new Vector3(-11.2f, 16f, 37.6f));
        spawnCoords.Add(new Vector3(-33.4f, 16f, 21.4f));
        spawnCoords.Add(new Vector3(-35.8f, 16f, 16.4f));

        spawnCoords.Add(new Vector3(-39.4f, 20.6f, 0f));
        spawnCoords.Add(new Vector3(-30f, 20.6f, 25.9f));
        spawnCoords.Add(new Vector3(11.1f, 20.6f, 37.9f));
        spawnCoords.Add(new Vector3(38f, 20.6f, 11f));
        spawnCoords.Add(new Vector3(37.9f, 20.6f, -11f));
        spawnCoords.Add(new Vector3(29.8f, 20.6f, -25.8f));
        spawnCoords.Add(new Vector3(5.7f, 20.6f, -39f));
        spawnCoords.Add(new Vector3(-16.3f, 20.6f, -35.8f));

        // sort spawnCoords randomly
        for (int i = 0; i < spawnCoords.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnCoords.Count);
            Vector3 temp = spawnCoords[i];
            spawnCoords[i] = spawnCoords[randomIndex];
            spawnCoords[randomIndex] = temp;
        }
    }

    void Update()
    {
        if (extraEnemyCount > 0)
        {
            // add time delta time to enemyspawntime
            spawnEnemyTime += Time.deltaTime;
            
            // if spawnenemytime bigger than 2, spawn horde with extraenemycount
            if (spawnEnemyTime > 2f)
            {
                spawnEnemyTime = 0;
                SpawnHorde(extraEnemyCount);
            }
        }

        //increment elasped time with time delta
        elapsedTime += Time.deltaTime;

        //Debug.Log(elapsedTime);
        
        // if elapsed time bigger than 3 and not round active, set round active to true, reset elapsed time and call startround function
        if (elapsedTime > 3f && !round_active && enemiesHolder.transform.childCount == 0)
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
            if (roundNr % 5 != 0){
                if ((Time.time - startTime) >= waveTimeout || enemiesHolder.transform.childCount <= 0){
                    startTime = Time.time;
                    numWaves++;

                    Debug.Log("Still need to defeat " + enemiesToDefeat + " enemies");
                    
                    UpdateNumEnemiesAlive();

                    if (enemiesToDefeat > 0)
                    {
                        ShowWaveStartUI(numWaves);
                        Debug.Log("Spawning wave with size " + (waveSize - enemiesHolder.transform.childCount));
                        if (waveSize - enemiesHolder.transform.childCount <= enemiesToDefeat)
                            SpawnHorde(waveSize - enemiesHolder.transform.childCount);
                        else
                            SpawnHorde(enemiesToDefeat);
                    }
                }
            } else {
                if (enemiesHolder.transform.childCount <= 0){
                    round_active = false;
                }
            }

            UpdateNumEnemiesAlive();
        }
    }

    public void UpdateNumEnemiesAlive()
    {
        // convert numEnemiesAliveText.text to int and store it previousNumber
        int previousNumber = int.Parse(numEnemiesAliveText.text);
        
        int count = 0;
        // iterate enemiesHolder and check if enemies animation is not death
        foreach (Transform child in enemiesHolder.transform)
        {
            if (!child.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                count++;
            }
        }

        if (previousNumber - count > 0)
        {
            enemiesToDefeat -= (previousNumber - count);
            if (enemiesToDefeat == 0)
            {
                Debug.Log("You killed all enemies");
                round_active = false;
                elapsedTime = 0f;
            }
        }

        numEnemiesAliveText.text = count.ToString();
    }

    bool checkIfInBounds(float x, float y, float centerX, float centerY, float radius){

        return Mathf.Pow((x-centerX), 2) + Mathf.Pow((y-centerY), 2) < Mathf.Pow(radius, 2);

    }

    public void StartRound()
    {
        Debug.Log("Start round!");
        roundNr++;
        ShowRoundStartUI();
        if (roundNr % 5 != 0)
        {
            waveSize = waveSize + 1;
            SpawnHorde(waveSize);
            numWaves = 1;
            enemiesToDefeat = waveSize * (roundNr + 2);
        }
        else {
            // TODO: spawn boss
        }
    }

    void SpawnHorde(int numEnemies)
    {
        roundTime = 0f;
        int num_spawned = 0;

        // if scene name is colliseum then numEnemies is the length of the spawnCoords list and extraEnemies is numEnemes - spawncoords list length
        if (SceneManager.GetActiveScene().name == "Colliseum" && numEnemies > spawnCoords.Count)
        {
            extraEnemyCount = numEnemies - spawnCoords.Count;
            numEnemies = spawnCoords.Count;
            spawnEnemyTime = 0.0f;
        }

        // list equal to spawnCoords
        List<Vector3> available = new List<Vector3>(spawnCoords);

        while (num_spawned < numEnemies)
        {   
            switch(SceneManager.GetActiveScene().name)
            {
                case "Colliseum":
                    // get random number between 0 and 6
                    int random = Random.Range(0, available.Count - 1);

                    Vector3 position = available[random];
                    
                    // remove position from available list
                    available.RemoveAt(random);

                    GameObject enemy = Instantiate(enemyPrefab, position, new Quaternion(0, 0, 0, 0));
                    //GameObject enemy = Instantiate(enemyPrefab, RandomNavmeshLocation(100f), new Quaternion(0, 0, 0, 0));
                    enemy.transform.GetChild(0).GetComponent<EnemyBehaviour>().setStats(roundNr);
                    enemy.transform.SetParent(enemiesHolder.transform);
                    UpdateNumEnemiesAlive();
                    num_spawned++;

                    break;
                case "Factory":
                    GameObject spawned = Instantiate(enemyPrefab, RandomNavmeshLocation(100f), Quaternion.identity);
                    spawned.transform.SetParent(enemiesHolder.transform);
                    num_spawned++;
                    break;
            }
        }
        UpdateNumEnemiesAlive();
    }


    void ShowWaveStartUI(int wave_num)
    {
        //Debug.Log("Wave " + wave_num.ToString());
    }

    void ShowRoundStartUI()
    {
        startRoundText = true;
        startRoundTextUI.text = "Round " + roundNr.ToString();

        if (roundNr % 5 == 0)
            startRoundTextUI.text = "BOSS FIGHT";

        startRoundTextUI.gameObject.GetComponent<Animator>().Play("RoundStartTextFadeOut", -1, 0f);
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        //randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

}
