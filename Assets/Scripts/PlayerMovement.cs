using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  [Header("Health Settings")]
  public float health = 100f;
  public float maxHealth = 100f;
  public bool canRegenerateHealth = true;
  public float healthRegenMaxTotalAmount = 40f;
  private float totalHealthRegenerated = 0f;
  private float healthRegenTimer = 0f;
  public float healthRegenRate = 1f;
  public float healthRegenAmount = 1f;
  public float healthRegenStartDelay = 5f;
  public RectTransform healthBar;
  public RectTransform healthBarBackground;
  public TextMeshProUGUI healthText;
  [Header("Movement Settings")]
  public float speed = 5f;
  public float runMultiplier = 1.5f;
  public float jumpForce = 5f;
  public float groundCheckDistance = 0.15f;
  public LayerMask groundLayers = ~0;
  Rigidbody playerRigidbody;
  Vector2 moveInput;
  bool jumpQueued;
  bool isGrounded;
  public float mouseSensitivity = 2f;
  public bool lockCursor = true;
  public InputAction moveAction;
  public InputAction sprintAction;
  public InputAction jumpAction;
  public InputAction reloadAction;
  public InputAction crouchAction;
  public InputAction flashlightAction;
  [Header("Item Settings")]
  public bool flashlightEnabled = true;
  public GameObject flashlight;
  public Transform itemTransform;
  CapsuleCollider playerCollider;
  public GameObject[] items;
  public int currentItemIndex = 0;
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

      // if it has a object called "flashlight", set the flashlight reference to it
      GameObject flashlightComponent = currentItemInstance.transform.Find("flashlight")?.gameObject;
      if (flashlightComponent != null)
      {
        flashlight = flashlightComponent.gameObject;
        flashlight.SetActive(flashlightEnabled);
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

    if (flashlightAction != null)
    {
      flashlightAction.Enable();
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
    // Update health bar
    if (healthBar != null && healthBarBackground != null)
    {
      float healthPercentage = Mathf.Clamp01(health / maxHealth);
      healthBar.localScale = new Vector3(healthPercentage, 1f, 1f);
      healthBarBackground.localScale = new Vector3(1f, 1f, 1f);
      if (healthText != null)
      {
        healthText.text = $"{health}";
      }
    }

    // Handle Renegeneration
    if (canRegenerateHealth && health < maxHealth && totalHealthRegenerated < healthRegenMaxTotalAmount)
    {
      healthRegenTimer += Time.deltaTime;
      if (healthRegenTimer >= healthRegenStartDelay)
      {
        health += Mathf.RoundToInt(healthRegenAmount);
        totalHealthRegenerated += healthRegenAmount;
        healthRegenTimer = 0f;
      }
    }

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

    if (flashlightAction != null && flashlightAction.WasPressedThisFrame())
    {
      flashlightEnabled = !flashlightEnabled;
      flashlight.SetActive(flashlightEnabled);
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
      if (currentItemInstance != null)
      {
        WeaponScript weaponScript = currentItemInstance.GetComponentInChildren<WeaponScript>();
        if (weaponScript != null)
        {
          if (weaponScript.Shoot())
          {
            animator.SetTrigger("shoot");
          }
        }
      }
    }

    
  }

  public void HolsterItem() {
    Animator animator = GetComponent<Animator>();
    if (animator)
    {
      animator.SetTrigger("holster_enter");
    }
  }

  public void UnholsterItem() {
    Animator animator = GetComponent<Animator>();
    if (animator)
    {
      animator.SetTrigger("holster_exit");
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

    // Only sprint when moving forward and sprint button is pressed
    float moveSpeedMultiplier = sprintAction != null && moveInput.y > 0 && sprintAction.IsPressed() ? runMultiplier : 1f;
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

  public void TakeDamage(float damage)
  {
    health -= damage;
    if (health <= 0)
    {
      // !!!! TEMP !!!!
      Destroy(gameObject);
    }
  }
}
