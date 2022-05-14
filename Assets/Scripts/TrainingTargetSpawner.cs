
using UnityEngine;

public class TrainingTargetSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject enemiesHolder;

    float startTime;

    Vector2 xLimits = new Vector2(2.5f, 13f);
    Vector2 zLimits = new Vector2(-3f, 3f);
    float spawningY = 28.5f;

    public bool active;

    int counter = 1;
    int timeBetweenSpawns = 6;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(counter % (5*(7-timeBetweenSpawns)) == 0){
            timeBetweenSpawns -= 1;
            if(timeBetweenSpawns <= 0){
                active = false;
            }
        }
        if (((Time.time - startTime) >= timeBetweenSpawns) && active){
            startTime = Time.time;
            SpawnTarget();
            counter += 1;
        }
    }

    void SpawnTarget()
    {
        float x = Random.Range(xLimits[0], xLimits[1]);
        float z = Random.Range(zLimits[0], zLimits[1]);
        Vector3 position = new Vector3(x, spawningY, z);
        GameObject enemy = Instantiate(enemyPrefab, position, new Quaternion(0, 0, 0, 0));
        enemy.transform.SetParent(enemiesHolder.transform);
    }
}

