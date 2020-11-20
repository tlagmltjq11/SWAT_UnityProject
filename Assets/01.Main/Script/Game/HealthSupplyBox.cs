using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSupplyBox : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_canvas;
    GameObject m_player;
    Animator m_anim;
    //AudioSource m_audioSource;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponentInChildren<Animator>();
        m_canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player != null)
        {
            m_canvas.transform.LookAt(m_player.transform.position);

            if (Input.GetKeyDown(KeyCode.F))
            {
                Player_StateManager scr;
                if (m_player.gameObject.name.Equals("UpperBodyLean"))
                {
                    scr = m_player.GetComponentInParent<Player_StateManager>();
                }
                else
                {
                    scr = m_player.GetComponentInParent<Player_StateManager>();
                }

                scr.GetHealSupplyment();
                SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HEALTH_SUPPLY, 1.5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_player = other.gameObject;
            m_canvas.SetActive(true);
            m_anim.SetBool("ISPLAYERIN", true);
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.SUPPLYBOX_OPEN, gameObject.transform.position, 10, 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_player = null;
            m_canvas.SetActive(false);
            m_anim.SetBool("ISPLAYERIN", false);
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.SUPPLYBOX_CLOSE, gameObject.transform.position, 10, 1f);
        }
    }
    #endregion
}
