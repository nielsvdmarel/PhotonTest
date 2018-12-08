using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    [Header("Needed components")]
    [SerializeField]
    private Rigidbody rb;
    public Animator anim;
    public Camera cam;
    public CharacterController controller;
    [SerializeField]
    private CapsuleCollider GravityCollider;
    [Header("Values")]
    public float InputX;
    public float InputZ;
    private float WalkingRunning;
    public Vector3 desiredMoveDirection;
    public float desiredRoationSpeed;
    public float Speed;
    public float allowPlayerRotation;
    private float verticalVel;
    private Vector3 moveVector;
    private float max = 1f;
    private float min = 0.0f;
    private Vector3 JumpDirections;
    [SerializeField]
    private float JumpPower;
    [SerializeField]
    private float forwardPower;
    [Header("Momentum booleans")]
    [SerializeField]
    private bool UsePhysics;
    [SerializeField]
    private bool isjumpping = false;
    [SerializeField]
    private bool Freemove = true;
    [SerializeField]
    private bool IsCrouching = false;
    [SerializeField]
    private bool JumpRun = false;
    [SerializeField]
    private bool JumpRunGreaterHight = false;
    public bool blockRotationPlayer;
    [SerializeField]
    private bool IsPointedGrounded;
    [Header("Surounding Check")]
    [SerializeField]
    private GameObject NormalJumpCheckObject;
    [SerializeField]
    private GameObject FaultCheck;
    [SerializeField]
    private GameObject WalldetectionObject;
    [SerializeField]
    private GameObject GroundCheckDistance;
    [SerializeField]
    private float NormalJumpCheckObjectFloat;
    [SerializeField]
    private float WallCheckFloat;
    [SerializeField]
    private float FaultWallCheckFloat;
    [SerializeField]
    private float GroundCheckFloat;
    [SerializeField]
    private bool HighJump = false;
    [SerializeField]
    private bool NearGround;
    [SerializeField]
    private bool IsStandingNearWall;
    [SerializeField]
    private float crouchControl;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        GravityCollider = this.GetComponent<CapsuleCollider>();
        IsPointedGrounded = false;
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        WalkingRunning = 0;
    }

    void Update()
    {
        CheckSuroundingValues();
        CheckForJumpRunInput();
        WalkingRunningChecker();
        UsePhysicsMovement();
        keepGrounded();
        FallingLanding();
        CheckChrouching();
        PhysicsFallingFunction();
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            blockRotationPlayer = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            blockRotationPlayer = false;
        }
        InputMagnitude();

        //CheckForSingleJumpInput();

    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
        if (!UsePhysics) // niet netjes wtf niels, blijkbaar werkt alleen root motion uitzetten al niet, dus je moet maar hard coded de kut movement uit zetten die dus blijkbaar ook nog af hangt van een of andere input?? 
        {
            desiredMoveDirection = forward * InputZ + right * InputX;

            if (blockRotationPlayer == false)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRoationSpeed);
            }
        }
    }

    void InputMagnitude()
    {
        //calculate input vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", InputZ, 0.0f, Time.deltaTime * 2);
        anim.SetFloat("InputX", InputX, 0.0f, Time.deltaTime * 2);

        //calculate the input magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //physically move player
        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, 0f, Time.deltaTime);   //origineel stond de waarde na speed op 0.0f
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, .5f, Time.deltaTime);   //origineel stond de waarde na speed op 0.0f
        }

        anim.SetFloat("Speed", WalkingRunning);

    }

    void keepGrounded()
    {
        if (IsPointedGrounded)
        {
            bool isGrounded;
            isGrounded = controller.isGrounded;
            if (isGrounded)
            {
                verticalVel -= 0;
            }
            else
            {
                verticalVel -= 2;
            }
            moveVector = new Vector3(0, verticalVel, 0);
            controller.Move(moveVector);
        }
    }

    void CheckForJumpRunInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))  // && Input.GetKey(KeyCode.LeftShift)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !JumpRunGreaterHight && !JumpRun && GroundCheckFloat < 4)
            {
                if (HighJump == false)
                {
                    Vector3 direction = rb.transform.position - transform.position;
                    IsPointedGrounded = false;
                    blockRotationPlayer = true;
                    anim.SetBool("JumpRun", true);
                    JumpRun = true;
                }
                else if (HighJump == true && !IsStandingNearWall)
                {
                    Debug.Log("HighJump");
                    UsePhysics = true;
                    anim.SetBool("JumpRunGreaterHeight", true);
                    anim.SetBool("LongJump", true);
                    rb.AddForce(transform.up * JumpPower, ForceMode.Impulse);
                    rb.AddForce(transform.forward * forwardPower, ForceMode.Impulse);
                    blockRotationPlayer = true;
                    JumpRunGreaterHight = true;
                    IsPointedGrounded = false; // effe kieken
                }
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                blockRotationPlayer = true;
                anim.SetBool("SmallJump", true);
                JumpRun = true;
            }
        }

        if (JumpRun)
        {

            IsPointedGrounded = false;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpNew"))
            {
                Debug.Log("JumpRun is now playing");
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    JumpRun = false;
                    anim.SetBool("JumpRun", false);
                    Debug.Log("after on jump return to normal anim");
                    //IsPointedGrounded = true;
                    blockRotationPlayer = false;
                }
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpingNoSpeed"))
            {
                Debug.Log("JumpRun is now playing");
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                {
                    JumpRun = false;
                    anim.SetBool("SmallJump", false);
                    Debug.Log("after on jump return to normal anim");
                    //IsPointedGrounded = true;
                    blockRotationPlayer = false;
                }
            }
        }
    }

    void WalkingRunningChecker()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            WalkingRunning += 1.3f * Time.deltaTime;
            if (WalkingRunning > max)
            {
                WalkingRunning = 1;
                //aah lelijk niels
            }
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            WalkingRunning -= .3f * Time.deltaTime;
            if (WalkingRunning < min)
            {
                //aah lelijk niels
                WalkingRunning = 0;
            }
        }
    }

    void UsePhysicsMovement()
    {
        if (UsePhysics)
        {
            GravityCollider.enabled = true;
            IsPointedGrounded = false;
            controller.enabled = false;
            anim.applyRootMotion = false;
            rb.useGravity = true;
        }

    }

    void RootMotionMovement()
    {
        GravityCollider.enabled = false;
        controller.enabled = true;
        anim.applyRootMotion = true;
        rb.useGravity = false;
    }

    void CheckChrouching()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (IsCrouching)
            {
                IsCrouching = false;
            }
            else if (!IsCrouching)
            {
                IsCrouching = true;
            }
        }
        if (crouchControl > 0 && !IsCrouching)
        {
            crouchControl -= 4f * Time.deltaTime;
            anim.SetLayerWeight(1, crouchControl);
        }

        if (crouchControl < 1 && IsCrouching)
        {
            crouchControl += 4f * Time.deltaTime;
            anim.SetLayerWeight(1, crouchControl);
        }
    }

    void FallingLanding()
    {
        if (JumpRunGreaterHight)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Falling Idle"))
            {
                if (GroundCheckFloat < 2f)
                {

                    JumpRunGreaterHight = false;
                    anim.SetBool("JumpRunGreaterHeight", false);
                    Debug.Log("Nu overgaan");
                    anim.SetFloat("HardLanding", 0);
                    anim.SetBool("LongJump", false);
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        UsePhysics = false;
                        RootMotionMovement();
                        anim.SetBool("Freemove", true);
                        anim.SetBool("JumpRunGreaterHeight", false);
                        blockRotationPlayer = true;
                        //IsPointedGrounded = true;
                    }
                }
            }
        }

        if (anim.GetNextAnimatorStateInfo(0).IsName("FreeMove2")) // wtf niels elke keer als animatie state FreeMove2 de volgende state is, wordt rotatie aangezet? wtfffff
        {
            Debug.Log("nu weer rotaten");
            blockRotationPlayer = false;
        }
    }

    void CheckSuroundingValues()
    {
        RaycastHit JumpCheck;
        RaycastHit GroundCheckHit;
        RaycastHit FaultCheckHit;
        RaycastHit WallDistanceCheckHit;
        if (Physics.Raycast(NormalJumpCheckObject.transform.position, Vector3.down, out JumpCheck))
        {
            NormalJumpCheckObjectFloat = JumpCheck.distance;
        }
        if (NormalJumpCheckObjectFloat > 1.4) // DIT WORDT WAARDE VAN GROND MIN, GROND IS DUS VERDER WEG DAN KLEINE JUMP
        {
            HighJump = true;
        }

        else
        {
            HighJump = false;
        }

        if (Physics.Raycast(GroundCheckDistance.transform.position, Vector3.down, out GroundCheckHit))
        {
            GroundCheckFloat = GroundCheckHit.distance;
        }
        if (NormalJumpCheckObjectFloat < 1.4f) // DIT WORDT WAARDE VAN GROND MIN, GROND IS DUS VERDER WEG DAN KLEINE JUMP
        {
            NearGround = true;
        }
        else
        {
            NearGround = false;
        }

        if (Physics.Raycast(WalldetectionObject.transform.position, transform.forward, out WallDistanceCheckHit))
        {
            WallCheckFloat = WallDistanceCheckHit.distance;
        }
        if (WallCheckFloat < 1.1) // DIT WORDT WAARDE VAN GROND MIN, GROND IS DUS VERDER WEG DAN KLEINE JUMP
        {
            IsStandingNearWall = true;
        }
        else
        {
            IsStandingNearWall = false;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GravityCollider.enabled = true)
        {
            if (collision.gameObject.tag == "Player")
            {
                Physics.IgnoreCollision(collision.collider, GravityCollider);
            }
        }
    }

    private void PhysicsFallingFunction()
    {
        if (!JumpRunGreaterHight)
        {
            if (!controller.isGrounded)
            {
                if (GroundCheckFloat > 1.7)
                {
                    //IsPointedGrounded = false;
                    anim.SetBool("NormalIdelFall", true);
                    UsePhysics = true;
                }

                else if (GroundCheckFloat < 1.7)
                {
                    //IsPointedGrounded = true;
                    Debug.Log("werkt met hoogte");
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        Debug.Log("nu weer kunnen bewegen");
                        anim.SetBool("Freemove", true);
                        UsePhysics = false;
                        RootMotionMovement();
                        anim.SetBool("NormalIdelFall", false);
                    }
                }
            }
        }
    }
}
