using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return3DAudioSource : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    #region Unity Methods
    void Start()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void ReturnToPool()
    {
        ObjPool.Instance.m_audioPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion

    #region Public Methods
    public void ReturnInvoke(float time)
    {
        Invoke("ReturnToPool", time);
    }
    #endregion
}
