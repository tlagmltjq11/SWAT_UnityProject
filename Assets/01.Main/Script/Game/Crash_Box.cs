using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crash_Box : MonoBehaviour
{
    #region Fields
    public MeshRenderer m_wholeCrate;
    public BoxCollider m_boxCollider;
    public GameObject m_fracturedCrate;
    public AudioSource m_crashAudioClip;
    #endregion

    public void Crash()
    {
        m_wholeCrate.enabled = false;
        m_boxCollider.enabled = false;
        m_fracturedCrate.SetActive(true);
        m_crashAudioClip.Play();

        Invoke("DestroySelf", 5f);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
