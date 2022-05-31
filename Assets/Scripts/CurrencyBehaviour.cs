
using UnityEngine;

public class CurrencyBehaviour : MonoBehaviour
{
    public float despawnTimer;
    float spawnTime;
    float flashTimer;
    bool isFlashing = false;
    float flashingTime = 0.0f;
    float blinkTimeFrame;
    int blinkCounter = 1;
    float startBlinkFrame;
    int speedDecreases = 1;

    void Start(){
        spawnTime = Time.time;
        flashTimer = despawnTimer - despawnTimer*0.4f;
        blinkTimeFrame = (despawnTimer-flashTimer)/50.0f;
        startBlinkFrame = blinkTimeFrame * 5;
    }

    void Update(){
        if(Time.time - spawnTime >= despawnTimer){
            isFlashing = false;
            DeSpawn();
        }
        if(Time.time - spawnTime >= flashTimer){
            isFlashing = true;
        }
        if(isFlashing){
            flashingTime+=Time.deltaTime;
            if(flashingTime >= startBlinkFrame){
                blinkCounter++;
                if((blinkCounter % (3*speedDecreases)==0) && startBlinkFrame > blinkTimeFrame){
                    speedDecreases +=1;
                    startBlinkFrame -= blinkTimeFrame;
                }
                flashingTime = 0.0f;
                GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
            }
        }
    }

    public void DeSpawn(){
        Destroy(gameObject);
    }

}
