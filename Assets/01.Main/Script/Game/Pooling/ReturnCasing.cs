using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCasing : MonoBehaviour
{
    public bool m_ifFirstEnable = true;

    #region Unity Methods
    private void OnEnable()
    {
        if (!m_ifFirstEnable)
        {
            Invoke("ReturnToPool", 1f);
        }

        m_ifFirstEnable = false;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void ReturnToPool()
    {
        gameObject.transform.SetParent(ObjPool.Instance.gameObject.transform);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        ObjPool.Instance.m_casingPool.Set(this);
        gameObject.SetActive(false);
    }
    #endregion
}
