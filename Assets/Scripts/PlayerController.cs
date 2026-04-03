using InfimaGames.LowPolyShooterPack;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MovementSpeed = 2.0f;
    public float SprintSpeed = 4.0f;
    public float JumpForce = 5.0f;
    public float DistanceToGround = 0.76f;

    [Header("Mouse & Camera")]
    public bool LockCursor = true;
    public float MouseSensitivity = 2.0f;
    [Range(0f, 0.1f)]
    [Tooltip("Плавность камеры")]
    public float RotationSmoothTime = 0.03f;

    [Header("References")]
    public GameObject HandMeshes;
    public GameObject[] WeaponInventory;
    public GameObject[] WeaponMeshes;

    private int SelectedWeaponId = 0;
    private Weapon _Weapon;

    private float pitch, yaw;
    private float pitchVelocity, yawVelocity;
    private float currentPitch, currentYaw;

    private bool IsGrounded;
    private bool IsSprinting = false;
    private Rigidbody _Rigidbody;
    private GameManager _GameManager;
    private AnimationManager _AnimationManager;

    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _GameManager = FindAnyObjectByType<GameManager>();

        yaw = transform.eulerAngles.y;
        currentYaw = yaw;
        if (HandMeshes != null)
        {
            pitch = HandMeshes.transform.localEulerAngles.x;
            currentPitch = pitch;
        }

        if (WeaponInventory != null && WeaponInventory.Length > 0)
        {
            SelectWeapon(0);
        }

        UpdateCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor = !LockCursor;
            UpdateCursorState();
        }

        if (LockCursor)
        {
            HandleRotation();

            if (_Weapon != null)
            {
                if (Input.GetKey(KeyCode.Mouse0)) _Weapon.Fire();
                if (Input.GetKey(KeyCode.R)) _Weapon.Reload();
            }

            if (WeaponInventory != null && WeaponInventory.Length > 0)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll < 0) SelectNextWeapon();
                else if (scroll > 0) SelectPrevWeapon();
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();

        if (Input.GetKey(KeyCode.Space) && IsGrounded) Jump();

        bool canSprint = IsGrounded && IsMoving();
        if (_GameManager != null) canSprint = canSprint && !_GameManager.IsStaminaRestoring;

        if (Input.GetKey(KeyCode.LeftShift) && canSprint)
        {
            if (_GameManager != null) _GameManager.SpendStamina();
            _Rigidbody.MovePosition(CalculateSprint());
        }
        else
        {
            _Rigidbody.MovePosition(CalculateMovement());
        }

        SetAnimation();
    }

    private void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * MouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        currentYaw = Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, RotationSmoothTime);
        currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVelocity, RotationSmoothTime);

        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        if (HandMeshes != null) HandMeshes.transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    private void UpdateCursorState()
    {
        Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !LockCursor;
    }

    private Vector3 CalculateMovement()
    {
        IsSprinting = false;
        return GetMoveVector(MovementSpeed);
    }

    private Vector3 CalculateSprint()
    {
        IsSprinting = true;
        return GetMoveVector(SprintSpeed);
    }

    private Vector3 GetMoveVector(float speed)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        return _Rigidbody.position + move.normalized * speed * Time.fixedDeltaTime;
    }

    private void Jump() => _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

    private void GroundCheck() => IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToGround);

    private void SelectWeapon(int id)
    {
        foreach (var mesh in WeaponMeshes) mesh.SetActive(false);

        SelectedWeaponId = id;
        WeaponMeshes[SelectedWeaponId].SetActive(true);
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

        if (_Weapon != null)
        {
            _Weapon.SetAnimationManager(_AnimationManager);
            _Weapon.UpdateUI();
        }
    }

    private void SelectPrevWeapon() { if (SelectedWeaponId > 0) SelectWeapon(SelectedWeaponId - 1); }
    private void SelectNextWeapon() { if (SelectedWeaponId < WeaponInventory.Length - 1) SelectWeapon(SelectedWeaponId + 1); }

    private bool IsMoving() => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

    private void SetAnimation()
    {
        if (_AnimationManager == null) return;

        if (IsMoving())
        {
            if (IsSprinting) _AnimationManager.SetAnimationRun();
            else _AnimationManager.SetAnimationWalk();
        }
        else _AnimationManager.SetAnimationIdle();
    }
}