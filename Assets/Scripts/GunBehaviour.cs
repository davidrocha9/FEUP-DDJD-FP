
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{

    [SerializeField]
    private float damage = 10f;

    [SerializeField]
    private float range = 100f;

    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Shoot()
    {
        ParticleSystem obj = Instantiate(muzzleFlash, this.transform.position, new Quaternion(0, 0, 0, 0)) as ParticleSystem;
        obj.transform.parent = this.transform;
        obj.transform.localPosition = new Vector3(14.69f, 0, 0);
        Destroy(obj.gameObject, obj.main.duration);
    }

}
