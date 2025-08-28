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

    //! Melee saldırısında hasar uygulama
    private void Player_Take_Damage_Event()
    {
        Enemy_Attack_System.Apply_Damage_Melee();
    }

    //! Dash başlangıcında çağrılır
    private void Enemy_Dash_Starter()
    {
        Enemy_Attack_System.Execute_Dash_Attack();
    }

    //! Ranged atış tetikleme (yoğunlaşma animasyonu sonunda)
    private void Enemy_Ranged_Shot()
    {
        Enemy_Attack_System.Fire_Ranged_Ray();
    }

    #endregion

    //*-----------------------------------------------------------------------------------------//
}
