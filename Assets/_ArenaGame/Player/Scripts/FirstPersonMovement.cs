using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Debug")]
    public bool useDebugUI;
    [SerializeField] CanvasGroup _debugUI;
    [SerializeField] TMP_Text _velocityTMP;

    [Space(5f)]
    [Header("Mouse Look")]
    [SerializeField][Range(100, 400)] private float _mouseSensX;
    [SerializeField][Range(100, 400)] private float _mouseSensY;
    private float _xRotation;
    private float _yRotation;
    private float _mouseX;
    private float _mouseY;
    private bool _isCursorLocked = false;

    [Space(5f)]
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;
    private float _horizontalInput;
    private float _verticalInput;

    [Space(5f)]
    [SerializeField] Animator anim;

    [SerializeField] private float _playerAcceleration = 2f;
    [SerializeField] private float _playerMaxSpeed = 15f;
    [SerializeField] private float _airVelocityMulti = .4f;
    [SerializeField] float _standHeight = 1.8f;
    //[SerializeField] float _DuckHeight = .75f;
    //[SerializeField] float _sprintSpeedMultiplyer = 1.5f;
    //[SerializeField] float _walkSpeedMultiplyer = 1f;
    [SerializeField] private float _gravityForce = 9.81f * 1.2f;
    [SerializeField] private float _groundDrag = 5f;

    private bool _isGrounded;

    private float _height;
    private float _velocity;
    private Camera _camera;
    private Rigidbody _rb;
    private Vector3 _moveDirection;

    private Vector3 _fallVelocity;
    private LayerMask _groundLayer;

    public CursorLockMode CursorLock
    {
        set
        {
            Cursor.lockState = value;
            _isCursorLocked = value == CursorLockMode.Locked;
            Cursor.visible = !_isCursorLocked;
        }
    }

    virtual protected void Awake()
    {
        if (useDebugUI) _debugUI.alpha = 1;
        else _debugUI.alpha = 0;
        CursorLock = CursorLockMode.Locked;
        _groundLayer = LayerMask.GetMask("Ground", "Static");
        _camera = Camera.main;
    }

    virtual protected void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    virtual protected void Update()
    {
        Debug.Log($"IsGrounded: {_isGrounded}");
        InputHolder();
        MouseLook();
        GroundCheck();
        Move();
        Gravity();
    }

    void MouseLook()
    {
        if (!_isCursorLocked) return;

        _mouseX = Input.GetAxis("Mouse X") * _mouseSensX * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * _mouseSensY * Time.deltaTime;

        _yRotation += _mouseX;
        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
    }

    void Move()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _moveDirection = transform.forward * _verticalInput + transform.right * _horizontalInput;

        if(_isGrounded) _rb.AddForce(_moveDirection.normalized * _playerAcceleration * 10f, ForceMode.Force);
        else _rb.AddForce(_moveDirection.normalized * _playerAcceleration * 10f * _airVelocityMulti, ForceMode.Force);

        if (_isGrounded) _rb.linearDamping = _groundDrag;
        else _rb.linearDamping = 0;

        //Speed Control
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (flatVel.magnitude > _playerMaxSpeed)
        {
            Vector3 limitVel = flatVel.normalized * _playerMaxSpeed;
            _rb.linearVelocity = new Vector3(limitVel.x, _rb.linearVelocity.y, limitVel.z);
        }
        if (useDebugUI) _velocityTMP.text = $"Velocity: {_rb.linearVelocity.magnitude.ToString("F2")}";
    }

    void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    void Gravity()
    {
        if (!_isGrounded) _fallVelocity += Vector3.down * _gravityForce * Time.deltaTime;
        else _fallVelocity = Vector3.zero;

        _rb.AddForce(_fallVelocity, ForceMode.Force);
    }

    void GroundCheck()
    {
        Vector3 checkPosition = transform.position + Vector3.down * ((_standHeight / 2) + .25f);
        _isGrounded = Physics.CheckSphere(checkPosition, 0.15f, _groundLayer);

        // Desenha a esfera de checagem
        DebugExtension.DrawSphere(checkPosition, 0.15f, _isGrounded ? Color.green : Color.red, .1f);
    }

    void InputHolder()
    {
        // Duck
        if (Input.GetKeyDown(KeyCode.LeftControl)) { ToggleDuck(); }

        // Walk and Sprint
        /*if (Input.GetKeyDown(KeyCode.LeftShift) && !walk && Input.GetKey(KeyCode.W)) { sprint = true; }
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !sprint) { walk = true; }
        
        if (Input.GetKeyUp(KeyCode.LeftShift)) { sprint = false; }
        if (Input.GetKeyUp(KeyCode.LeftAlt)) { walk = false; }*/

        // Jump
        if (Input.GetButtonDown("Jump")) { if(_isGrounded) Jump(); }

        //Unlock Cursor
        if (Input.GetKeyDown(KeyCode.Escape)) { CursorLock = CursorLockMode.None; }
        if (_isCursorLocked && Input.GetKeyDown(KeyCode.Mouse0)) { CursorLock = CursorLockMode.Locked; }
    }

    void ToggleDuck(int mode = -1)
    {
        /*StopCoroutine(ToggleDuckRoutine());
        switch (mode)
        {
            case 0:
                duck = false;
                break;
            case 1:
                duck = true;
                break;
            default:
                duck = !duck;
                break;
        }

        StartCoroutine(ToggleDuckRoutine());*/
    }

    /*IEnumerator ToggleDuckRoutine()
    {
        float lerpTime = .08f;
        float enumFPS =  EnumFPS(60);
        

        while (characterController.height > DuckHeight && duck)
        {
            characterController.height = Mathf.Lerp(characterController.height, DuckHeight, lerpTime);
            yield return new WaitForSeconds(enumFPS);
        }

        while (characterController.height < StandHeight && !duck)
        {
            characterController.height = Mathf.Lerp(characterController.height, StandHeight, lerpTime);
            yield return new WaitForSeconds(enumFPS);
        }

        yield return null;
    }*/

    /// <summary>
    /// TBD.
    /// </summary>
    /// <param name="fps">TBD.</param>
    /// <returns>TBD.</returns>
    float EnumFPS(int fps) // Returns Expected framerate converted to float
    {
        return (1 / fps);

    }
}

public static class DebugExtension
{
    public static void DrawSphere(Vector3 position, float radius, Color color, float duration)
    {
        int segments = 20;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2;

            // Círculo no plano XY (vista de cima)
            Vector3 point1XY = new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius + position;
            Vector3 point2XY = new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius + position;
            Debug.DrawLine(point1XY, point2XY, color, duration);

            // Círculo no plano XZ (vista lateral)
            Vector3 point1XZ = new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius + position;
            Vector3 point2XZ = new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius + position;
            Debug.DrawLine(point1XZ, point2XZ, color, duration);

            // Círculo no plano YZ (vista de frente)
            Vector3 point1YZ = new Vector3(0, Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius + position;
            Vector3 point2YZ = new Vector3(0, Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius + position;
            Debug.DrawLine(point1YZ, point2YZ, color, duration);
        }
    }
}

