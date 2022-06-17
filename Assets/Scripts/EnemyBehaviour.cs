using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    private float moveSpeed;

    Vector3 offset;

    [Header("Enemy Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 5f;
    public float healthIncreasePerRound = 5f;
    public float damageIncreasePerRound = 2f;
    private float health;
    private float damage;
    public int dropPercentage;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    
    [SerializeField]
    private GameObject currencyPrefab;

    private GameObject currencyHolder;
    
    private GameObject enemies;

    private NavMeshAgent navMeshAgent;

    public ObstacleAvoidanceType AvoidanceType;

    // textmeshprougui with damage on hit
    [SerializeField]
    private TextMeshProUGUI damageText;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = baseHealth;
        damage = baseDamage;
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");

        navMeshAgent.obstacleAvoidanceType = AvoidanceType;
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        
        // Generate random float move speed between 1 and 3 with different random seed for each enemy
        moveSpeed = Random.Range(1f,3f);
        navMeshAgent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform == null)
            return;
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            navMeshAgent.enabled = false;
            moveSpeed = 0;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }
        
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Atack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.1f )
            {
                registeredHit = false;
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                alreadyAttacked = false;
                registeredHit = false;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && !registeredHit)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 1.5)
                {
                    playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(damage);
                }
                registeredHit = true;
            }
        }

        animator.speed = 1;
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Run") && moveSpeed > 0)
        {
            animator.speed = moveSpeed / 3.0f;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > 1.5){
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(playerTransform.position);         
            transform.LookAt(playerTransform);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            // transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_attacking", false);
            animator.SetBool("is_running", true);
            
        } else {
            navMeshAgent.enabled = false; 
            animator.SetBool("is_running", false);
            animator.SetBool("is_attacking", true);
        }

        // damage text position equal to transform position with y offset of 0.5
        damageText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        // damage text rotation equal to transform rotation with 180 degree offset
        damageText.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));
    }

    public void TakeDamage(float damage)
    {
        if(!animator.GetBool("is_dead")){
            health -= damage;

            // change text o textmeshprougui with damage on hit
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
        playerTransform.GetComponent<StarterAssets.ThirdPersonController>().AddWeaponCurrency(10);
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

    //on collision enter, if tag is "Player" then ignore collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ayo");
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    public void setStats(int roundNum){
        health = baseHealth + healthIncreasePerRound * (roundNum - 1);
        damage = baseDamage + damageIncreasePerRound * (roundNum - 1);
    }
}
