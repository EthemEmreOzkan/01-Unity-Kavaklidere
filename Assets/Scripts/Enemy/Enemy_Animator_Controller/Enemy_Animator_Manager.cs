using UnityEngine;

public class Enemy_Animator_Manager : MonoBehaviour
{
    [Header("Animator References --------------------------------------------------------------")]
    [Space]
    [SerializeField] private Animator Enemy_Animator;

    //*-----------------------------------------------------------------------------------------//
    #region Public Methods ----------------------------------------------------------------------

    //TODO Animator Controller Güncellenmesi

    public void SetBool(string paramName, bool state)
        => Enemy_Animator.SetBool(Animator.StringToHash(paramName), state);

    #endregion
    //*-----------------------------------------------------------------------------------------//
}
