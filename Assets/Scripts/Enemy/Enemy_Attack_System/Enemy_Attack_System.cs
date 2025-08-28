using UnityEngine;

public class Enemy_Attack_System : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Enum Definitions ------------------------------------------------------------------

    public enum Attack_Type
    {
        Melee,
        Ranged,
        Dasher
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Attack Settings -----------------------------------------------------------------")]
    [Space]
    [SerializeField] private Attack_Type Current_Attack_Type = Attack_Type.Melee;
    [SerializeField] private float Attack_Cooldown = 1.5f;
    //[SerializeField] private int Melee_Damage = 10;
    //[SerializeField] private int Ranged_Damage = 15;
    //[SerializeField] private int Dasher_Damage = 20;
    [Space]
    [Header("Ranged Settings -----------------------------------------------------------------")]
    [SerializeField] private float Ranged_Charge_Duration = 1f;
    [SerializeField] private float Ranged_Max_Distance = 15f;
    [SerializeField] private float Ranged_Ray_Width = 0.1f;  // genişlik (çap), yarıçap=width*0.5
    [SerializeField] private Transform Ranged_Fire_Point;     // opsiyonel: namlu/çıkarma noktası
    [SerializeField] private LayerMask Ranged_Hit_Layers;     // ray’in vuracağı layerlar (Player'ı ekle)
    [Space]
    [Header("Dash Settings -------------------------------------------------------------------")]
    [SerializeField] private float Dash_Charge_Duration = 1f;
    [SerializeField] private float Dash_Speed = 15f;
    [SerializeField] private float Dash_Distance = 5f;
    [SerializeField] private float Dash_Duration = 1f;
    [Space]
    [Header("References ----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Distance_Calculator Enemy_Distance_Calculator;
    [SerializeField] private Enemy_Animator_Manager Enemy_Animator_Manager;
    [SerializeField] private Collider2D Enemy_Collider;
    [SerializeField] private Enemy_Movement_AI Enemy_Movement_AI; // hareket kontrolü için
    [Space]
    [Header("Debug ---------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Logs = true;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Private Variables ------------------------------------------------------------------

    private float last_Attack_Time = -Mathf.Infinity;
    private bool is_Attacking = false;

    private bool is_Charging = false;
    private Vector2 cached_Player_Position_For_Ranged;
    private Vector2 cached_Dash_Target;
    private Vector2 dash_Start_Position;

    private Rigidbody2D rb; // <-- eklendi

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Unity Lifecycle -------------------------------------------------------------------

    void Awake()
    {
        last_Attack_Time = Time.time;

        if (Enemy_Collider == null)
            Enemy_Collider = GetComponent<Collider2D>();

        if (Enemy_Movement_AI == null)
            Enemy_Movement_AI = GetComponent<Enemy_Movement_AI>();

        rb = GetComponent<Rigidbody2D>(); // <-- eklendi
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

        if (Enemy_Distance_Calculator.Is_Target_In_Attack_Range)
        {
            if (Time.time >= last_Attack_Time + Attack_Cooldown)
            {
                switch (Current_Attack_Type)
                {
                    case Attack_Type.Melee:
                        Start_Melee_Attack();
                        break;

                    case Attack_Type.Ranged:
                        Start_Ranged_Attack();
                        break;

                    case Attack_Type.Dasher:
                        Start_Dash_Attack();
                        break;
                }
            }
        }
        else
        {
            if (is_Attacking)
                Stop_Attack();
        }
    }

    //!-----------------------------------------------------------------------------------------//

    //! Melee
    private void Start_Melee_Attack()
    {
        last_Attack_Time = Time.time;
        is_Attacking = true;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} Melee Attack başladı!");

        Enemy_Animator_Manager.SetTrigger("Attack_Melee");
        // Damage animasyon eventinden çağrılmalı
    }

    //!-----------------------------------------------------------------------------------------//

    //! Ranged
    private void Start_Ranged_Attack()
    {
        last_Attack_Time = Time.time;
        is_Attacking = true;
        is_Charging = true;

        // Yoğunlaşma sırasında movement kapat
        if (Enemy_Movement_AI != null)
        {
            Enemy_Movement_AI.Stop_Movement();
            Enemy_Movement_AI.enabled = false;
        }

        // Oyuncunun şu anki konumunu kaydet
        cached_Player_Position_For_Ranged = Enemy_Distance_Calculator.Current_Target.position;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} Ranged Attack için yoğunlaşıyor!");

        Enemy_Animator_Manager.SetTrigger("Attack_Ranged");

        // Animasyon event'i ile Fire_Ranged_Ray() çağrılacak
        // Invoke(nameof(Fire_Ranged_Ray), Ranged_Charge_Duration);
    }

    // Animation Event'ten çağrılmalı
    public void Fire_Ranged_Ray()
    {
        if (!is_Charging) return;
        is_Charging = false;

        if (Enemy_Distance_Calculator.Has_Valid_Target)
        {
            // origin: varsa Fire_Point, yoksa Rigidbody2D, o da yoksa transform
            Vector2 origin =
                Ranged_Fire_Point != null ? (Vector2)Ranged_Fire_Point.position :
                rb != null ? rb.position :
                (Vector2)transform.position;

            Vector2 ray_Direction = (cached_Player_Position_For_Ranged - origin).normalized;

            // LayerMask boşsa tüm katmanlarda tara
            int mask = (Ranged_Hit_Layers.value == 0) ? Physics2D.DefaultRaycastLayers : Ranged_Hit_Layers.value;

            // Geniş ray: CircleCast (yarıçap = width * 0.5f)
            float radius = Mathf.Max(0f, Ranged_Ray_Width * 0.5f);
            RaycastHit2D hit = Physics2D.CircleCast(origin, radius, ray_Direction, Ranged_Max_Distance, mask);

            if (hit.collider != null)
            {
                if (Show_Debug_Logs)
                    Debug.LogWarning($"{gameObject.name} Ranged ray {hit.collider.name} objesine çarptı!");

                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log($"{gameObject.name} Ranged hasar verdi!");
                    // TODO: Player health sistemine bağla
                }
            }
            else if (Show_Debug_Logs)
            {
                Debug.Log($"{gameObject.name} Ranged ray hiçbir şeye çarpmadı.");
            }
        }

        is_Attacking = false;

        // Ranged atış bitti, movement tekrar açılsın
        if (Enemy_Movement_AI != null)
            Enemy_Movement_AI.enabled = true;
    }

    //!-----------------------------------------------------------------------------------------//

    //! Dasher
    private void Start_Dash_Attack()
    {
        last_Attack_Time = Time.time;
        is_Attacking = true;
        is_Charging = true;

        dash_Start_Position = transform.position;
        cached_Dash_Target = Enemy_Distance_Calculator.Current_Target.position;

        // Yoğunlaşma sırasında movement kapat
        if (Enemy_Movement_AI != null)
        {
            Enemy_Movement_AI.Stop_Movement();
            Enemy_Movement_AI.enabled = false;
        }

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} Dash için yoğunlaşıyor!");

        Enemy_Animator_Manager.SetTrigger("Attack_Dasher");
    }

    // Animation event'inden çağrılacak
    public void Execute_Dash_Attack()
    {
        if (!is_Charging) return;
        is_Charging = false;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} Dash atılıyor!");

        Perform_Dash();
    }

    private void Perform_Dash()
    {
        if (Enemy_Distance_Calculator.Has_Valid_Target)
        {
            if (Enemy_Collider != null)
                Enemy_Collider.isTrigger = true;

            Vector2 dash_Direction = (cached_Dash_Target - dash_Start_Position).normalized;
            Vector2 dash_Target_Position = dash_Start_Position + (dash_Direction * Dash_Distance);

            if (rb != null)
                rb.linearVelocity = dash_Direction * Dash_Speed; // linearVelocity -> velocity

            Invoke(nameof(End_Dash), Dash_Duration);
        }
    }

    private void End_Dash()
    {
        if (Enemy_Collider != null)
            Enemy_Collider.isTrigger = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero; // linearVelocity -> velocity

        is_Attacking = false;

        // Dash bitti → movement aç
        if (Enemy_Movement_AI != null)
            Enemy_Movement_AI.enabled = true;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} Dash tamamlandı!");
    }

    //!-----------------------------------------------------------------------------------------//
    private void Stop_Attack()
    {
        is_Attacking = false;
        is_Charging = false;

        CancelInvoke();

        if (Enemy_Collider != null)
            Enemy_Collider.isTrigger = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Attack iptal edilirse movement tekrar açılsın
        if (Enemy_Movement_AI != null)
            Enemy_Movement_AI.enabled = true;

        if (Show_Debug_Logs)
            Debug.Log($"{gameObject.name} saldırıyı durdurdu!");
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Collision Detection ---------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (is_Attacking && Current_Attack_Type == Attack_Type.Dasher && !is_Charging)
        {
            if (other.CompareTag("Player"))
                Debug.LogWarning($"{gameObject.name} Dash ile oyuncuya çarptı!");
        }
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods --------------------------------------------------------------------

    public void Apply_Damage_Melee()
        => Debug.LogWarning("Melee Damage Verildi!");

    public void Apply_Damage_Ranged()
        => Debug.LogWarning("Ranged Damage Verildi!");

    public void Apply_Damage_Dash()
        => Debug.LogWarning("Dash Damage Verildi!");

    public void Set_Attack_Type(Attack_Type newType)
        => Current_Attack_Type = newType;

    public bool Is_Attacking => is_Attacking;
    public bool Is_Charging => is_Charging;

    public void Set_Ranged_Charge_Duration(float duration) => Ranged_Charge_Duration = duration;
    public void Set_Ranged_Max_Distance(float distance) => Ranged_Max_Distance = distance;
    public void Set_Ranged_Ray_Width(float width) => Ranged_Ray_Width = width;

    public void Set_Dash_Charge_Duration(float duration) => Dash_Charge_Duration = duration;
    public void Set_Dash_Speed(float speed) => Dash_Speed = speed;
    public void Set_Dash_Distance(float distance) => Dash_Distance = distance;
    public void Set_Dash_Duration(float duration) => Dash_Duration = duration;

    #endregion

    //*-----------------------------------------------------------------------------------------//
}
