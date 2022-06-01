
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;


    //private WaveSpawner waveSpawner;

    public int moveSpeed;
    //float offset;
    Vector3 offset;

    public float health;
    public int dropPercentage;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    float timeSinceAttack = 0.0f;
    
    [SerializeField]
    private GameObject currencyPrefab;

    private GameObject currencyHolder;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");
        //waveSpawner = GameObject.Find("WaveSpawner").GetComponent<WaveSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            moveSpeed = 0;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }
        
        if (alreadyAttacked)
        {
            timeSinceAttack += Time.deltaTime;
            if (timeSinceAttack > 2.0f)
            {
                timeSinceAttack = 0.0f;
                alreadyAttacked = false;
                registeredHit = false;
            }
            else if (timeSinceAttack > 1.0f && !registeredHit)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 1.5)
                {
                    playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(10);
                }
                registeredHit = true;
            }
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > 1.5){            
            transform.LookAt(playerTransform);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_attacking", false);
            animator.SetBool("is_running", true);
            
        } else {
            alreadyAttacked = true;
            animator.SetBool("is_running", false);
            animator.SetBool("is_attacking", true);
        }
    }

    public void TakeDamage(float damage)
    {
        if(!animator.GetBool("is_dead")){
            health -= damage;
            if (health <= 0){
                Die();
            }
        }
    }

    private void Die()
    {
        //waveSpawner.UpdateNumEnemiesAlive();
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

}
