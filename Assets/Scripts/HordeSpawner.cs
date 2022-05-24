
using UnityEngine;

public class HordeSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject enemiesHolder;

    float startTime;

    Vector2 xLimits = new Vector2(-30, 30);
    Vector2 zLimits = new Vector2(-30, 30);

    public bool active;


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
        if ( (Time.time - startTime) >= 5 && active){
            startTime = Time.time;
            SpawnHorde(5);
        }
    }

    bool checkIfInBounds(float x, float y, float centerX, float centerY, float radius){

        return Mathf.Pow((x-centerX), 2) + Mathf.Pow((y-centerY), 2) < Mathf.Pow(radius, 2);

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
}
