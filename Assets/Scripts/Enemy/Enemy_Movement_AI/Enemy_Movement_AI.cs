using UnityEngine;

public class Enemy_Movement_AI : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Hareket Ayarlari ----------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Move_Speed = 3f;
    [SerializeField] private float Acceleration_Rate = 8f;
    [SerializeField] private float Deceleration_Rate = 12f;
    [Space]
    [Header("Component References ------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Distance_Calculator Enemy_Distance_Calculator;
    [SerializeField] private Enemy_Animator_Manager Enemy_Animator_Manager;
    [Space]
    [Header("Debug ---------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = true;
    [SerializeField] private bool Show_Velocity_Info = false;
    [Space]
    
    private Rigidbody2D rb;
    private Vector2 movement_Direction, current_Velocity, target_Velocity;

    public Vector2 Current_Velocity => current_Velocity;
    public bool Is_Moving => current_Velocity.magnitude > 0.1f;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle -------------------------------------------------------------------

    void Awake()
    {
        Initialize_Components();
    }

    void FixedUpdate()
    {
        Update_AI_Logic();
        Calculate_Movement();
        Apply_Movement();
        Handle_Rotation();
        Update_Animations();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Initialization --------------------------------------------------------------------

    private void Initialize_Components()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Distance Calculator'ı otomatik bul
        if (Enemy_Distance_Calculator == null)
            Enemy_Distance_Calculator = GetComponent<Enemy_Distance_Calculator>();

        if (Enemy_Distance_Calculator == null)
        {
            Debug.LogError($"Enemy_Distance_Calculator component bulunamadı! {gameObject.name}");
        }
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region AI Logic ---------------------------------------------------------------------------

    private void Update_AI_Logic()
    {
        if (Enemy_Distance_Calculator == null || !Enemy_Distance_Calculator.Has_Valid_Target)
        {
            movement_Direction = Vector2.zero;
            return;
        }

        // Distance Calculator'dan bilgileri al
        if (Enemy_Distance_Calculator.Is_Target_In_Detection_Range && 
            Enemy_Distance_Calculator.Is_Target_Beyond_Stop_Range)
        {
            movement_Direction = Enemy_Distance_Calculator.Direction_To_Target;
        }
        else
        {
            movement_Direction = Vector2.zero;
        }
    }

    private void Calculate_Movement()
    {
        target_Velocity = movement_Direction * Move_Speed;
        float rate = movement_Direction.magnitude > 0.1f ? Acceleration_Rate : Deceleration_Rate;
        current_Velocity = Vector2.Lerp(current_Velocity, target_Velocity, rate * Time.fixedDeltaTime);
    }

    private void Apply_Movement()
    {
        rb.linearVelocity = current_Velocity;
    }

    private void Handle_Rotation()
    {
        if (current_Velocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(current_Velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void Update_Animations()
    {
        if (Enemy_Animator_Manager != null)
            Enemy_Animator_Manager.SetBool("Is_Walking", Is_Moving);
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods ---------------------------------------------------------------------

    public void Set_Movement_Speed(float new_Speed)
        => Move_Speed = Mathf.Max(0f, new_Speed);

    public void Set_Distance_Calculator(Enemy_Distance_Calculator new_Calculator)
        => Enemy_Distance_Calculator = new_Calculator;

    public void Stop_Movement()
    {
        current_Velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    // Distance Calculator'dan bilgi almak için proxy metodlar
    public float Get_Current_Distance()
        => Enemy_Distance_Calculator != null ? Enemy_Distance_Calculator.Current_Distance : float.MaxValue;

    public bool Is_Target_In_Range(float range)
        => Enemy_Distance_Calculator != null ? Enemy_Distance_Calculator.Is_Target_Within_Range(range) : false;

    public Vector2 Get_Direction_To_Target()
        => Enemy_Distance_Calculator != null ? Enemy_Distance_Calculator.Direction_To_Target : Vector2.zero;

    // Object Pooling support
    public void Reset_Movement()
    {
        Stop_Movement();
        movement_Direction = Vector2.zero;
        target_Velocity = Vector2.zero;
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Debug -------------------------------------------------------------------------------

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos) return;

        if (Application.isPlaying)
        {
            // Mevcut hız vektörü
            if (current_Velocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, current_Velocity.normalized * 2f);
            }
        }

        // Velocity info display
        if (Show_Velocity_Info && Application.isPlaying)
        {
            Vector3 label_Pos = transform.position + Vector3.up * 3f;
            UnityEditor.Handles.Label(label_Pos, 
                $"Speed: {current_Velocity.magnitude:F2}\n" +
                $"Is Moving: {Is_Moving}\n" +
                $"Direction: {movement_Direction}");
        }
    }

    #endregion
    
    //*-----------------------------------------------------------------------------------------//
}