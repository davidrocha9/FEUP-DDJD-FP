using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumblePlane : MonoBehaviour
{
    private int zrotation = 0;
    public float xLimit;
    public float yLimit;
    public float zLimit;
    private Vector3 teleportPosition;

    //public Vector3 spawnPosition;  
    void Awake(){
        zrotation = Random.Range(0, 360);
        // apply rotation to the transform of the game object
        transform.Rotate(0, 0, zrotation);
        float increaseX = 0.0f;
        float increaseY = 0.0f;
        int xfactor = 1;
        int yfactor = 0;
        int counter = 0;
        while(Physics.CheckSphere(new Vector3(transform.position.x + increaseX, 5.0f + increaseY, transform.position.z+1.0f), 0.5f)){
            increaseX += 0.1f*xfactor;
            increaseY += 0.05f*yfactor;
            xfactor = -xfactor;
            counter += 1;
            if(increaseX >= xLimit && yfactor == 0){
                //increaseX = 0.0f;
                xfactor = 0;
                yfactor = 1;
            }
            else if(increaseY >= yLimit && xfactor == 0){
                //increaseY = 0.0f;
                xfactor = 1;
                yfactor = 1;
            }
            if(Mathf.Abs(increaseX) >= xLimit && increaseY >= yLimit){
                Debug.Log("No position found");
                return;
            }
            if(counter > 100){
                Debug.Log("No position found");
                return;
            }

        }
        teleportPosition = new Vector3(transform.position.x + increaseX, 3.0f + increaseY, transform.position.z + 1.0f);
        Debug.Log(teleportPosition);
    }

    public Vector3 getSpawnPosition(){
        int startZ = 0;
        if(transform.position.z > 50){
            startZ = 100;
        }
        else if(transform.position.z < -50){
            startZ = -100;
        }
        Vector3 spawnPosition = new Vector3(10*Random.Range(-1,2), 3, startZ+10*Random.Range(-1,2));
        return spawnPosition;
    }

    public Vector3 getTeleportPosition(){
        return teleportPosition;
    }

    public int getZRotation(){
        return zrotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
