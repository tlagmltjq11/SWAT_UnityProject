using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    #region Field
    #region References
    [SerializeField]
    GameObject m_player;
    [SerializeField]
    GameObject m_verticalCamRotate;
    #endregion
    #region Mouse Info
    public float mouseSensitivity = 400f;
    #endregion
    #region About Mouse Rotate
    Vector2 m_mouseDir = Vector2.zero;
    public float xRotation = 0f;
    #endregion
    #endregion

    #region Unity Methods
    private void Awake()
    {
        //시작할때 커서 안보이게 설정.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        MouseLook();
    }
    #endregion

    #region Private Methods
    void MouseLook()
    {
        m_mouseDir.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        m_mouseDir.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= m_mouseDir.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        m_verticalCamRotate.transform.localRotation = Quaternion.Slerp(m_verticalCamRotate.transform.localRotation, Quaternion.Euler(xRotation, 0, 0), Time.deltaTime * 35f);
        m_player.transform.Rotate(Vector3.up * m_mouseDir.x);
    }
    #endregion

    #region Public Methods
    public void VerticalCamRotate(float recoilRotX)
    {
        xRotation -= recoilRotX;
    }
    #endregion
}
