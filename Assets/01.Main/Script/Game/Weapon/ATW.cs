using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATW : MonoBehaviour //ahead thrown weapon
{
    #region Field
    public GameObject m_effectObj;
    public GameObject m_meshObj;
    public Rigidbody m_rigid;
    public int m_remainNum;
    public string m_name;
    public float m_timeToOper;
    protected float m_power;
    protected float m_explosionRadius;
    #endregion

    #region Public Methods
    public void Starter()
    {
        Invoke("Operation", m_timeToOper);
    }
    #endregion

    #region Abstract Methods
    public abstract void Operation(); //동작
    #endregion
}
