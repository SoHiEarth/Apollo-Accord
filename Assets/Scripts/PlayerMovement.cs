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
    Transform cameraTarget;
    Transform viewTransform;
    float pitch;
    // Animated a thumping red vignette when hp is low
    public GameObject thumpVolume;
    Animator thumpAnimator;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        if (Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
            viewTransform = Camera.main.transform;
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
                if (cameraTarget == null && Camera.main != null)
                {
                    cameraTarget = Camera.main.transform;
                }

                faceObject.target = cameraTarget;
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

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            jumpQueued = true;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;
        transform.Rotate(0f, mouseDelta.x, 0f, Space.Self);

        if (viewTransform != null)
        {
            pitch = Mathf.Clamp(pitch - mouseDelta.y, -85f, 85f);
            viewTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
        if (crouchAction != null && crouchAction.WasPressedThisFrame())
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("isCrouching", !animator.GetBool("isCrouching"));
        }
        if (thumpAnimator != null)
        {
            thumpAnimator.SetFloat("PlayerHealth", health);
        }
        if (reloadAction != null && reloadAction.WasPressedThisFrame())
        {
            if (currentItemInstance != null)
            {
                WeaponScript weaponScript = currentItemInstance.GetComponentInChildren<WeaponScript>();
                if (weaponScript != null)
                {
                    weaponScript.Reload();
                }
                // play reload animation
                Animator animator = GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("reload");
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
                        Animator animator = GetComponent<Animator>();
                        if (animator != null) {
                            animator.SetTrigger("shoot");
                        }
                    }
                }
            }
        }
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
