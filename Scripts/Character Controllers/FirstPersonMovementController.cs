using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class FirstPersonMovementController : MonoBehaviour
{
    // Private variables
    public static FirstPersonMovementController instance;

    bool grounded = true;
    //bool lookingAtInteractible = false;
    float verticalLookRotation;
    //Vector3 vel;
    public Transform cameraTransform;
    //Rigidbody rb;
    //CapsuleCollider capCollider;
    private PlayerInputs input;
    PlayerInput inputManager;
    private float speed;
    private float targetRotation = 0.0f;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    public Vector3 targetDirection { get; private set; }
    private float fallTimeoutDelta;
    private bool hasAnimator;
    public Animator animator;
    bool hasUsedDoubleJump;
    

    // System variables
    
    bool debugStopMovement;
    public CharacterController controller { get; private set; }
    CharacterSkills skillsController;
    public GameObject interactTooltipContainer;
    public TMP_Text interactPrompt;
    private GameObject mainCamera;
    bool inHubWorld;
    public Image crosshair;
    public float percentageMovementOfMax = 0.0f;
    CinemachineImpulseSource cineImpulse;

    void Awake()
    {
        instance = this;
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
        Cursor.lockState = CursorLockMode.Locked;
        //cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        skillsController = GetComponent<CharacterSkills>();
        input = GetComponent<PlayerInputs>();
        inputManager = GetComponent<PlayerInput>();
        fallTimeoutDelta = fallTimeout;
        cineImpulse = GetComponent<CinemachineImpulseSource>();
        if (SceneManager.GetActiveScene().name == "HomeArea") inHubWorld = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        hasAnimator = animator;
        if(hasAnimator)
            animator.ResetTrigger("Landing");
    }

    void Update()
    {
        cineImpulse.m_ImpulseDefinition.m_AmplitudeGain = CharacterStatisticsController.screenShake == true ? 2 : 0;
        cineImpulse.m_ImpulseDefinition.m_FrequencyGain = CharacterStatisticsController.screenShake == true ? 3 : 0;
        if (inHubWorld) if (HubWorldController.instance.paused) return;
        if (!inHubWorld) if (GameController.instance.paused || GameController.instance.playerDead) return;

        if (input.jump)
        {
            _lastJumpPressed = Time.time;
        }
        RunCollisionChecks();

        CalculateJumpApex();
        CalculateJump();
        

        PlayerMovement();
        CastForward();
    }

    private void LateUpdate()
    {
        if (inHubWorld) if (HubWorldController.instance.paused) return;
        if (!inHubWorld) if (GameController.instance.paused || GameController.instance.playerDead) return;
        MouseLook();
    }

    [Header("MOVEMENT")]
    [Tooltip("The velocity bonus provided by reaching the jump apex")]
    [SerializeField] private float apexBonus = 50;
    [Tooltip("The gravity applied when falling - must be negative")]
    public float gravity = 9.81f;
    [Tooltip("The maximum jump height")]
    [SerializeField] private float jumpHeight = 10;
    [Tooltip("The default horizontal velocity")]
    public float moveSpeed = 5.335f;
    [Tooltip("The horizontal velocity when sprinting")]
    public float sprintSpeed = 5.335f;
    [Tooltip("The multiplier to horizontal velocity")]
    public float movementModifier = 1f;
    [Tooltip("Movement acceleration and deceleration")]
    public float speedChangeRate = 20.0f;
    [Tooltip("The time after falling before which the player is marked as falling for animation purposes")]
    public float fallTimeout = 0.15f;

    /// <summary>
    /// Moves the player based on current input, varying dependant on sprint state, use of a gamepad. Also changes animations.
    /// </summary>
    private void PlayerMovement()
    {
        float targetSpeed = (input.sprint ? sprintSpeed : moveSpeed) * movementModifier;
        bool walking = false;
        bool running = false;

        if (input.sprint) running = true;

        if (input.move == Vector2.zero)
        {
            targetSpeed = 0.0f;
            walking = false;
            running = false;
        } else
        {
            walking = true;
        }

        if(hasAnimator)
        {
            animator.SetBool("Walking", walking);
            animator.SetBool("Running", running);
        }
        

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = !inputManager.currentControlScheme.Equals("KeyboardMouse") ? input.move.magnitude : 1f;

        if (targetSpeed != 0.0f)
        {
            var _apexBonus = Mathf.Sign(currentHorizontalSpeed) * apexBonus * apexPoint;
            currentHorizontalSpeed += _apexBonus * Time.deltaTime;
        }

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // apply speed to non-linear gradient
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        }

        targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        if (speed == 0) percentageMovementOfMax = 0;
        if(speed != 0) percentageMovementOfMax = speed / targetSpeed;

        // move the player
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    [Header("JUMP")]
    [Tooltip("Clamp the fall velocity")]
    [SerializeField] private float fallClamp = -12f;
    [Tooltip("The maximum jump apex value")]
    [SerializeField] private float _jumpApexThreshold = 10f;
    [Tooltip("The time after walking off an edge where the player can still jump")]
    [SerializeField] private float coyoteTimeThreshold = 0.1f;
    [Tooltip("The time before landing within which a player can line up a second jump")]
    [SerializeField] private float _jumpBuffer = 0.1f;

    public bool jumpingThisFrame { get; private set; }

    // private jump variables
    private bool _coyoteUsable;
    //private bool _endedJumpEarly = true;
    private float apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool canUseCoyote => _coyoteUsable && !collisionDown && timeLeftGrounded + coyoteTimeThreshold > Time.time;
    private bool hasBufferedJump => grounded && _lastJumpPressed + _jumpBuffer > Time.time;

    private void CalculateJumpApex()
    {
        if (!collisionDown)
        {
            // Gets stronger the closer to the top of the jump
            apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(controller.velocity.y));
        }
        else
        {
            apexPoint = 0;
        }
    }

    /// <summary>
    /// Performs any jump action, with limitations based on contact with the ground, buffered jumps, double jumping and coyote time.
    /// </summary>
    private void CalculateJump()
    {
        if (grounded || canUseCoyote || (CharacterStatisticsController.hasDoubleJump && !hasUsedDoubleJump))
        {
            

                // reset the fall timeout timer
                fallTimeoutDelta = fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f && grounded)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && canUseCoyote || hasBufferedJump)
            {
                _coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                jumpingThisFrame = true;
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (hasAnimator)
                {
                    animator.ResetTrigger("Landing");
                    animator.SetTrigger("Jump");
                    animator.SetBool("Jumping", true);
                }
                input.jump = false;
            } else if(input.jump && CharacterStatisticsController.hasDoubleJump && !hasUsedDoubleJump && !grounded) {
                hasUsedDoubleJump = true;
                _coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                jumpingThisFrame = true;
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (hasAnimator)
                {
                    animator.ResetTrigger("Landing");
                    animator.SetTrigger("Jump");
                    animator.SetBool("Jumping", true);
                }
                input.jump = false;
            } else
            {
                jumpingThisFrame = false;
            }
        }
        else
        {

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (collisionUp)
        {
            if (verticalVelocity > 0) verticalVelocity = 0;
        }

        if (verticalVelocity < fallClamp) verticalVelocity = fallClamp;
    }

    [Header("ROTATION")]
    [Tooltip("The horizontal mouse sensitivity multiplier")]
    public float mouseSensX = 1;
    [Tooltip("The vertical mouse sensitivity multiplier")]
    public float mouseSensY = 1;
    [Tooltip("The minimum and maximum angles the player can rotate vertically")]
    public Vector2 lookAngleMinMax = new Vector2(-75, 80);

    public Vector3 cameraTrans { get; private set; }
    public Vector3 cameraRot { get; private set; }
    public bool lookingAtInteractible = true;
    public bool lookingAtEnemy;

    /// <summary>
    /// Moves the camera in relation to mouse or gamepad input. If using a gamepad, the sensitivity of movement is halved, and halved again if looking at an enemy
    /// in order to support a basic level of aim assist.
    /// </summary>
    private void MouseLook()
    {
        float sensitivity = CharacterStatisticsController.sensitivity;
        string inputScheme = inputManager.currentControlScheme;
        if (!inputScheme.Equals("KeyboardMouse"))
        {
            sensitivity = sensitivity * .75f;
        }
            if (!inputScheme.Equals("KeyboardMouse") && lookingAtEnemy)
        {
            sensitivity = sensitivity * .5f;
        }
        transform.Rotate(Vector3.up * input.look.x * Time.deltaTime * sensitivity);
        verticalLookRotation -= input.look.y * Time.deltaTime * sensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, lookAngleMinMax.x, lookAngleMinMax.y);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
        cameraRot = cameraTransform.forward;
    }

    /// <summary>
    /// Casts forward a ray to detect what the player is looking at exactly.
    /// </summary>
    private void CastForward()
    {
        Ray ray = new Ray(cameraTransform.position, cameraRot);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2, LayerMask.GetMask("Interactible")))
        {
            lookingAtInteractible = true;
            PlayerInteract(hit);
            interactTooltipContainer.SetActive(true);
            interactPrompt.text = hit.collider.GetComponent<Interactible>().toolTip;
            crosshair.color = new Color(224, 178, 0);
        }
        else
        {
            if(lookingAtInteractible)
            {
                interactTooltipContainer.SetActive(false);
                interactPrompt.text = "Interact";
                crosshair.color = Color.white;
                lookingAtInteractible = false;
            }
            
        }
    }

    /// <summary>
    /// Sends a message triggering any interact method on an object the player is looking at.
    /// </summary>
    /// <param name="hit"></param>
    private void PlayerInteract(RaycastHit hit)
    {
        if (input.interact)
            hit.collider.gameObject.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);

        input.interact = false;
    }

    [Header("COLLISION")]
    [Tooltip("Useful for rough ground")]
    public float groundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;
    //bool landingThisFrame;
    bool collisionDown, collisionUp;
    float timeLeftGrounded;

    /// <summary>
    /// Checks whether the player is in contact with either the ground or ceiling, and modifiers animations.
    /// </summary>
    void RunCollisionChecks()
    {
        //landingThisFrame = false;
        bool groundedCheck = CheckGrounded();
        if (collisionDown && !groundedCheck)
        {
            timeLeftGrounded = Time.time;
        }
        else if (!collisionDown && groundedCheck)
        {
            _coyoteUsable = true; // Only trigger when first touching
            if(hasAnimator) {
                animator.SetBool("Jumping", false);
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Landing");
            }
            
            MovementAnimationSoundEvents.instance.landSource.Play();
        }

        collisionDown = groundedCheck;
        collisionUp = CheckUpCollision();
        grounded = groundedCheck;
        if(hasUsedDoubleJump && grounded)
        {
            hasUsedDoubleJump = false;
        }
    }

    private bool CheckGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private bool CheckUpCollision()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundedOffset, transform.position.z);
        return Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }
}