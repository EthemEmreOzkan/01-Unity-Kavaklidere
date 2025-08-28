using UnityEngine;

public class Enemy_Animations_Event_System : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ----------------------------------------------------------------------

    [Header("Script References -----------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Attack_System Enemy_Attack_System;

    #endregion

    //*-----------------------------------------------------------------------------------------//

    #region  Event Functions --------------------------------------------------------------------

    //! Apply Damage Animation cagrısı
    private void Player_Take_Damage_Event()
    {
        Enemy_Attack_System.Apply_Damage();
    }

    #endregion

}