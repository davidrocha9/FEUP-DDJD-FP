using UnityEngine.UI;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{

    public float damage;

    public float range;

    public float shootingSpread;

    public int numBulletsPerMagazine;

    public int defaultNumberOfExtraMagazines;

    public int currentAmmo;

    public int availableAmmo;

    private GameObject camera;

    private Vector2 screenCenter;

    public ParticleSystem[] impactEffects;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private Text currentAmmoUI;

    [SerializeField]
    private Text availableAmmoUI;

    [SerializeField]
    private LayerMask aimColliderMask = new LayerMask();

    void Start()
    {
        FillAmmo();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    public void Shoot(Vector2 shootingSpreadVec)
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        muzzleFlash.Play();
        Ray ray = Camera.main.ScreenPointToRay(screenCenter + shootingSpreadVec);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, aimColliderMask)){
            ParticleSystem effect = Instantiate(impactEffects[0], hit.point, Quaternion.identity);
            effect.transform.forward = -transform.forward;
            Destroy(effect.gameObject, effect.main.duration);

            if (hit.transform.tag == "Enemy"){
                EnemyBehaviour enemy = hit.transform.GetComponent<EnemyBehaviour>();
                if (enemy != null){
                    enemy.TakeDamage(damage);
                }
            }
            if(hit.transform.name == "target_test"){
                TrainingTargetBehaviour target = hit.transform.parent.GetComponent<TrainingTargetBehaviour>();
                if (target != null){
                    target.TakeDamage(damage);
                }
            }
        }
        updateReloadUI();
    }

    public void updateReloadUI()
    {
        currentAmmoUI.text = currentAmmo.ToString();
        availableAmmoUI.text = availableAmmo.ToString();
    }

    public void Reload()
    {
        int ammoNeeded = numBulletsPerMagazine - currentAmmo;
        if (ammoNeeded <= availableAmmo){
            availableAmmo -= ammoNeeded;
            currentAmmo += ammoNeeded;
        } else {
            availableAmmo = 0;
            currentAmmo += availableAmmo;
        }
        updateReloadUI();
    }

    public void FillAmmo(){
        currentAmmo = numBulletsPerMagazine;
        availableAmmo = numBulletsPerMagazine * defaultNumberOfExtraMagazines;
        updateReloadUI();
    }

}
