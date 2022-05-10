/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Debug.Log(direction);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);  
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            controller.Move(direction*speed*Time.deltaTime);
        }
    }
}*/

using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;

    private Rigidbody body;

    public Transform cam;

    public float speed = 6;
    public float decreaseSpeed = 0;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    Vector3 velocity;
    Vector2 movementRcvd;

    Vector3 lastDirection;

    bool isGrounded;

    [SerializeField]
    private Transform playerBody;

    [SerializeField]
    private GunBehaviour gun;
    
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    Animator playerAnimator;

    void Start(){
        playerAnimator = GetComponentInChildren<Animator>();
        playerAnimator.SetBool("is_running", false);
        playerAnimator.SetBool("is_shooting", false);
        body = GetComponent<Rigidbody>();
        lastDirection = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 1.6 && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void FixedUpdate(){
        MovePlayer();
    }

    void MovePlayer(){
        
        Vector3 direction = new Vector3(movementRcvd.x, 0f, movementRcvd.y).normalized;
        if (direction.z != 0f || direction.y != 0 || direction.x != 0){
            lastDirection = direction;
        }        

        if(direction.magnitude >= 0.1f)
        {
            decreaseSpeed = 6;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        } else {
            float targetAngle = Mathf.Atan2(lastDirection.x, lastDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            decreaseSpeed = decreaseSpeed * 0.957f;
            controller.Move(moveDir.normalized * decreaseSpeed * Time.deltaTime);
        }
        
    }

    void OnFire(InputValue input){

        if(input.Get() != null){
            //Debug.Log("Estou a disparar");
            playerAnimator.SetBool("is_shooting", true);
            gun.Shoot();
        }
        else{
            //Debug.Log("Parei de disparar");
            playerAnimator.SetBool("is_shooting", false);
        }
    }

    void OnMove(InputValue input){
        movementRcvd = input.Get<Vector2>();
        //Debug.Log(movementRcvd);
        if(movementRcvd.x == 0 && movementRcvd.y == 0){
            //Debug.Log("Parei de correr");
            playerAnimator.SetBool("is_running", false);
        }
        else{
            //Debug.Log("Comecei a correr");
            playerAnimator.SetBool("is_running", true);
        }
    }

    void OnJump(InputValue input){
        if (transform.position.y <= 1.9f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
}