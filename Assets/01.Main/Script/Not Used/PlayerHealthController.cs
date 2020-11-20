using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    #region Field
    //References
    public Text m_HpText;
    //Player Info
    float m_hp;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_hp = 100f;
        m_HpText.text = m_hp.ToString();
    }

    void Update()
    {

    }
    #endregion

    #region Public Methods
    public void Damaged(float dmg)
    {
        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
            Debug.Log("You Died!");
        }
        else
        {
            m_hp = m_hp - dmg;
        }

        m_HpText.text = m_hp.ToString();
    }
    #endregion
}
