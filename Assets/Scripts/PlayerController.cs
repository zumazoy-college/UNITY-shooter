using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool LockCursor = true;
    public float MovementSpeed = 2.0f;
    public float SprintSpeed = 4.0f;
    public float JumpForce = 5.0f;
    public float DistanceToGround = 0.76f;
    public float MouseSensitivity = 5.0f;
    public float RotationSmoothing = 20f;
    public GameObject HandMeshes;
    public GameObject[] WeaponInventory;
    public GameObject[] WeaponMeshes;
    private int SelectedWeaponId = 0;
    private Weapon _Weapon;
    private float pitch, yaw;
    private bool IsGrounded;
    private bool IsSprinting = false; //булевая бежим мы сейчас или нет
    private Rigidbody _Rigidbody;
    private GameManager _GameManager;
    private AnimationManager _AnimationManager; //приватное поле которое будет хранить в себе анимайшен менеджер

    void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _GameManager = FindAnyObjectByType<GameManager>();
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
        WeaponMeshes[SelectedWeaponId].SetActive(true);

        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

        LockCursor = true;
        UpdateCursorState();
    }

    void FixedUpdate()
    {
        GroundCheck();
        
        if (Input.GetKey(KeyCode.Space) && IsGrounded) Jump();

        if (Input.GetKey(KeyCode.LeftShift) && IsGrounded && !_GameManager.IsStaminaRestoring)
        {
            _GameManager.SpendStamina();
            _Rigidbody.MovePosition(CalculateSprint());
        }
        else _Rigidbody.MovePosition(CalculateMovement());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor = !LockCursor;
            UpdateCursorState();
        }

        if (Input.GetKey(KeyCode.Mouse0)) _Weapon.Fire(_AnimationManager);
        if (Input.GetKey(KeyCode.R)) _Weapon.Reload(_AnimationManager);
        if (Input.GetAxis("Mouse ScrollWheel") < 0) SelectNextWeapon();
        else if (Input.GetAxis("Mouse ScrollWheel") > 0) SelectPrevWeapon();

        SetAnimation();
        SetRotation();
    }

    private void UpdateCursorState()
    {
        if (LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private Vector3 CalculateMovement()
    {
        IsSprinting = false;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * MovementSpeed;
    }

    private Vector3 CalculateSprint()
    {
        IsSprinting = true;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * SprintSpeed;
    }

    private void Jump()
    {
        _Rigidbody.AddForce(Vector3.up *  JumpForce, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistanceToGround));
    }

    public void SetRotation()
    {
        yaw += Input.GetAxis("Mouse X") * MouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);

        HandMeshes.transform.rotation = Quaternion.Slerp(HandMeshes.transform.rotation, SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);

        SmoothRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);
    }

    private void SelectPrevWeapon()
    {
        if (SelectedWeaponId != 0)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId -= 1;

            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
            WeaponMeshes[SelectedWeaponId].SetActive(true);

            _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

            Debug.Log("Weapon: " + _Weapon.WeaponType);
        }
    }

    private void SelectNextWeapon()
    {
        if (WeaponInventory.Length > SelectedWeaponId + 1)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId += 1;

            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
            WeaponMeshes[SelectedWeaponId].SetActive(true);

            _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

            Debug.Log("Weapon: " + _Weapon.WeaponType);
        }
    }

    //метод который считывает двигаемся ли в данный момент
    private bool IsMoving()
    {
        //если перемещается по горизонтали или по вертикали
        //если по какой либо из осей перемещается то возвращает тру
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
    //метод переключения анимации
    private void SetAnimation()
    {
        if (_AnimationManager == null)
        {
            Debug.LogError("AnimationManager is null! SelectedWeaponId: " + SelectedWeaponId);
            return;
        }

        if (IsMoving())
        {
            if (IsSprinting) _AnimationManager.SetAnimationRun();
            else _AnimationManager.SetAnimationWalk();
        }
        else _AnimationManager.SetAnimationIdle();
    }
}
