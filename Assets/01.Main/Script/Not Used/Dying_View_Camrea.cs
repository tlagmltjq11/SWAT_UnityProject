using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dying_View_Camrea : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_camera;
    Vector3 goalPosition;
    #endregion

    #region Unity Methods

    void Start()
    {
        goalPosition = new Vector3(m_camera.transform.localPosition.x, m_camera.transform.localPosition.y + 5f, m_camera.transform.localPosition.z + 1f);
    }

    void Update()
    {
        m_camera.transform.localRotation = Quaternion.Slerp(m_camera.transform.localRotation, Quaternion.Euler(90f, 0f, 0f), Time.deltaTime * 3f);
        m_camera.transform.localPosition = Vector3.Lerp(m_camera.transform.localPosition, goalPosition, Time.deltaTime * 2f);
    }
    #endregion
}
