using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public Camera camera;
    public GameObject interactIndicator;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public float movementSpeed;
    public float jumpHeight;
    public float gravityUp;
    public float gravityDown;
    public float turnSmoothTime;
    public float slopeForce;
    public float slopeRayLength;
    public float slopeLimit;
    public float slideFriction;
    public float slopeSlowDownCoefficient = 1f;

    public List<DecorationInfo> availableDecorations;
    private List<GameObject> decorationProjectiles;
    private int activeDecorationIndex;

    private bool canMove;

    [SerializeField] private float interactionDistance = 0.5f;
    private Interactable interactableFocus;

    [SerializeField] string snowballPrefabResource = "Decorations/Snowball";
    private GameObject snowballPrefab;

    private CharacterController characterController;
    private Animator animator;

    private float verticalSpeed = 0f;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private Vector3 slopeVelocity = Vector3.zero;
    private Vector3 hitNormal = Vector3.zero;

    private bool aimMode = false;

    private int collectibleLayer;
    private int mudLayer;

    private float mudTime;

    [SerializeField] public Transform bucketTransform;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        DialogueUI.EventResumePlayerControl += RemoveFocus;
        canMove = true;

        snowballPrefab = Resources.Load(snowballPrefabResource) as GameObject;

        collectibleLayer = LayerMask.NameToLayer("Collectible");
        mudLayer = LayerMask.NameToLayer("Mud");

        activeDecorationIndex = 0;
        decorationProjectiles = new List<GameObject>();
        foreach (DecorationInfo info in availableDecorations)
        {
            GameObject decorationPrefab = Resources.Load(info.projectileResource) as GameObject;
            decorationProjectiles.Add(decorationPrefab);
        }
        UIActions.EventActiveDecorationChanged?.Invoke(availableDecorations[activeDecorationIndex]);

        mudTime = 0.0f;
    }

    private void OnDestroy()
    {
        DialogueUI.EventResumePlayerControl -= RemoveFocus;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove || Time.timeScale == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIActions.EventPauseGame?.Invoke();
        }

        if (interactableFocus != null)
        {
            // Face interacting object
            //animator.SetBool("isRunning", false);
            //animator.SetBool("isFlying", false);
            Vector3 interactDir = (interactableFocus.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(interactDir.x, 0f, interactDir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);

            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            UIActions.EventUnlockCursor?.Invoke();
            aimMode = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            UIActions.EventLockCursor?.Invoke();
            aimMode = false;
        }

        if (aimMode && Input.GetMouseButtonDown(0))
        {
            shootSnow();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            activeDecorationIndex = (activeDecorationIndex + 1) % availableDecorations.Count;
            UIActions.EventActiveDecorationChanged?.Invoke(availableDecorations[activeDecorationIndex]);
        }

        // ground check
        RaycastHit groundHit;
        bool groundCheckResult = Physics.SphereCast(transform.position + Vector3.down * (characterController.height / 2.0f - characterController.skinWidth - characterController.radius),
                                                    characterController.radius,
                                                    Vector3.down,
                                                    out groundHit,
                                                    groundDistance,
                                                    groundMask);

        bool isOnSteepSlope = false;
        if (groundCheckResult)
            isOnSteepSlope = Vector3.Angle(Vector3.up, groundHit.normal) >= slopeLimit;

        if (!groundCheckResult)
        {
            isGrounded = false;
        }
        else if (!isGrounded && !isOnSteepSlope)
        {
            if (verticalSpeed <= 0)
                isGrounded = true;
        }

        // stay on ground
        if (isGrounded && verticalSpeed <= 0)
            verticalSpeed = -2f;

        // movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // slow down climbing slopes
            moveDir = moveDir.normalized * movementSpeed;

            if (isGrounded && Vector3.Dot(moveDir, groundHit.normal) < 0) // moving up slope
            {
                moveDir *= 1f + slopeSlowDownCoefficient * Vector3.Dot(moveDir.normalized, groundHit.normal);
            }
            // stop player from moving up a steep slope
            if (isOnSteepSlope)
            {
                Vector3 slopeDirH = -groundHit.normal; // horizontal direction up slope
                slopeDirH.y *= 0;
                if (Vector3.Dot(moveDir, slopeDirH) > 0) // player is moving partially up a steep slope
                {
                    moveDir -= Vector3.Dot(moveDir, slopeDirH.normalized) * slopeDirH;
                }
            }

            if (mudTime > 0) moveDir /= 5.0f;
            characterController.Move(moveDir * Time.deltaTime);
        }

        // slide down slope
        if (groundCheckResult && isOnSteepSlope)
        {
            slopeVelocity.x += (1f - groundHit.normal.y) * groundHit.normal.x * (1f - slideFriction);
            slopeVelocity.z += (1f - groundHit.normal.y) * groundHit.normal.z * (1f - slideFriction);
            // move away from slope
            characterController.Move(slopeVelocity * Time.deltaTime);
            slopeVelocity *= (1f - 0.1f * Time.deltaTime);
        }
        else
        {
            slopeVelocity = Vector3.zero;
        }

        // cling to slope
        if (isGrounded && verticalSpeed <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 * slopeRayLength))
                if (hit.normal != Vector3.up)
                    characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);
        }

        //jumping / interacting

        RaycastHit interactableHit;
        bool interactableCastResult = Physics.Raycast(transform.position, transform.forward, out interactableHit, interactionDistance);
        if (interactableCastResult == true)
        {
            Interactable interactable = interactableHit.collider.GetComponent<Interactable>();
            if (interactable == null)
            {
                interactableCastResult = false;
                interactIndicator.GetComponent<RotateAndBob>().ResetBobTime(); // this is a hack at best
                interactIndicator.SetActive(false);
            }
            else
            {
                interactIndicator.transform.position = interactable.transform.position + Vector3.up * interactable.IndicatorHeight;
                interactIndicator.SetActive(true);
            }
        }
        else
        {
            interactIndicator.GetComponent<RotateAndBob>().ResetBobTime();
            interactIndicator.SetActive(false);
        }


        if (isGrounded && Input.GetButtonDown("Jump"))
        {

            if (interactableCastResult == true)
            {
                Interactable interactable = interactableHit.collider.GetComponent<Interactable>();
                SetFocus(interactable);
                interactIndicator.GetComponent<RotateAndBob>().ResetBobTime();
                interactIndicator.SetActive(false);
            }
            else
            {
                // jump
                float jumpSpeed = jumpHeight * 2f * gravityUp;
                if (mudTime > 0) jumpSpeed /= 5.0f;

                verticalSpeed = Mathf.Sqrt(jumpSpeed);
            }
        }

        if (verticalSpeed > 0)
        {
            verticalSpeed -= gravityUp * Time.deltaTime;
        }
        else
        {
            verticalSpeed -= gravityDown * Time.deltaTime;
        }

        // ceiling check
        RaycastHit ceilingHit;
        bool ceilingCheckResult = Physics.SphereCast(transform.position + Vector3.up * (characterController.height / 2.0f - characterController.skinWidth - characterController.radius),
                                                    characterController.radius,
                                                    Vector3.up,
                                                    out ceilingHit,
                                                    groundDistance,
                                                    groundMask);

        characterController.Move(Vector3.up * verticalSpeed * Time.deltaTime);

        if (ceilingCheckResult && verticalSpeed > 0)
        {
            verticalSpeed = Mathf.Min(0.0f, verticalSpeed);
        }

        //animations
        //animator.SetBool("isRunning", direction.magnitude >= 0.1f);
        //animator.SetBool("isGrounded", isGrounded);
        //animator.SetBool("isFalling", verticalSpeed < 0.2f);

        if (mudTime > 0)
        {
            mudTime -= Time.deltaTime;
        }
    }


    void SetFocus(Interactable newFocus)
    {
        //animator.SetBool("isRunning", false);
        //animator.SetBool("isFlying", false);
        interactableFocus = newFocus;

        var target = newFocus.GetComponentInParent<ThirdPersonCamera.Targetable>();
        UIActions.EventEnterTextboxCamera?.Invoke(target);
        DialogueUI.EventShowDialogue.Invoke(interactableFocus.InteractionDialogue);
    }

    void RemoveFocus()
    {
        canMove = true;
        interactableFocus = null;
        UIActions.EventExitTextboxCamera?.Invoke(true);
    }

    public static Vector2 GetGameCameraMousePosition()
    {
        // transform screen coords into 320x240 coords
        Vector3 mousePos = Input.mousePosition;
        Vector2 screen = new Vector2(Screen.width, Screen.height);
        float aspectRatio = screen.x / screen.y;
        if (aspectRatio > 4.0f / 3.0f) // wider resolution
        {
            float ratio = screen.y / 240.0f;
            float edge = 160.0f * ratio;
            float offset = screen.x / 2.0f - edge;
            mousePos.x = Math.Clamp(mousePos.x, screen.x * 0.5f - edge, screen.x * 0.5f + edge) - offset;

            mousePos.x /= ratio;
            mousePos.y /= ratio;
        }
        else // taller resolution (why???)
        {
            float ratio = screen.x / 320.0f;
            float edge = 120.0f * ratio;
            float offset = screen.y / 2.0f - edge;

            mousePos.y = Math.Clamp(mousePos.y, screen.y * 0.5f - edge, screen.y * 0.5f + edge) - offset;

            mousePos.x /= ratio;
            mousePos.y /= ratio;
        }

        return new Vector2(mousePos.x, mousePos.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == collectibleLayer)
        {
            canMove = false;
            Collectible collectible = other.gameObject.GetComponent<Collectible>();
            AddDecoration(collectible.decorationInfo);
            DialogueUI.EventShowDialogue.Invoke(collectible.collectionDialog);

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == mudLayer)
        {
            mudTime = 0.3f;
        }
    }

    public void AddDecoration(DecorationInfo info)
    {
        availableDecorations.Add(info);
        decorationProjectiles.Add(Resources.Load(info.projectileResource) as GameObject);
    }

    public void OnFellInLake()
    {
        characterController.enabled = false; // need to disable CharacterController before teleporting, because Unity is dumb
        transform.position = new(-10, 1, 16);
        characterController.enabled = true;

        verticalSpeed = 0;
    }
    public void shootSnow()
    {
        Vector2 mousePos = GetGameCameraMousePosition();

        Vector3 aim = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.0f));
        Vector3 mouseDirection = aim - camera.transform.position;

        GameObject snowball = Instantiate(decorationProjectiles[activeDecorationIndex], bucketTransform.position, Quaternion.identity);
        snowball.transform.LookAt(aim);
        Rigidbody b = snowball.GetComponent<Rigidbody>();
        b.AddForce(mouseDirection.normalized * 500f);
    }
}