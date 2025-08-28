using UnityEngine;

public class Enemy_Animator_Manager : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Animator References --------------------------------------------------------------")]
    [Space]
    [SerializeField] private Animator Enemy_Animator;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region Public Methods ---------------------------------------------------------------------

    // Bool parametre kontrolü
    public void SetBool(string param_Name, bool state)
        => Enemy_Animator.SetBool(Animator.StringToHash(param_Name), state);

    // Trigger parametre kontrolü
    public void SetTrigger(string param_Name)
        => Enemy_Animator.SetTrigger(Animator.StringToHash(param_Name));

    // Float parametre kontrolü
    public void SetFloat(string param_Name, float value)
        => Enemy_Animator.SetFloat(Animator.StringToHash(param_Name), value);

    // Integer parametre kontrolü
    public void SetInteger(string param_Name, int value)
        => Enemy_Animator.SetInteger(Animator.StringToHash(param_Name), value);

    #endregion

    //*-----------------------------------------------------------------------------------------//
}
