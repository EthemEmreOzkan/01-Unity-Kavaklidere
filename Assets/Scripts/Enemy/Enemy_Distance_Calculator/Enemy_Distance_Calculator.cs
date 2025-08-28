using UnityEngine;

public class Enemy_Distance_Calculator : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Distance Settings --------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Detection_Range = 10f;
    [SerializeField] private float Stop_Distance = 1.5f;
    [SerializeField] private float Attack_Range = 2f;
    [SerializeField] private Transform Player_Transform;
    [Space]
    [Header("Advanced Settings --------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Auto_Find_Player = true;
    [Space]
    [Header("Debug ---------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = true;
    [SerializeField] private bool Show_Distance_Info = false;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Private Variables ------------------------------------------------------------------

    private float current_Distance_To_Target;
    private Vector2 direction_To_Target;
    private Vector2 position_2D, target_Position_2D;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Properties ------------------------------------------------------------------

    public float Current_Distance => current_Distance_To_Target;
    public Vector2 Direction_To_Target => direction_To_Target;
    public Transform Current_Target => Player_Transform;
    public float Detection_Range_Value => Detection_Range;
    public float Stop_Distance_Value => Stop_Distance;
    public float Attack_Range_Value => Attack_Range;

    // Distance Check Methods
    public bool Is_Target_In_Detection_Range => current_Distance_To_Target <= Detection_Range;
    public bool Is_Target_In_Stop_Range => current_Distance_To_Target <= Stop_Distance;
    public bool Is_Target_In_Attack_Range => current_Distance_To_Target <= Attack_Range;
    public bool Is_Target_Beyond_Stop_Range => current_Distance_To_Target > Stop_Distance;
    public bool Has_Valid_Target => Player_Transform != null;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle -------------------------------------------------------------------

    void Awake()
    {
        Initialize_Target();
    }

    void Update()
    {
        Update_Distance_Calculations();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Initialization --------------------------------------------------------------------

    private void Initialize_Target()
    {
        if (Auto_Find_Player && Player_Transform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                Player_Transform = player.transform;
        }
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Distance Calculations -------------------------------------------------------------

    private void Update_Distance_Calculations()
    {
        if (Player_Transform == null)
        {
            current_Distance_To_Target = float.MaxValue;
            direction_To_Target = Vector2.zero;
            return;
        }

        Calculate_Positions();
        Calculate_Distance();
        Calculate_Direction();
    }

    private void Calculate_Positions()
    {

        position_2D = transform.position;
        target_Position_2D = Player_Transform.position;
    }

    private void Calculate_Distance()
    {
        current_Distance_To_Target = Vector2.Distance(position_2D, target_Position_2D);
    }

    private void Calculate_Direction()
    {
        direction_To_Target = (target_Position_2D - position_2D).normalized;
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods ---------------------------------------------------------------------

    // Target Management
    public void Set_Primary_Target(Transform new_Target)
    {
        Player_Transform = new_Target;
        Update_Distance_Calculations();
    }

    // Range Settings
    public void Set_Detection_Range(float new_Range)
        => Detection_Range = Mathf.Max(0f, new_Range);

    public void Set_Stop_Distance(float new_Distance)
        => Stop_Distance = Mathf.Max(0f, new_Distance);

    public void Set_Attack_Range(float new_Range)
        => Attack_Range = Mathf.Max(0f, new_Range);

    // Distance Queries with Custom Values
    public bool Is_Target_Within_Range(float custom_Range)
        => current_Distance_To_Target <= custom_Range;

    public bool Is_Target_Beyond_Range(float custom_Range)
        => current_Distance_To_Target > custom_Range;

    // TODO Object Pooling Support
    public void Reset_Calculator()
    {
        current_Distance_To_Target = float.MaxValue;
        direction_To_Target = Vector2.zero;
        Initialize_Target();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Debug Methods ---------------------------------------------------------------------

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos) return;

        Vector3 pos = transform.position;

        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, Detection_Range);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, Attack_Range);

        // Stop distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, Stop_Distance);

        if (Application.isPlaying && Player_Transform != null)
        {
            // Direction line
            Color line_Color = Is_Target_In_Detection_Range ? Color.red : 
                             Is_Target_In_Attack_Range ? Color.magenta : Color.white;
            Gizmos.color = line_Color;
            Gizmos.DrawLine(pos, Player_Transform.position);
        }

        // Distance info display
        if (Show_Distance_Info && Application.isPlaying && Player_Transform != null)
        {
            UnityEditor.Handles.Label(pos + Vector3.up * 2f, 
                $"Distance: {current_Distance_To_Target:F2}\n" +
                $"In Detection: {Is_Target_In_Detection_Range}\n" +
                $"In Attack: {Is_Target_In_Attack_Range}");
        }
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//
}