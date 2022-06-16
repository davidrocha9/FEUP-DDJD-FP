using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumblePlane : MonoBehaviour
{
    private int zrotation = 0;
    void Awake(){
        zrotation = Random.Range(0, 360);
        // apply rotation to the transform of the game object
        transform.Rotate(0, 0, zrotation);
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
