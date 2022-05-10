
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{

    [SerializeField]
    private float damage = 10f;

    [SerializeField]
    private float range = 100f;

    [SerializeField]
    private GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(camera.transform.position, -camera.transform.forward, Color.green);
    }


    public void Shoot()
    {
        Debug.Log("Shooting");
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, -camera.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
        }
    }

}
