using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHitHole : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    #region Unity Methods
    void Start()
    {
        gameObject.SetActive(false);  
    }

    private void OnEnable()
    {
        if (!m_ifFirstEnable)
        {
            Invoke("ReturnToPool", 5f);
        }

        m_ifFirstEnable = false;
    }
    #endregion

    #region Public Methods
    //HitHole은 HealthController에서 사용하므로 Public으로 선언해줘야 함.
    public void ReturnToPool()
    {
        gameObject.transform.SetParent(GameObject.Find("ObjPoolManager").transform);
        ObjPool.Instance.m_hitHoleObjPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion
}
