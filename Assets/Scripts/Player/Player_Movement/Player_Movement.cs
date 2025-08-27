using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    // TODO yorum satırlarını ayarla ve kodları daha düzenli hale getir
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Hareket Ayarlari ----------------------------------------------------------------")]
    [SerializeField] private float Move_Speed = 5f;
    [SerializeField] private float Acceleration_Rate = 10f;
    [SerializeField] private float Deceleration_Rate = 15f;

    [Header("Debug ---------------------------------------------------------------------------")]
    [SerializeField] private bool Show_Debug_Gizmos = false;
    [Header("Animations----------------------------------------------------------------------")]
    [SerializeField] private Player_Animator_Manager Player_Animator_Manager;

    private Rigidbody2D rb;
    private Vector2 movement_Input, current_Velocity, target_Velocity;
    private InputAction move_Action;

    public Vector2 Current_Velocity => current_Velocity;
    public bool Is_Moving => current_Velocity.magnitude > 0.1f;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle ---------------------------------------------------------------------

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        move_Action = GetComponent<PlayerInput>().actions["Move"];
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        target_Velocity = movement_Input * Move_Speed;
        float rate = movement_Input.magnitude > 0.1f ? Acceleration_Rate : Deceleration_Rate;
        current_Velocity = Vector2.Lerp(current_Velocity, target_Velocity, rate * Time.fixedDeltaTime);
        rb.linearVelocity = current_Velocity;

        // Karakter yönünü çevirme
        if (current_Velocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(current_Velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // TODO Animator çağrısı detaylıca bak
        Player_Animator_Manager.SetBool("Is_Walking", Is_Moving);
    }

    void OnEnable()
    {
        move_Action.performed += On_Move_Performed;
        move_Action.canceled += On_Move_Canceled;
        move_Action.Enable();
    }

    void OnDisable()
    {
        move_Action.performed -= On_Move_Performed;
        move_Action.canceled -= On_Move_Canceled;
        move_Action.Disable();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Input Handling ----------------------------------------------------------------------    

    private void On_Move_Performed(InputAction.CallbackContext context)
        => movement_Input = context.ReadValue<Vector2>().normalized;

    private void On_Move_Canceled(InputAction.CallbackContext context)
        => movement_Input = Vector2.zero;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods ----------------------------------------------------------------------

    public void Set_Movement_Speed(float new_Speed)
        => Move_Speed = Mathf.Max(0f, new_Speed);

    public void Stop_Movement()
    {
        current_Velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Debug -------------------------------------------------------------------------------

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos || !Application.isPlaying) return;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, current_Velocity.normalized * 2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, movement_Input * 1.5f);
    }

    #endregion
    
    //*-----------------------------------------------------------------------------------------//
}
