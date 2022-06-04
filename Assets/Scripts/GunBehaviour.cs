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

    private string availableAmmoString;

    public bool is_pistol;

    public float fireRateTimer;

    private float lastShotTime;

    public float reloadTime;

    private bool reloading = false;

    private float reloadStart;

    private GameObject camera;

    //[FMODUnity.EventRef]
    //public string ShootingEvent = "";

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

    WeaponRecoil recoil;

    void Start()
    {
        recoil = GetComponent<WeaponRecoil>();
        if(is_pistol){
            availableAmmoString = "∞";
        }
        FillAmmo();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update(){
        if(reloading){
            if(Time.time - reloadStart >= reloadTime){
                reloading = false;
                finishReload();
            }
        }
    }

    public void Shoot(Vector2 shootingSpreadVec)
    {
        if (currentAmmo <= 0 | reloading){
            return;
        }
        if(Time.time - lastShotTime <= fireRateTimer){
            return;
        }
        lastShotTime = Time.time;
        currentAmmo--;
        muzzleFlash.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/General Sounds/Character Related/Gun Shooting/Pistol");
        recoil.TriggerRecoil();
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
        if(currentAmmo == 0){
            Reload();
        }
    }

    public void updateReloadUI()
    {
        currentAmmoUI.text = currentAmmo.ToString();
        availableAmmoUI.text = availableAmmoString;
    }

    public void Reload()
    {
        if(!reloading){
            reloadStart = Time.time;
            reloading = true;
        }
    }

    private void finishReload(){
        if(is_pistol){
            currentAmmo = numBulletsPerMagazine;
        }
        else{
            int ammoNeeded = numBulletsPerMagazine - currentAmmo;
            if (ammoNeeded <= availableAmmo){
                availableAmmo -= ammoNeeded;
                currentAmmo += ammoNeeded;
            } else {
                availableAmmo = 0;
                currentAmmo += availableAmmo;
            }
        }
        updateReloadUI();
    }

    public void FillAmmo(){
        currentAmmo = numBulletsPerMagazine;
        if(!is_pistol){
            availableAmmo = numBulletsPerMagazine * defaultNumberOfExtraMagazines;
            availableAmmoString = availableAmmo.ToString();
        }
        updateReloadUI();
    }

}
