using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_StateManager : FSM<Enemy_StateManager>
{
    #region Field
    public Animator m_anim;
    public GameObject m_player;
    public NavMeshAgent m_navAgent;
    public GameObject m_upperBody;
    public GameObject m_shootPoint;
    public LineRenderer m_lineRenderer;
    public AnimatorStateInfo m_info;
    public ParticleSystem muzzleFlash;
    public Rigidbody m_bodyRig;

    public float m_idleTime;
    public float m_dieTime;
    public float m_detectSight = 30f;
    public float m_attackSight = 15f;
    public int m_check;
    public float m_footstepTimer;
    public float m_footstepCycle;
    public float m_hp;
    public float m_bullets;
    public int m_footstepTurn = 0;

    public Transform m_wayPointObj;
    public Transform[] m_wayPoints;
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        Init();
        RagdollOnOff(true);
        InitState(this, Enemy_IDLE.Instance);
    }

    void OnDisable()
    {
        RagdollOnOff(true);
    }

    void Start()
    {
        if(m_wayPointObj != null)
        {
            m_wayPoints = m_wayPointObj.GetComponentsInChildren<Transform>();
        }
    }

    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0);

        FSMUpdate();
    }
    #endregion

    #region Private Methods
    void Init()
    {
        m_anim = GetComponent<Animator>();
        m_navAgent = GetComponent<NavMeshAgent>();

        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.01f;
        m_lineRenderer.endWidth = 0.01f;
        m_lineRenderer.startColor = Color.white;
        m_lineRenderer.endColor = Color.white;
        m_lineRenderer.enabled = false;
        
        m_idleTime = 0f;
        m_dieTime = 0f;
        m_check = 0;
        m_hp = 100f;
        m_bullets = 5f;

        m_player = GameObject.Find("Player");
        m_anim.Rebind();
    }
    #endregion

    #region Public Methods
    public bool SearchTarget()
    {
        var dir = (m_player.transform.position + Vector3.up * 0.4f) - (gameObject.transform.position + Vector3.up * 1.3f);

        m_check = 0;

        RaycastHit m_hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | (1 << LayerMask.NameToLayer("Interactable")));
        layerMask = ~layerMask;
        if (Physics.Raycast(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized, out m_hit, m_detectSight, layerMask))
        {
            if (m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_check++;
            }
        }

        if (m_check == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool canAttack()
    {
        var distance = Vector3.Distance(gameObject.transform.position, m_player.transform.position);

        if (distance <= m_attackSight)
        {
            return true;
        }

        return false;
    }

    public void Fire()
    {
        RaycastHit hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
        layerMask = ~layerMask;

        if (Physics.Raycast(m_shootPoint.transform.position, m_shootPoint.transform.forward + Random.onUnitSphere * 0.05f, out hit, 100f, layerMask))
        {
            m_lineRenderer.SetPosition(0, m_shootPoint.transform.position);
            m_lineRenderer.SetPosition(1, hit.point);
            StartCoroutine(ShowBulletLine());

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(hit.collider.gameObject.name.Equals("Player"))
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(5f);
                    }
                }
                else if(hit.collider.gameObject.name.Equals("UpperBodyLean"))
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponentInParent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(10f);
                    }
                }
            }
            else
            {
                var hitSpark = ObjPool.Instance.m_hitSparkPool.Get();

                if (hitSpark != null)
                {
                    hitSpark.gameObject.transform.position = hit.point;
                    hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    hitSpark.gameObject.SetActive(true);
                }
            }
        }

        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_SHOOT, gameObject.transform.position, 30f, 0.7f);
        muzzleFlash.Play();
        m_bullets--;

        if(m_bullets == 0)
        {
            m_anim.SetBool("ISATTACK", false);
            m_anim.CrossFadeInFixedTime("RELOAD", 0.01f);
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_RELOAD, gameObject.transform.position, 20f, 0.5f);
            m_bullets = 5f;
        }
    }

    public void Damaged(float dmg)
    {
        if (m_hp <= 0)
        {
            return;
        }

        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
            SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ENEMY_DEATH1, (int)SoundManager.eAudioClip.HITSOUND), gameObject.transform.position, 20f, 1.2f);
            ChangeState(Enemy_DIE.Instance);
        }
        else
        {
            m_hp = m_hp - dmg;

            if(canAttack())
            {
                ChangeState(Enemy_ATTACK.Instance);
            }
            else
            {
                ChangeState(Enemy_RUN.Instance);
            }
        }
    }

    public void RagdollOnOff(bool OnOff)
    {
        m_anim.enabled = OnOff;
        Rigidbody[] rigs = gameObject.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = OnOff;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator ShowBulletLine()
    {
        m_lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        m_lineRenderer.enabled = false;
    }
    #endregion
}
