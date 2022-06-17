
using UnityEngine;
using UnityEngine.AI;
using TMPro;


public class RangedEnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    public int moveSpeed;

    public float range = 10f;

    private bool alreadyShot = false;

    Vector3 offset;

    public float health;
    public int dropPercentage;

    public float damage = 10f;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    
    [SerializeField]
    private GameObject currencyPrefab;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private TextMeshProUGUI damageText;

    public ObstacleAvoidanceType AvoidanceType;

    private Vector3 bulletOffset = new Vector3(0f, 1f, 0f);

    private float upDir =  0f;

    private GameObject currencyHolder;

    private Rigidbody body;

    private NavMeshAgent navMeshAgent;

    private Vector3 playerOffset;

    [SerializeField]
    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //layerMask = new LayerMask();
        playerOffset = new Vector3(0f, 1f, 0f);
        body = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");
        navMeshAgent.obstacleAvoidanceType = AvoidanceType;
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform == null)
            return;
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            moveSpeed = 0;
            
            //navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }
        
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Shooting"))
        {
            //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.7f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.4f && !alreadyShot)
            {
                Shoot();
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.85f)
            {
                alreadyShot = false;
            }
            
            /*if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                alreadyAttacked = false;
                registeredHit = false;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && !registeredHit)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 1.5)
                {
                    Debug.Log("hit");
                    //playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(10);
                }
                registeredHit = true;
            }*/
        }

        if (Vector3.Distance(transform.position, playerTransform.position) <= range && hasLineOfSightToPlayer()){
            // navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            transform.LookAt(playerTransform);

            upDir = transform.forward.y;

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);
            animator.SetBool("is_walking", false);
            animator.SetBool("is_shooting", true);
        } else {
            // navMeshAgent.isStopped = false;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(playerTransform.position);
            transform.LookAt(playerTransform);

            upDir = transform.forward.y;

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            //transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_shooting", false);
            animator.SetBool("is_walking", true);
        }

        /*if (Vector3.Distance(transform.position, playerTransform.position) > range + 10f){
            transform.LookAt(playerTransform);

            upDir = transform.forward.y;

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_shooting", false);
            animator.SetBool("is_walking", true);
            
        } else {
            transform.LookAt(playerTransform);

            upDir = transform.forward.y;

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);
            animator.SetBool("is_walking", false);
            animator.SetBool("is_shooting", true);
        }*/
        damageText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        // damage text rotation equal to transform rotation with 180 degree offset
        damageText.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));
    }

    public void Shoot()
    {
        alreadyShot = true;
        Vector3 offset = new Vector3(0.1f, 1, 0.2f);
        Vector3 dest = transform.forward;
        dest.y = upDir;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + bulletOffset, new Quaternion(0, 0, 0, 0));
        bullet.GetComponent<BulletBehaviour>().setDamage(damage);
        //bullet.GetComponent<Rigidbody>().velocity = transform.forward * 3f;
        bullet.GetComponent<Rigidbody>().AddForce(dest * 200f);
    }

    public void TakeDamage(float damage)
    {

        if(!animator.GetBool("is_dead")){
            health -= damage;

            damageText.text = damage.ToString();
            damageText.GetComponent<Animator>().Play("EnemyDamageOnHit", -1, 0f);

            if (health <= 0){
                Die();
            }
        }
    }

    void OnParticleCollision(){
            TakeDamage(1);
    }

    private void Die()
    {
        moveSpeed = 0;
        animator.SetBool("is_dead", true);
    }

    private void DropCurrency(){
        if(!dropped){
            dropped = true;
            int dropRng = Random.Range(1, 101);
            if(dropRng <= dropPercentage){
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
                GameObject currency = Instantiate(currencyPrefab, spawnPos, new Quaternion(0, 0, 0, 0));
                Debug.Log("Dropped currency");
                currency.transform.SetParent(currencyHolder.transform);
            }
        }
    }

    private bool hasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position + bulletOffset, playerTransform.position + playerOffset, Color.yellow, 1f);
        if (Physics.Linecast(transform.position + bulletOffset, playerTransform.position + playerOffset, out hit, layerMask)){
            Debug.Log("Raycast hit!");
            Debug.Log(hit.transform.gameObject.tag);
            if (hit.transform.gameObject.tag == "Player"){
                return true;
            }
            return false;
        } else {
            Debug.Log("Raycast failed!");
            return false;
        }
    }

}
