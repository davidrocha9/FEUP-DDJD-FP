
using UnityEngine;

public class RangedEnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    public int moveSpeed;

    public float range = 10f;

    Vector3 offset;

    public float health;
    public int dropPercentage;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    
    [SerializeField]
    private GameObject currencyPrefab;

    private GameObject currencyHolder;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform == null)
            return;
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            moveSpeed = 0;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }
        
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Shooting"))
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
                    Debug.Log("hit");
                    //playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(10);
                }
                registeredHit = true;
            }
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > range + 10f){            
            transform.LookAt(playerTransform);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_shooting", false);
            animator.SetBool("is_walking", true);
            
        } else {
            animator.SetBool("is_walking", false);
            animator.SetBool("is_shooting", true);
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
        GetComponent<CapsuleCollider>().enabled = false;
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
