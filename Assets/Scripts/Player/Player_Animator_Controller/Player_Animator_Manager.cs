using UnityEngine;

public class Player_Animator_Manager : MonoBehaviour
{
    [SerializeField] private Animator Player_Animator;

    //*-----------------------------------------------------------------------------------------//
    #region Public Methods ----------------------------------------------------------------------

    //TODO Animator Controller Güncellenmesi

    public void SetBool(string paramName, bool state)
        => Player_Animator.SetBool(Animator.StringToHash(paramName), state);

    #endregion
    //*-----------------------------------------------------------------------------------------//
}
