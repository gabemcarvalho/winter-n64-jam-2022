using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public Transform cam;
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

    private bool canMove;

    [SerializeField] private float interactionDistance = 0.5f;
    private Interactable interactableFocus;

    private CharacterController characterController;
    private Animator animator;

    private float verticalSpeed = 0f;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private Vector3 slopeVelocity = Vector3.zero;
    private Vector3 hitNormal = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        DialogueUI.EventResumePlayerControl += RemoveFocus;
        canMove = true;
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
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (!canMove)
        {
            return;
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


        // ground check
        RaycastHit groundHit;
        bool groundCheckResult = Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out groundHit, characterController.radius + characterController.skinWidth + groundDistance, groundMask);

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
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
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
                interactIndicator.GetComponent<IndicatorMotion>().ResetBobTime(); // this is a hack at best
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
            interactIndicator.GetComponent<IndicatorMotion>().ResetBobTime();
            interactIndicator.SetActive(false);
        }


        if (isGrounded && Input.GetButtonDown("Jump"))
        {

            if (interactableCastResult == true)
            {
                Interactable interactable = interactableHit.collider.GetComponent<Interactable>();
                SetFocus(interactable);
                interactIndicator.GetComponent<IndicatorMotion>().ResetBobTime();
                interactIndicator.SetActive(false);
            }
            else
            {
                // jump
                verticalSpeed = Mathf.Sqrt(jumpHeight * 2f * gravityUp);
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

        characterController.Move(Vector3.up * verticalSpeed * Time.deltaTime);

        //animations
        //animator.SetBool("isRunning", direction.magnitude >= 0.1f);
        //animator.SetBool("isGrounded", isGrounded);
        //animator.SetBool("isFalling", verticalSpeed < 0.2f);
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
        interactableFocus = null;
        UIActions.EventExitTextboxCamera?.Invoke(true);
    }


}