using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Controller : MonoBehaviour
{
    #region Field
    [SerializeField]
    bool m_pushOpen;
    float m_angle;
    GameObject m_player;
    bool m_isClosed;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_isClosed = true;

        if(m_pushOpen)
        {
            m_angle = 120f;
        }
        else
        {
            m_angle = -120f;
        }
    }

    void Update()
    {
        if (m_player != null)
        {
            if (Input.GetKeyDown(KeyCode.F) && m_isClosed)
            {
                StartCoroutine("Open");
                SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.SUPPLYBOX_OPEN, transform.position, 10f, 1f);
            }
            else if(Input.GetKeyDown(KeyCode.F) && !m_isClosed)
            {
                StartCoroutine("Close");
                SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.SUPPLYBOX_CLOSE, transform.position, 10f, 1f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            m_player = other.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            m_player = null;
        }
    }

    IEnumerator Open()
    {
        while (true)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.Euler(0f, m_angle, 0f), Time.deltaTime * 2f);

            if (m_pushOpen)
            {
                if (gameObject.transform.localRotation.eulerAngles.y >= m_angle - 5f)
                {
                    m_isClosed = false;
                    yield break;
                }
            }
            else
            {
                if (gameObject.transform.localRotation.eulerAngles.y <= (m_angle * -2f) + 5f)
                {
                    m_isClosed = false;
                    yield break;
                }
            }

            yield return null;
        }
    }

    IEnumerator Close()
    {
        while (true)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 2f);

            if (m_pushOpen)
            {
                if (gameObject.transform.localRotation.eulerAngles.y <= 0.5f)
                {
                    m_isClosed = true;
                    yield break;
                }
            }
            else
            {
                if (gameObject.transform.localRotation.eulerAngles.y >= 359.5f)
                {
                    m_isClosed = true;
                    yield break;
                }
            }

            yield return null;
        }
    }
    #endregion
}
