using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] private float Move_Speed = 5f;
    [SerializeField] private float Acceleration_Rate = 10f;
    [SerializeField] private float Deceleration_Rate = 15f;
    
    [Header("Debug")]
    [SerializeField] private bool Show_Debug_Gizmos = false;
    
    // Private değişkenler
    private Rigidbody2D rb;
    private Vector2 movement_Input;
    private Vector2 current_Velocity;
    private Vector2 target_Velocity;
    
    // Input Actions
    private PlayerInput player_Input;
    private InputAction move_Action;
    
    // Properties
    public Vector2 Current_Velocity => current_Velocity;
    public bool Is_Moving => current_Velocity.magnitude > 0.1f;
    
    void Awake()
    {
        Initialize_Components();
        Setup_Input();
    }
    
    void Start()
    {
        Setup_Physics();
    }
    
    void FixedUpdate()
    {
        Handle_Movement();
    }
    
    void OnEnable()
    {
        Enable_Input();
    }
    
    void OnDisable()
    {
        Disable_Input();
    }
    
    private void Initialize_Components()
    {
        rb = GetComponent<Rigidbody2D>();
        player_Input = GetComponent<PlayerInput>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component gerekli!");
        }
        
        if (player_Input == null)
        {
            Debug.LogError("PlayerInput component gerekli!");
        }
    }
    
    private void Setup_Input()
    {
        if (player_Input != null)
        {
            move_Action = player_Input.actions["Move"];
        }
    }
    
    private void Setup_Physics()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }
    
    private void Enable_Input()
    {
        if (move_Action != null)
        {
            move_Action.performed += On_Move_Performed;
            move_Action.canceled += On_Move_Canceled;
            move_Action.Enable();
        }
    }
    
    private void Disable_Input()
    {
        if (move_Action != null)
        {
            move_Action.performed -= On_Move_Performed;
            move_Action.canceled -= On_Move_Canceled;
            move_Action.Disable();
        }
    }
    
    private void On_Move_Performed(InputAction.CallbackContext context)
    {
        movement_Input = context.ReadValue<Vector2>().normalized;
    }
    
    private void On_Move_Canceled(InputAction.CallbackContext context)
    {
        movement_Input = Vector2.zero;
    }
    
    private void Handle_Movement()
    {
        Calculate_Target_Velocity();
        Apply_Acceleration();
        Apply_Movement();
    }
    
    private void Calculate_Target_Velocity()
    {
        target_Velocity = movement_Input * Move_Speed;
    }
    
    private void Apply_Acceleration()
    {
        float acceleration_Rate = movement_Input.magnitude > 0.1f ? Acceleration_Rate : Deceleration_Rate;
        current_Velocity = Vector2.Lerp(current_Velocity, target_Velocity, acceleration_Rate * Time.fixedDeltaTime);
    }
    
    private void Apply_Movement()
    {
        rb.linearVelocity = current_Velocity;
    }
    
    // Public metodlar
    public void Set_Movement_Speed(float new_Speed)
    {
        Move_Speed = Mathf.Max(0f, new_Speed);
    }
    
    public void Stop_Movement()
    {
        current_Velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
    
    // Gizmo çizimi
    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos || !Application.isPlaying) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, current_Velocity.normalized * 2f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, movement_Input * 1.5f);
    }
}