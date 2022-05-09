
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    int moveSpeed = 3;
    //float offset;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Third Person Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, playerTransform.position) > 3){
            transform.LookAt(playerTransform);

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("is_attacking", false);
            animator.SetBool("is_running", true);
            
        } else {
            animator.SetBool("is_running", false);
            animator.SetBool("is_attacking", true);
            Debug.Log("Attacking");

        }


    }
}
