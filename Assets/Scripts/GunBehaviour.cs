
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{

    public float damage = 20f;

    public float range = 100f;

    public float shootingSpread = 15f;

    private GameObject camera;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    public void Shoot()
    {
        muzzleFlash.Play();
    }

}
