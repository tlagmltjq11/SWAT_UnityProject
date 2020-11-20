using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    float m_hp;

    #region Unity Methods
    void Start()
    {
        m_hp = 100f;
    }
    #endregion

    #region Public Methods
    public void Damaged(float dmg)
    {
        if(m_hp - dmg <= 0f)
        {
            /*
            ReturnHitHole[] hole = gameObject.GetComponentsInChildren<ReturnHitHole>();

            foreach(ReturnHitHole h in hole)
            {
                h.ReturnToPool();
            }
            */

            m_hp = 0f;
            Destroy(gameObject);
        }
        else
        {
            m_hp = m_hp - dmg;
        }
    }
    #endregion

}
