using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hostage_Controller : MonoBehaviour
{
    #region Field
    public enum eState
    {
        TIED,
        RESCUE,
        IDLE,
        RUN,
        MAX
    }

    eState m_state;
    Animator m_anim;
    NavMeshAgent m_nav;
    float m_disFromPlayer;
    AnimatorStateInfo m_info;
    GameObject m_player;
    float m_footstepTimer;
    float m_footstepCycle;
    int m_footstepTurn;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_nav = GetComponent<NavMeshAgent>();
        m_disFromPlayer = 3f;
        m_state = eState.TIED;
        m_footstepTimer = 0f;
        m_footstepCycle = 0.5f;
        m_footstepTurn = 0;
    }

    void Update()
    {
        if(m_player != null && m_state == eState.TIED)
        {
            if (Input.GetKey(KeyCode.F))
            {
                if (UIManager.Instance.ProgressBarFill(0.2f))
                {
                    m_state = eState.RESCUE;
                    UIManager.Instance.ProgressObjOnOff(false);
                    SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.UNTIE_HOSTAGE, 1f);
                }
            }
            else
            {
                UIManager.Instance.ProgressBarFill(-0.2f);
            }
        }

        switch(m_state)
        {
            case eState.TIED:
                break;

            case eState.RESCUE:
                m_anim.SetTrigger("ISRESCUE");

                m_info = m_anim.GetCurrentAnimatorStateInfo(0);
                if (m_info.IsName("Hostage_IDLE"))
                {
                    GameManager.Instance.AddScore(500);
                    GameManager.Instance.HostageRescued();
                    m_state = eState.IDLE;
                }
                break;

            case eState.IDLE:
                m_nav.ResetPath();
                Vector3 dir = m_player.transform.position - gameObject.transform.position;
                gameObject.transform.forward = Vector3.Lerp(gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);

                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) > m_disFromPlayer)
                {
                    m_state = eState.RUN;
                }
                break;

            case eState.RUN:
                m_anim.SetBool("ISRUN", true);
                m_nav.SetDestination(m_player.transform.position);

                #region Footstep
                m_footstepTimer += Time.deltaTime;

                if (m_footstepTimer > m_footstepCycle)
                {
                    if (m_footstepTurn == 0)
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 1;
                    }
                    else if (m_footstepTurn == 1)
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 0;
                    }

                    m_footstepTimer = 0f;
                }
                #endregion

                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) <= m_disFromPlayer)
                {
                    m_anim.SetBool("ISRUN", false);
                    m_state = eState.IDLE;
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED )
        {
            m_player = other.transform.root.gameObject;
            UIManager.Instance.ProgressObjOnOff(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED)
        {
            UIManager.Instance.ProgressObjOnOff(false);
        }
    }
    #endregion
}
