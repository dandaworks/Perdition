using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerStateMachine : BaseStateMachine
{
    // Movement
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;

    private CharacterController controller;
    public CharacterController GetController() { return controller; }
    private PlayerControls controls;
    private Vector2 moveInput;
    void SetMoveInput(Vector2 a) { moveInput = a; }
    public Vector2 GetMoveInput() { return moveInput; }


    private Vector2 lookInput;
    void SetLookInput(Vector2 a) { lookInput = a; }

    // There's issues with Unity's new input system and mouse input deltas that I can't fully explain;
    // it's like they're not captured on the same ticks that Update() methods are called and Time.deltaTime
    // corelates with, so the net result is a measurement that can't be frame-compensated for controlling
    // the camera (I had this issue on the last project and in personal projects as well; I've yet to find
    // a solution other than using the old methods for mouse capture in the event mouse input is needed for
    // camera control)
    // - piqey
    // private Vector2 lookInput;

    private Vector3 velocity;
    public Vector3 GetVelocity() { return velocity; }
    public void SetVelocity(Vector3 a) { velocity = a; }

    public bool isDashing = false;
    private bool canDash = true;
    public bool GetCanDash() { return canDash; }
    public void SetCanDash(bool a) { canDash = a; }

    // Camera
    public Transform cameraPivot;
    public Vector2 cameraSensitivity = Vector2.one;
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
    [Range(0f, 60f)]
    public float lightAtkStaminaCost = 30f;
    [Range(0f, 60f)]
    public float hvyAtkStaminaCost = 30f;
    [Range(0f, 50f)]
    public float staminaRegenRate = 20f;
    [Range(0f, 10f)]
    public float lowStaminaRegenRate = 5f;
    public float lowStaminaThreshold = 0f;
    private bool outOfStamina = false;
    public bool IsOutOfStamina() { return outOfStamina; }
    public void SetIsOutOfStamina(bool a) { outOfStamina = a; }

    bool canUseStamina = true;
    public void SetCanUseStamina(bool a) { canUseStamina = a; }

    //Health
    [Header("Health")]
    [Range(0f, 100f)]
    public float maxPlayerHealth = 100f;
    [Range(0f, 100f)]
    public float currentPlayerHealth;
    private bool isAlive = true;
    public void SetIsAlive(bool a) { isAlive = a; }
    public GameObject deathUI;

    [Header("UI")]
    public RectTransform healthBar;
    public RectTransform staminaBar; // assign in inspector

    [Range(0.01f, 16f)]
    public float healthBarLerpSpeed = 6f;
    [Range(0.01f, 16f)]
    public float staminaBarLerpSpeed = 6f;

    [Header("Debug Only")]

    [SerializeField] private float healthBarInitialX;
    [SerializeField] private float staminaBarInitialX;

    [SerializeField] private float healthBarLerpX;
    [SerializeField] private float staminaBarLerpX;

    //

    public Animator animator;

    public PlayerStateIdle stateIdle;
    public override BaseState InitialState()
    {
        return stateIdle;
    }
    public PlayerStateJump stateJump;
    public PlayerStateWalk stateWalk;

    public PlayerStateMeleeLight stateMeleeLight;
    public PlayerStateMeleeStrong stateMeleeStrong;
    public PlayerStateDash stateDash;
    public PlayerStateDead stateDead;

    public GameObject propSword;
    public GameObject weaponSword;

    public GameObject propSpear;
    public GameObject weaponSpear;

    public AudioSource audioSource;

    public AudioClip jump;
    public AudioClip dash;
    public AudioClip lightAttack;
    public AudioClip heavyAttack;


    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        PlayerInput.OnMoveEvent += SetMoveInput;
        PlayerInput.OnAimEvent += SetLookInput;

        PlayerInput.OnJumpEvent += TryJump;
        PlayerInput.OnDashEvent += TryDash;

        PlayerInput.OnMeleeLightEvent += TryLightMelee;
        PlayerInput.OnMeleeHeavyEvent += TryHeavyMelee;

        PlayerInput.OnMenuEvent += ExitToMainMenu;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        PlayerInput.OnMoveEvent -= SetMoveInput;
        PlayerInput.OnAimEvent -= SetLookInput;

        PlayerInput.OnJumpEvent -= TryJump;
        PlayerInput.OnDashEvent -= TryDash;

        PlayerInput.OnMeleeLightEvent -= TryLightMelee;
        PlayerInput.OnMeleeHeavyEvent -= TryHeavyMelee;

        PlayerInput.OnMenuEvent -= ExitToMainMenu;
    }

    public override void StartFunctions()
    {
        base.StartFunctions();

        propSword.SetActive(true);
        weaponSword.SetActive(false);

        propSpear.SetActive(true);
        weaponSpear.SetActive(false);

        stateIdle = new PlayerStateIdle(this);
        stateJump = new PlayerStateJump(this);
        stateWalk = new PlayerStateWalk(this);
        stateMeleeLight = new PlayerStateMeleeLight(this);
        stateMeleeStrong = new PlayerStateMeleeStrong(this);
        stateDash = new PlayerStateDash(this);
        stateDead = new PlayerStateDead(this);

        isAlive = true;
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        currentStamina = maxStamina;
        currentPlayerHealth = maxPlayerHealth;

        healthBarInitialX = healthBar.anchoredPosition.x;
        staminaBarInitialX = staminaBar.anchoredPosition.x;

        healthBarLerpX = healthBarInitialX;
        staminaBarLerpX = staminaBarInitialX;
    }

    public override void UpdateFunctions()
    {
        base.UpdateFunctions();
        
        if (isAlive)
        {
            UpdateCamera();
        }
        
        RegenerateStamina();
        UpdateUI();

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward * moveInput.y + right * moveInput.x;
    }

    void UpdateCamera()
    {
        //Vector2 mouse = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 mouse = lookInput;
        mouse *= cameraSensitivity;

        yaw += mouse.x;
        pitch -= mouse.y;

        pitch = Mathf.Clamp(pitch, minY, maxY);
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0);
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

    void TryJump()
    {
        Ray ray = new Ray();

        //Physics.SphereCast(Physics.Ra, 0.4f, 0f, LayerMask.GetMask(""));

        if (controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, 2f, LayerMask.GetMask("Environment")))
            ChangeState(stateJump);
        else
        {
            Debug.Log("No GRound");
        }
    }

    void TryDash()
    {
        if (!canUseStamina || !canDash || currentStamina < dashStaminaCost) return;

        ModifyStamina(dashStaminaCost);

        canUseStamina = false;

        ChangeState(stateDash);
    }

    void TryLightMelee()
    {
        if (!canUseStamina || currentStamina < lightAtkStaminaCost) return;

        ModifyStamina(lightAtkStaminaCost);

        canUseStamina = false;

        ChangeState(stateMeleeLight);
    }

    void TryHeavyMelee()
    {
        if (!canUseStamina || currentStamina < hvyAtkStaminaCost) return; else { Debug.Log("no heavy attack");}

        ModifyStamina(hvyAtkStaminaCost);

        canUseStamina = false;

        ChangeState(stateMeleeStrong);
    }

    public void ModifyStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina <= lowStaminaThreshold)
        {
            SetIsOutOfStamina(true);
        }
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
    void GameOver()
    {
        currentPlayerHealth = 0;

        ChangeState(stateDead);

        SetCanChangeStates(false);
    }

    void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
