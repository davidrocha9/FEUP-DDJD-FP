using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumblePlane : MonoBehaviour
{
    private int zrotation = 0;
    public float xLimit;
    public float yLimit;
    public float zLimit;
    private int startZ;
    private Vector3 teleportPosition = Vector3.zero;

    //public Vector3 spawnPosition;  
    void Awake(){
        zrotation = Random.Range(-180, 181);
        // apply rotation to the transform of the game object
        transform.Rotate(0, 0, zrotation);
        int xfactor = 1;
        int counter = 1;
        if(zrotation >= 0){
            this.gameObject.transform.GetChild(0).gameObject.transform.Translate(new Vector3(0.0f,1.35f,0.0f));
        }
        else{
            this.gameObject.transform.GetChild(0).gameObject.transform.Translate(new Vector3(0.0f,-1.35f,0.0f));
        }
        for(float x = 0f; Mathf.Abs(x)<=(xLimit+0.1f); x += 0.1f*xfactor){
            for(float y = 0; y <= yLimit; y+=0.05f){
                if(!Physics.CheckSphere(new Vector3(transform.position.x + x, y, transform.position.z+1.0f), 0.75f)){
                    teleportPosition = new Vector3(transform.position.x + x, 3.0f + y, transform.position.z + 1.0f);
                    Debug.Log(teleportPosition);
                    return;
                }
            }
            if(x >= xLimit){
                xfactor = -1;
                x = 0f;
            }
        }
        if(teleportPosition == Vector3.zero){
            teleportPosition = getSpawnPosition();
        }
    }

    public Vector3 getSpawnPosition(){
        startZ = 0;
        if(transform.position.z > 50){
            startZ = 100;
        }
        else if(transform.position.z < -50){
            startZ = -100;
        }
        int spawnZ = 0;
        if(teleportPosition.z - startZ > 0){
            spawnZ = startZ-10;
        }
        else{
            spawnZ = startZ+10;
        }
        Vector3[] spawnPositions = new [] {new Vector3(-10,3,startZ-10), new Vector3(0,3,startZ-10), new Vector3(10,3,startZ-10), new Vector3(-10,3,startZ+10), new Vector3(0,3,startZ+10), new Vector3(10,3,startZ+10)};
        Transform childTransform = this.gameObject.transform.GetChild(0).gameObject.transform;
        Vector3 spawnPosition = Vector3.up;
        float maxdist = 0;
        foreach(Vector3 pos in spawnPositions){
            float dist = Vector3.Distance(childTransform.position, pos);
            if(maxdist < dist){
                spawnPosition = pos;
                maxdist = dist;
            }
        }

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
