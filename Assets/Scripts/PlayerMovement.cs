using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float health = 100f;
    public float speed = 5f;
    public float runMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    public bool lockCursor = true;
    public InputAction moveAction;
    public InputAction sprintAction;
    public InputAction jumpAction;
    public InputAction reloadAction;
    public InputAction crouchAction;
    public InputAction leftLeanAction;
    public InputAction rightLeanAction;
    public Transform itemTransform;
    CapsuleCollider playerCollider;
    public GameObject[] items;
    public int currentItemIndex = 0;
    public float groundCheckDistance = 0.15f;
    public LayerMask groundLayers = ~0;
    Rigidbody playerRigidbody;
    Vector2 moveInput;
    bool jumpQueued;
    bool isGrounded;
    GameObject currentItemInstance;
    public GameObject playerCamera;
    float pitch;
    // Animated a thumping red vignette when hp is low
    public GameObject thumpVolume;
    Animator thumpAnimator;
    float playerColliderDefaultHeight;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider != null)
        {
            playerColliderDefaultHeight = playerCollider.height;
        }
    }

    void SetItem(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            currentItemIndex = index;
            if (currentItemInstance != null)
            {
                Destroy(currentItemInstance);
            }

            currentItemInstance = Instantiate(items[currentItemIndex], itemTransform);
            currentItemInstance.transform.localPosition = Vector3.zero;
            currentItemInstance.transform.localRotation = Quaternion.identity;

            FaceObject faceObject = currentItemInstance.GetComponentInChildren<FaceObject>(true);
            if (faceObject != null)
            {
                faceObject.target = playerCamera.transform;
            }
        }
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (moveAction != null)
        {
            moveAction.Enable();
        }

        if (sprintAction != null)
        {
            sprintAction.Enable();
        }

        if (jumpAction != null)
        {
            jumpAction.Enable();
        }

        if (reloadAction != null)
        {
            reloadAction.Enable();
        }
        
        if (crouchAction != null)
        {
            crouchAction.Enable();
        }

        if (leftLeanAction != null)
        {
            leftLeanAction.Enable();
        }

        if (rightLeanAction != null)
        {
            rightLeanAction.Enable();
        }

        if (items.Length > 0)
        {
            SetItem(currentItemIndex);
        }
        if (thumpVolume != null)
        {
            thumpAnimator = thumpVolume.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        Animator animator = GetComponent<Animator>();
        if (animator == null)
        {
            return;
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            jumpQueued = true;
        }

        if (crouchAction != null)
        {
            animator.SetBool("isCrouching", crouchAction.IsPressed());
            if (crouchAction.IsPressed())
            {
                playerCollider.height = playerColliderDefaultHeight / 2f;
            }
            else
            {
                playerCollider.height = playerColliderDefaultHeight;
            }
        }

        if (leftLeanAction != null)
        {
            animator.SetBool("isLeaningLeft", leftLeanAction.IsPressed());
        }

        if (rightLeanAction != null)
        {
            animator.SetBool("isLeaningRight", rightLeanAction.IsPressed());
        }

        if (thumpAnimator != null)
        {
            thumpAnimator.SetFloat("PlayerHealth", health);
        }

        if (reloadAction != null && reloadAction.WasPressedThisFrame())
        {
            if (currentItemInstance != null)
            {
                // play reload animation
                animator.SetTrigger("reload");
                WeaponScript weaponScript = currentItemInstance.GetComponentInChildren<WeaponScript>();
                if (weaponScript != null)
                {
                    weaponScript.Reload();
                }
            }
        }

        // if mouse button down, set animator's "isShooting" parameter to true, otherwise set it to false
        if (Mouse.current.leftButton.isPressed)
        {
            
            // get the weapon script from the current item instance and call its Shoot() method
            if (currentItemInstance != null) {
                WeaponScript weaponScript = currentItemInstance.GetComponentInChildren<WeaponScript>();
                if (weaponScript != null) {
                    if (weaponScript.Shoot())
                    {
                        animator.SetTrigger("shoot");
                    }
                }
            }
        }
    }

// Store the current X rotation of the player for use in mouse look
// this prevents the animator from override the player's yaw rotation
    float rotationX;
    void LateUpdate()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;
        rotationX += mouseDelta.x;
        transform.rotation = Quaternion.Euler(0f, rotationX, 0f);


        // rotate the camera up and down based on mouse Y movement, clamping to prevent flipping
        pitch = Mathf.Clamp(pitch - mouseDelta.y, -85f, 85f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void FixedUpdate()
    {
        if (playerRigidbody == null)
        {
            return;
        }

        UpdateGroundedState();

        Vector3 forward = Vector3.ProjectOnPlane(playerRigidbody.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(playerRigidbody.transform.right, Vector3.up).normalized;

        float moveSpeedMultiplier = sprintAction != null && sprintAction.IsPressed() ? runMultiplier : 1f;
        Vector3 desiredPlanarVelocity = (right * moveInput.x + forward * moveInput.y) * speed * moveSpeedMultiplier;

        Vector3 currentVelocity = playerRigidbody.linearVelocity;
        currentVelocity.x = desiredPlanarVelocity.x;
        currentVelocity.z = desiredPlanarVelocity.z;

        Animator animator = GetComponent<Animator>();

        if (jumpQueued && isGrounded)
        {
            currentVelocity.y = jumpForce;
            jumpQueued = false;
            isGrounded = false;
            animator.SetTrigger("jump");
        }

        playerRigidbody.linearVelocity = currentVelocity;

        if (animator != null)
        {
            Vector3 actualHorizontalVelocity = playerRigidbody.linearVelocity;
            actualHorizontalVelocity.y = 0f;
            animator.SetFloat("player_speed", actualHorizontalVelocity.magnitude);
        }
    }

    void UpdateGroundedState()
    {
        Vector3 origin;
        float castDistance = groundCheckDistance;

        if (playerCollider != null)
        {
            Bounds bounds = playerCollider.bounds;
            origin = bounds.center;
            castDistance += bounds.extents.y;
        }
        else
        {
            origin = playerRigidbody.position;
        }

        isGrounded = Physics.Raycast(origin, Vector3.down, castDistance, groundLayers, QueryTriggerInteraction.Ignore);
    }

    void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }

        if (sprintAction != null)
        {
            sprintAction.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.Disable();
        }
    }
}
