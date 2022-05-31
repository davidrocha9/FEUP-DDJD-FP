
using UnityEngine;

public class TrainingTargetBehaviour : MonoBehaviour
{

    float maxY = 29.8f;
    float startingY;

    bool moveUp = true;
    bool moveDown = false;

    bool dying = false;
    
    float spawnTime;

    float deathTime;
    public float health;

    public int maxDuration;
    public int minDuration;
    
    int duration;

    Vector3 pivotPoint;


    // Start is called before the first frame update
    void Start()
    {
        startingY = transform.position.y;
        duration = Random.Range(minDuration, maxDuration);
        pivotPoint = new Vector3(transform.position.x, 30, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(moveUp){
            if(transform.position.y >= maxY){
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
                spawnTime = Time.time;
                moveUp = false;
            }
            else{
                transform.position += transform.up * 2 * Time.deltaTime;
            }
        }
        else{
            if(moveDown){
                if(transform.position.y <= startingY){
                    moveDown = false;
                    Destroy(this.gameObject);
                }
                else{
                    transform.position += transform.up * -2 * Time.deltaTime;
                }
            }
            else{
                if(Time.time - spawnTime >= duration){
                    moveDown = true;
                }
            }
        }
        
        if(dying){
            transform.RotateAround(pivotPoint, Vector3.forward, -180 * Time.deltaTime);
            if(Time.time - deathTime >= 0.5f){
                Destroy(this.gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0){
            Die();
        }
    }

    private void Die()
    {
        dying = true;
        deathTime = Time.time;
    }

}
