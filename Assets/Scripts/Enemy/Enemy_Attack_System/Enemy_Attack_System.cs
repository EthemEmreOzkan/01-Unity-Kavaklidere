using UnityEngine;

public class Enemy_Attack_System : MonoBehaviour
{
    //TODO 3 ayrı attack tipine göre güncelleme yap
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Attack Settings -----------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Attack_Cooldown = 1.5f;
    [SerializeField] private int Attack_Damage = 10;
    [Space]
    [Header("References ----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Distance_Calculator Enemy_Distance_Calculator;
    [SerializeField] private Enemy_Animator_Manager Enemy_Animator_Manager;
    [Space]
    [Header("Debug ---------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Logs = true;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Private Variables ------------------------------------------------------------------

    private float last_Attack_Time = -Mathf.Infinity;
    private bool is_Attacking = false;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle -------------------------------------------------------------------
    void Awake()
    {
        last_Attack_Time = Time.time;    
    }

    void Update()
    {
        Handle_Attack();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Attack Logic ----------------------------------------------------------------------

    private void Handle_Attack()
    {
        if (Enemy_Distance_Calculator == null || !Enemy_Distance_Calculator.Has_Valid_Target) return;

        // Hedef saldırı menzilinde mi?
        if (Enemy_Distance_Calculator.Is_Target_In_Attack_Range)
        {
            // Cooldown kontrolü
            if (Time.time >= last_Attack_Time + Attack_Cooldown)
            {
                Start_Attack();
            }
        }
        else
        {
            if (is_Attacking)
                Stop_Attack();
        }
    }

    private void Start_Attack()
    {
        last_Attack_Time = Time.time;
        is_Attacking = true;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} saldırıya geçti!");

        // TODO Animator tetikle
        Enemy_Animator_Manager.SetTrigger("Attack");
    }

    private void Stop_Attack()
    {
        is_Attacking = false;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} saldırıyı durdurdu!");
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods --------------------------------------------------------------------
   
    public void Apply_Damage()
    {
        // TODO Player Health kurulacak
        Debug.LogWarning("Enemy Attack");
    }

    public void Set_Attack_Damage(int new_Damage)
        => Attack_Damage = Mathf.Max(0, new_Damage);

    public void Set_Attack_Cooldown(float new_Cooldown)
        => Attack_Cooldown = Mathf.Max(0f, new_Cooldown);

    public void Force_Attack()
    {
        if (Enemy_Distance_Calculator != null && Enemy_Distance_Calculator.Has_Valid_Target)
            Start_Attack();
    }

    public bool Is_Attacking => is_Attacking;

    #endregion

    //*-----------------------------------------------------------------------------------------//
}
