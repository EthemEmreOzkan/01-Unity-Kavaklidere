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
    [Header("AI Ayarlari ---------------------------------------------------------------------")]
    [Space]
    // TODO Object Pool eklendiği zaman için Detetion_Range güncelle
    [SerializeField] private float Detection_Range = 10f;
    [SerializeField] private float Stop_Distance = 1.5f;
    [SerializeField] private Transform Player_Target;
    [Space]
    [Header("Animations----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Animator_Manager Enemy_Animator_Manager;
    [Space]
    [Header("Debug ---------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = true;
    [Space]
    
    private Rigidbody2D rb;
    private Vector2 movement_Direction, current_Velocity, target_Velocity;
    private float distance_To_Player;

    public Vector2 Current_Velocity => current_Velocity;
    public bool Is_Moving => current_Velocity.magnitude > 0.1f;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle ---------------------------------------------------------------------

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (Player_Target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                Player_Target = player.transform;
        }
    }

    void FixedUpdate()
    {
        Update_AI_Logic();
        Calculate_Movement();
        Apply_Movement();
        Handle_Rotation();

        // TODO Animator çağrısı eklenecek
        Enemy_Animator_Manager.SetBool("Is_Walking", Is_Moving);
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region AI Logic ---------------------------------------------------------------------------

    private void Update_AI_Logic()
    {
        if (Player_Target == null) return;

        distance_To_Player = Vector2.Distance(transform.position, Player_Target.position);

        if (distance_To_Player <= Detection_Range && distance_To_Player > Stop_Distance)
        {
            movement_Direction = ((Vector2)Player_Target.position - (Vector2)transform.position).normalized;
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

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods ----------------------------------------------------------------------

    // TODO Gerekli yerlerde çağırılacak

    public void Set_Movement_Speed(float new_Speed)
        => Move_Speed = Mathf.Max(0f, new_Speed);

    public void Set_Player_Target(Transform new_Target)
        => Player_Target = new_Target;

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
        if (!Show_Debug_Gizmos) return;

        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Detection_Range);

        // Stop distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Stop_Distance);

        if (Application.isPlaying)
        {
            // Mevcut hız vektörü
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, current_Velocity.normalized * 2f);

            // Oyuncuya olan çizgi
            if (Player_Target != null)
            {
                Gizmos.color = distance_To_Player <= Detection_Range ? Color.red : Color.white;
                Gizmos.DrawLine(transform.position, Player_Target.position);
            }
        }
    }

    #endregion
    
    //*-----------------------------------------------------------------------------------------//
}