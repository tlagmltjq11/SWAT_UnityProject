using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATW : MonoBehaviour
{
    #region Field
    public Rigidbody m_rigid;
    public GameObject m_effectObj;
    public GameObject m_meshObj;
    public int m_remainNum;
    public float m_timeToOper;
    public string m_name;
    public float m_power;
    public float m_explosionRadius;
    #endregion

    #region Abstract Methods
    public abstract void Operation(); //동작
    #endregion
}
