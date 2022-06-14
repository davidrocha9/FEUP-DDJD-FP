using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem particleSystem;
    private float lastFireTime;
    private float randomTime = -1f;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        lastFireTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(randomTime != -1f){
            if(Time.time - lastFireTime >= randomTime){
                particleSystem.Play();
                lastFireTime = Time.time;
                randomTime = -1f;
            }
        }
        else{
            randomTime = Random.Range(5, 15);
        }
    }
}
