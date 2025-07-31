using UnityEngine;
using piqey.Utilities.Editor;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    public bool isDashing = false;
    private bool canDash = true;

    // Camera
    public Transform cameraPivot;
    [Range(0.01f, 5f)]
    public float cameraSensitivity = 2f;
    [Range(0f, 6f)]
    public float cameraDistance = 3f;
    public float minY = -40f;
    public float maxY = 80f;
    private float yaw = 0f;
    private float pitch = 0f;

    // Stamina
    [Header("Stamina")]
    [Range(0f, 100f)]
    public float maxStamina = 100f;
    [Range(0f, 100f)]
    public float currentStamina;
    [Range(0f, 60f)]
    public float dashStaminaCost = 30f;
    [Range(0f, 50f)]
    public float staminaRegenRate = 20f;
    [Range(0f, 10f)]
    public float lowStaminaRegenRate = 5f;
    public float lowStaminaThreshold = 0f;
    private bool outOfStamina = false;

    //Health
    [Header("Health")]
    [Range(0f, 100f)]
    public float maxPlayerHealth = 100f;
    [Range(0f, 100f)]
    public float currentPlayerHealth;
    private bool isAlive = true;
    public GameObject deathUI;

    [Header("UI")]
    public RectTransform healthBar;
    public RectTransform staminaBar; // assign in inspector

    [Range(0.01f, 16f)]
    public float healthBarLerpSpeed = 6f;
    [Range(0.01f, 16f)]
    public float staminaBarLerpSpeed = 6f;

    [Header("Debug Only")]

    [SerializeField, ReadOnly] private float healthBarInitialX;
    [SerializeField, ReadOnly] private float staminaBarInitialX;

    [SerializeField, ReadOnly] private float healthBarLerpX;
    [SerializeField, ReadOnly] private float staminaBarLerpX;

    private void Awake()
    {
        isAlive = true;
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => TryJump();
        controls.Player.Dash.performed += ctx => TryDash();

        currentStamina = maxStamina;
        currentPlayerHealth = maxPlayerHealth;

        healthBarInitialX = healthBar.anchoredPosition.x;
        staminaBarInitialX = staminaBar.anchoredPosition.x;

        healthBarLerpX = healthBarInitialX;
        staminaBarLerpX = staminaBarInitialX;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        if (isAlive)
        {
            UpdateCamera();
            RegenerateStamina();

            UpdateUI();

            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            Vector3 forward = cameraPivot.forward;
            Vector3 right = cameraPivot.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * moveInput.y + right * moveInput.x;

            if (move != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            if (!isDashing)
                controller.Move(moveSpeed * Time.deltaTime * move);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void TryJump()
    {
        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void TryDash()
    {
        if (!canDash || currentStamina < dashStaminaCost) return;
        StartCoroutine(DashRoutine());
    }

    System.Collections.IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        currentStamina -= dashStaminaCost;
        if (currentStamina <= lowStaminaThreshold)
        {
            outOfStamina = true;
        }

        Vector3 dashDirection = transform.forward;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            controller.Move(dashSpeed * Time.deltaTime * dashDirection);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void RegenerateStamina()
    {
        float regen = outOfStamina ? lowStaminaRegenRate : staminaRegenRate;
        if (currentStamina < maxStamina)
        {
            currentStamina += regen * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        if (currentStamina >= 20f)
        {
            outOfStamina = false;
        }
    }

    void UpdateCamera()
    {
        yaw += lookInput.x * cameraSensitivity;
        pitch -= lookInput.y * cameraSensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    public void PlayerTakeDamage(float amount)
    {
        if (!isDashing)
        {
            currentPlayerHealth -= amount;
            Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentPlayerHealth}");
        }

        if (currentPlayerHealth <= 0)
        {
            GameOver();
            Debug.Log("GameOver");
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
            UpdateUIBar(healthBar, currentPlayerHealth / maxPlayerHealth, healthBarInitialX, ref healthBarLerpX, healthBarLerpSpeed);

        if (staminaBar != null)
            UpdateUIBar(staminaBar, currentStamina / maxStamina, staminaBarInitialX, ref staminaBarLerpX, staminaBarLerpSpeed);
    }

    void UpdateUIBar(RectTransform barTransform, float delta, float initialX, ref float lerpX, float lerpSpeed)
    {
        lerpX = Mathf.Lerp(lerpX, initialX * delta, Time.deltaTime * lerpSpeed);
        barTransform.anchoredPosition = new Vector2(lerpX, barTransform.anchoredPosition.y);
    }

    void GameOver()
    {
        currentPlayerHealth = 0;
        isAlive = false;
        deathUI.SetActive(true);
    }
}
