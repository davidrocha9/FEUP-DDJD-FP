using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Character's Health")]
        public float Health = 100.0f;
        public float MaxHealth = 100.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public float sensitivity = 1f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.5f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        public int currencyCounter = 0;
        public GameObject WeaponShopUI;
        

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDRun;
        private int _animIDShoot;
        

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private HealingOverTime healingOverTime;

        [SerializeField]
        private CinemachineVirtualCamera aimCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        public bool interact = false;

        public bool reload = false;

        private bool startRound = false;

        private int nextRound = 1;

        private bool rotateWhenMoving = true;

        private bool canFire = true;
        private float fireTimer = 0f;
        private float shootingSpread = 15f;

        private Trigger trigger;
        private ArenaTrigger arenaTrigger;

        [SerializeField]
        private WaveSpawner waveSpawner;

        [SerializeField]
        private Slider healthBar;

        [SerializeField]
        private Text currencyAmountUI;

        private Vector2 screenCenter;

        [SerializeField]
        private LayerMask aimColliderMask;

        [SerializeField]
        private GameObject blade;
        
        [SerializeField]
        private GunBehaviour PS;

        [SerializeField]
        private GunBehaviour AR;

        [SerializeField]
        private GunBehaviour SG;

        [SerializeField]
        private GunBehaviour RL;

        private List<GunBehaviour> gunArsenal = new List<GunBehaviour>();

        private int selectedGun = 0;

        [SerializeField]
        private GameObject bloodOverlay;

        private ArenaTrigger onDeathTrigger;
        private bool foundArenaTrigger = false;

        private bool playerGotHit = false;
        private bool heartBeat = false, heartBeatUp = true;
        private float heartBeatRatio = 1.0f;
        private int hitsNumber = 0;
        private int weaponCurrency = 0;
        private float fallingY = -1234.56789f;
        private float teleportedTime = 0f;
        private bool recentlyTeleported = false;

        private GameObject[] rumblePlanes;

        MeshRenderer ARMesh = null;
        MeshRenderer SGMesh = null;
        MeshRenderer RLMesh = null;

        // Sound Variables

        private FMOD.Studio.EventInstance walkSound;
        private bool walking = false;

        private bool jumping = false;

        private FMOD.Studio.PLAYBACK_STATE pbState;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {

            screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            healingOverTime = GetComponent<HealingOverTime>();
            walkSound = FMODUnity.RuntimeManager.CreateInstance("event:/Project/General Sounds/Character Related/Footsteps/Grass");
            //string[] names = {"Default", "Enemy"};
            //aimColliderMask = LayerMask.GetMask(names);

            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            AssignAnimationIDs();
            _animator.SetBool("Pistol", true);

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            //PS.ApplyUpgrades() // should apply upgrades before adding to the arsenal
            //AR.ApplyUpgrades()
            //SG.ApplyUpgrades()
            //RL.ApplyUpgrades()

            gunArsenal.Add(PS);
            //gunArsenal.Add(AR);
            //gunArsenal.Add(AR);
            //gunArsenal.Add(SG);
            //gunArsenal.Add(RL);
            gunArsenal[selectedGun].gameObject.SetActive(true);
            // find all objects with the tag RumblePlane
            rumblePlanes = GameObject.FindGameObjectsWithTag("RumblePlane");
            // select one of the rumble planes to spawn the player at
            if(rumblePlanes != null && rumblePlanes.Length != 0){
                int random = Random.Range(0, rumblePlanes.Length);
            // spawn the player at the selected rumble plane
                transform.position = rumblePlanes[random].gameObject.GetComponent<RumblePlane>().getSpawnPosition();
            }
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            Aim();
            Fire();
            WeaponShop();
            Interact();
            SwitchWeapon();
            StartRound();
            Reload();

            if(recentlyTeleported && Time.time - teleportedTime > 0.5f){
                for(int i = 0; i<rumblePlanes.Length; i++){
                    rumblePlanes[i].gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
                recentlyTeleported = false;
            }


            if (!canFire){
                fireTimer += Time.deltaTime;
                if (fireTimer >= 0.3f){
                    
                    canFire = true;
                    fireTimer = 0;
                }
            }
            if (playerGotHit)
            {
                Color temp = bloodOverlay.GetComponent<Image>().color;
                temp.a -= 0.005f;
                bloodOverlay.GetComponent<Image>().color = temp;
                
                if (temp.a < 0)
                {
                    playerGotHit = false;
                    bloodOverlay.SetActive(false);
                    hitsNumber--;
                }
            }

            if (heartBeat)
            {
                if (Health > 20)
                {
                    heartBeat = false;
                    playerGotHit = true;
                    return;
                }

                if (heartBeatRatio >= 1.0f || heartBeatRatio <= 0.0f)
                {
                    heartBeatUp = !heartBeatUp;
                }
                
                if (heartBeatUp)
                {
                    heartBeatRatio += 0.01f;
                }
                else
                {
                    heartBeatRatio -= 0.01f;
                }

                Color temp = bloodOverlay.GetComponent<Image>().color;
                bloodOverlay.GetComponent<Image>().color = temp;
                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1.0f + Health / 100.0f + heartBeatRatio, 1.0f + Health / 100.0f + heartBeatRatio * 3, 1);
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDRun = Animator.StringToHash("Run");
            _animIDShoot = Animator.StringToHash("Shoot");
        }

        private void WeaponShop(){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            GameObject UItext = WeaponShopUI.transform.GetChild(0).gameObject;
            if (Physics.Raycast(ray, out hit, 10f, aimColliderMask))
            {
                if (hit.collider.gameObject.name == "AR")
                {
                    ARMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
 
                    if (!gunArsenal.Contains(AR))
                    {
                        if(weaponCurrency >= AR.weaponPrice){
                            UItext.GetComponent<Text>().text = "Press [E] to buy Tango-45 AR (Cost: " + AR.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            ARMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(AR);
                                weaponCurrency -= AR.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                            }
                        }
                        else{
                            ARMesh.material.color = Color.red;
                        }
                    }
                    else{
                        UItext.GetComponent<Text>().text = "Already acquired Tango-45 AR";
                        WeaponShopUI.SetActive(true);
                        ARMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "ARAmmo"){
                    
                    if(weaponCurrency >= AR.ammoPrice & gunArsenal.Contains(AR)){
                        UItext.GetComponent<Text>().text = "Press [E] to buy Tango-45 AR Ammo (Cost: " + AR.ammoPrice + ")";
                        WeaponShopUI.SetActive(true);
                        if(_input.interact){
                            if(AR.BuyAmmo(1)){                            
                                _input.interact = false;
                                weaponCurrency -= AR.ammoPrice;
                            }
                        }
                    }
                    else{
                        UItext.GetComponent<Text>().text = "Tango-45 AR not acquired";
                        WeaponShopUI.SetActive(true);
                    }
                }
                else if (hit.collider.gameObject.name == "SG")
                {
                    SGMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (!gunArsenal.Contains(SG))
                    {
                        if(weaponCurrency >= SG.weaponPrice){
                            UItext.GetComponent<Text>().text = "Press [E] to buy Baligant XU772 SG (Cost: " + SG.weaponPrice + ")";
                            WeaponShopUI.SetActive(true);
                            SGMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(SG);
                                weaponCurrency -= SG.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                            }
                        }
                        else{
                            
                            SGMesh.material.color = Color.red;
                        }
                    }
                    else{
                        UItext.GetComponent<Text>().text = "Already acquired Baligant XU772 SG";
                        WeaponShopUI.SetActive(true);
                        SGMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "SGAmmo"){
                    if(weaponCurrency >= SG.ammoPrice & gunArsenal.Contains(SG)){
                        UItext.GetComponent<Text>().text = "Press [E] to buy Baligant XU772 SG Ammo (Cost: " + SG.ammoPrice + ")";
                        WeaponShopUI.SetActive(true);
                        if(_input.interact){
                            _input.interact = false;
                            if(SG.BuyAmmo(1)){
                                weaponCurrency -= SG.ammoPrice;
                            }
                        }
                    }
                    else{
                        UItext.GetComponent<Text>().text = "Baligant XU772 SG not acquired";
                        WeaponShopUI.SetActive(true);
                    }
                }
                /*else if (hit.collider.gameObject.name == "RL")
                {
                    RLMesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (!gunArsenal.Contains(RL))
                    {
                        if(weaponCurrency >= RL.weaponPrice){
                            RLMesh.material.color = Color.green;
                            if (_input.interact)
                            {
                                gunArsenal.Add(RL);
                                weaponCurrency -= AR.weaponPrice;
                                gunArsenal[selectedGun].gameObject.SetActive(false);
                                selectedGun = gunArsenal.Count - 1;
                                gunArsenal[selectedGun].gameObject.SetActive(true);
                            }
                        }
                        else{
                            RLMesh.material.color = Color.red;
                        }
                    }
                    else{
                        RLMesh.material.color = Color.red;
                    }
                }
                else if (hit.collider.gameObject.name == "RLAmmo"){
                    if(weaponCurrency >= RL.ammoPrice & gunArsenal.Contains(RL)){
                        if(_input.interact){
                            _input.interact = false;
                            if(RL.BuyAmmo(1)){
                                weaponCurrency -= RL.ammoPrice;
                            }
                        }
                    }
                }*/
                else{
                    if(ARMesh != null){
                        ARMesh.material.color = AR.GetComponent<MeshRenderer>().material.color;
                    }
                    if(SGMesh != null){
                        SGMesh.material.color = SG.GetComponent<MeshRenderer>().material.color;
                    }
                    /*if(RLMesh != null){
                        RLMesh.material.color = RL.GetComponent<MeshRenderer>().material.color;
                    }*/
                    WeaponShopUI.SetActive(false);
                }
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed =  SprintSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero){
                walkSound.getPlaybackState(out pbState);
                targetSpeed = 0.0f;
                // Parar o som
                walking = false;
                if (pbState == FMOD.Studio.PLAYBACK_STATE.PLAYING){
                    //Debug.Log("Sound stopping");
                    //walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            } else {
                if (!walking){
                    //Debug.Log("Sound starting");
                    walking = true;
                    // Comecar som
                    FMODUnity.RuntimeManager.AttachInstanceToGameObject(walkSound, transform);
                    walkSound.start();
                    walkSound.release();
                }
            }


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                if (rotateWhenMoving){
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                if (_animationBlend >= 1)
                {
                    //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Run"));
                    _animator.SetBool(_animIDRun, true);
                }
                else {
                    //Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Run"));
                    _animator.SetBool(_animIDRun, false);
                }
                
                //_animator.SetFloat(_animIDSpeed, _animationBlend);
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void Interact()
        {
            if (_input.interact && !interact){
                interact = true;
                checkTrigger();
            } else {
                interact = false;
                _input.interact = false;
            }
        }

        private void Reload()
        {
            if (_input.reload && !reload){
                reload = true;
                gunArsenal[selectedGun].Reload();
            } else {
                reload = false;
                _input.reload = false;
            }
        }

        private void StartRound()
        {
            /*if (_input.startRound && !startRound){
                startRound = true;
                //gunArsenal[selectedGun].FillAmmo();
                nextRound = waveSpawner.StartRound(nextRound);
            } else {
                startRound = false;
                _input.startRound = false;
            }*/
        }

        private void Aim()
        {
            if (_input.aim) {
                SprintSpeed = 2.0f;
                sensitivity = 0.5f;
                aimCamera.gameObject.SetActive(true);
                //_animator.SetBool(_animIDRun, false);
                rotateWhenMoving = false;

                Vector3 mouseGlobalPosition = Vector3.zero;

                // Rotate the player to face where he is aiming
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 999f, aimColliderMask)){
                    mouseGlobalPosition = hit.point;
                }

                mouseGlobalPosition.y = transform.position.y;
                Vector3 aimDir = (mouseGlobalPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
                
                _animator.SetBool("Aiming", true);

            } else {
                SprintSpeed = 5.335f;
                sensitivity = 1f;
                aimCamera.gameObject.SetActive(false);
                _animator.SetBool("Aiming", false);
                //_animator.SetBool(_animIDShoot, false);
                //_animator.SetBool(_animIDRun, true);
                rotateWhenMoving = true;
            }
        }

        public void AddWeaponCurrency(int ammount){
            weaponCurrency+=ammount;
        }

        private void Fire()
        {
            if (_input.fire) {
                rotateWhenMoving = false;

                Vector3 mouseGlobalPosition = Vector3.zero;

                Vector2 shootingSpreadVec = new Vector2(0, 0);
                if (!_input.aim){
                    if (!Grounded){
                        shootingSpreadVec = new Vector2(Random.Range(-gunArsenal[selectedGun].shootingSpread*2, gunArsenal[selectedGun].shootingSpread*2), Random.Range(-gunArsenal[selectedGun].shootingSpread*2, gunArsenal[selectedGun].shootingSpread*2));
                    } else {
                        shootingSpreadVec = new Vector2(Random.Range(-gunArsenal[selectedGun].shootingSpread, gunArsenal[selectedGun].shootingSpread), Random.Range(-gunArsenal[selectedGun].shootingSpread, gunArsenal[selectedGun].shootingSpread));
                    }
                }

                // Rotate the player to face where he is aiming
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 999f, aimColliderMask)){
                    mouseGlobalPosition = hit.point;
                }
                
                mouseGlobalPosition.y = transform.position.y;
                Vector3 aimDir = (mouseGlobalPosition - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 20f);
                _animator.SetBool(_animIDShoot, true);
                if(canFire){
                    gunArsenal[selectedGun].Shoot(shootingSpreadVec);
                    canFire = false;
                }
            }
            else{
                if(!_animator.GetBool("Aiming")) rotateWhenMoving = true;
                _animator.SetBool(_animIDShoot, false);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                if(fallingY != -1234.56789f){
                    float distanceFallen = fallingY - transform.position.y;
                    fallingY = -1234.56789f;
                    //Debug.Log("You fell " + distanceFallen + " units");
                    if(distanceFallen > 4){
                        TakeDamage(1*Mathf.RoundToInt(distanceFallen));
                    }
                }
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    
                    
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    
                    if (_hasAnimator)
                    {
                        if(_animator.GetBool("Aiming")) _input.jump = false;
                        _animator.SetBool(_animIDJump, true);
                        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Jump")) _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    }

                    if (!jumping)
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/General Sounds/Character Related/Jump/Jump");
                    jumping = true;
                                    

                    // update animator if using character
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                if(!_animator.GetBool(_animIDFreeFall)){
                    fallingY = transform.position.y;
                }
                jumping = false;
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void SwitchWeapon(){
            if(_input.weapon1){
                _animator.SetBool("Pistol", true);
                selectedGun = 0;
            }
            else if(_input.weapon2){
                if(gunArsenal.Count >= 2){
                    _animator.SetBool("Pistol", false);
                    selectedGun = 1;
                }
            }
            else if(_input.weapon3){
                if(gunArsenal.Count >= 3){                    
                    _animator.SetBool("Pistol", false);
                    selectedGun = 2;
                }                
            }
            else if(_input.weapon4){
                if(gunArsenal.Count >= 4){
                    _animator.SetBool("Pistol", false);
                    selectedGun = 3;
                }                            
            }
            if(_input.weaponScroll != 0){
                selectedGun = (selectedGun + gunArsenal.Count + _input.weaponScroll) % gunArsenal.Count;
                if(selectedGun == 0){
                    _animator.SetBool("Pistol", true);
                }
                else{
                    _animator.SetBool("Pistol", false);
                }
            }
            for (int i = 0; i < gunArsenal.Count; i++)
            {
                if(i == selectedGun){
                    gunArsenal[selectedGun].gameObject.SetActive(true);
                    gunArsenal[selectedGun].updateReloadUI();
                }
                else{
                    gunArsenal[i].gameObject.SetActive(false);
                }
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Project/General Sounds/Character Related/Footstep/Footstep");
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            
        }

        private void checkTrigger()
        {
            if (arenaTrigger!= null){
                if((nextRound-1) % 5 == 0 && !startRound){
                    Debug.Log("You're okay to extract");
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Portal/Enter");
                    arenaTrigger.performAction();
                }
                else{
                    Debug.Log("You can only extract every 5 rounds");
                }
            }
            if (trigger!= null){
                Debug.Log("Entering Arena");
                FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Portal/Enter");
                trigger.performAction();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            trigger = other.GetComponent<Trigger>();
            arenaTrigger = other.GetComponent<ArenaTrigger>();
            if(!foundArenaTrigger){
                foundArenaTrigger = true;
                onDeathTrigger = arenaTrigger;
            }
            if (other.gameObject.tag == "Lava")
            {
                TakeDamage(Health-1);
                TakeDamage(Health);
            }
            if(other.gameObject.tag == "RumblePlane" && !recentlyTeleported){
                // find the index of the other object in the rumble planes array
                int randomRumblePlane = Random.Range(0, rumblePlanes.Length);
                while(rumblePlanes[randomRumblePlane].name == other.gameObject.name){
                    randomRumblePlane = Random.Range(0, rumblePlanes.Length);
                }
                // teleport the player to the randomRumblePlane
                _controller.enabled = false;
                transform.position = rumblePlanes[randomRumblePlane].gameObject.GetComponent<RumblePlane>().getTeleportPosition();
                _controller.enabled = true;
                other.isTrigger = false;
                teleportedTime = Time.time;
                recentlyTeleported = true;
            }
        }

        void OnTriggerExit(Collider other){
            trigger = null;
            arenaTrigger = null;
        }

        void OnParticleCollision(){
            TakeDamage(1);
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Die();
                return;
            }

            healingOverTime.PlayerTookDamage();
            
            UpdateHealthUI();

            if (Health > 20 && !heartBeat)
            {
                float scale;
                if (hitsNumber >= 5)
                {
                    scale = 0.0f;
                }
                else
                {
                    scale = 2.5f - hitsNumber * 0.5f;
                }
                
                
                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1);
                Color temp = bloodOverlay.GetComponent<Image>().color;
                if (hitsNumber >= 5)
                {
                    temp.a = 0.75f;
                }
                else
                {
                    temp.a = 0.5f + hitsNumber * 0.05f;
                }

                bloodOverlay.GetComponent<Image>().color = temp;

                bloodOverlay.SetActive(true);
                playerGotHit = true;
                hitsNumber++;
            }
            else
            {
                bloodOverlay.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Color temp = bloodOverlay.GetComponent<Image>().color;
                temp.a = 0.5f;
                bloodOverlay.GetComponent<Image>().color = temp;

                bloodOverlay.SetActive(true);
                heartBeat = true;
            }
        }

        public void Heal(float amount)
        {
            //Debug.Log("Healing");
            Health += amount;
            if (Health >= MaxHealth){
                Health = MaxHealth;
            }
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            healthBar.value = (int) Health;
        }

        private void Die()
        {
            Debug.Log("Player died");
            currencyCounter = 0;
            updateCurrencyUI();
            onDeathTrigger.performAction();
        }

        private void updateCurrencyUI()
        {
            currencyAmountUI.text = currencyCounter.ToString();
        }

        void OnControllerColliderHit(ControllerColliderHit hit){
            if(hit.gameObject.tag == "Currency"){
                CurrencyBehaviour currency = hit.transform.GetComponent<CurrencyBehaviour>();
                increaseCurrency();
                currency.DeSpawn();
            }
        }

        void increaseCurrency(){
            currencyCounter++;
            Debug.Log("Currrent Currency: " + currencyCounter);
            updateCurrencyUI();
        }
    }
}