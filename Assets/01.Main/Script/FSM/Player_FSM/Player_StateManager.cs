using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
public class Player_StateManager : FSM<Player_StateManager>
{
    //상태들의 변수를 매니저에 전부 때려박는 이유는 상태들이 싱글턴이기 때문에 전역으로 관리되는데, 거기서 변수를 관리해버리면
    //해당 상태를 사용하는 모든 유닛들의 변수값이 같이 바뀌게 되기 때문에, 각자 유닛들의 매니저에서 변수를 관리해야하는것임.
    //상태클래스는 오직 하는 일만!★★★
    #region Field
    #region Enums
    //enums
    public enum eWeaponType
    {
        AR,
        PISTOL,
        Grenade,
        MAX
    }
    #endregion
    #region References
    //References
    public Camera m_camera;
    public CharacterController m_charCon;
    public Animator m_animator;
    public Transform m_groundCheck;
    public GameObject[] m_weapons;
    public Weapon m_currentWeapon;
    public ATW m_currentATW;
    public GameObject m_upperBodyLean;
    public Light m_flashLight;
    #endregion
    #region Player Info
    //Player Info
    float m_hp = 100;
    public float m_runSpeed = 5f;
    public float m_walkSpeed = 1.5f;
    public float m_crouchSpeed = 2.5f;
    public float m_normalSpeed = 3.5f;
    public float m_crouchHeight = 0.475f;
    public float m_playerHeight = 1.075f;
    public float m_jumpHeight = 1.5f;
    #endregion
    #region About Player Move
    //Player Move
    public Vector3 m_moveDir = Vector3.zero;
    public float m_horiz = 0f;
    public float m_vert = 0f;
    public float m_diagonalSpeedModifier;
    public Vector3 m_crouchCheckPos;
    public Vector3 m_normalCheckPos;
    public Vector3 m_velocity;
    public float m_groundDistance = 0.2f;
    public float m_gravity = -21f;
    public LayerMask m_groundMask;
    public float m_footstepTimer;
    public float m_footstepCycle;
    public float m_footstepVolume;
    public int m_footstepTurn = 0;
    public bool m_PreviouslyGrounded;
    public float m_gunMoveSpeed;//움직일때 총 좌우로 흔들리는 속도조절.
    #endregion
    #region State Check Vars
    //State Check Vars
    public bool m_isGrounded;
    public bool m_isCrouching;
    public bool m_isWalking;
    public bool m_isRunning;
    public bool m_crouchAccuracyApply;
    public bool m_jumpAccruacyApply;
    public bool m_isLeanQ;
    public bool m_isLeanE;
    public bool m_isLeanDouble;
    public eWeaponType m_WPType;
    #endregion
    #endregion

    #region Unity Methods
    void Start() 
    {
        Init();
        InitState(this, Player_MOVE.Instance); 
    }

    void Update()
    { 
        FSMUpdate();
        #region Weapon Handle
        if (m_currentWeapon != null || m_currentATW != null)
        {
            // 총기 발사
            if(m_WPType == eWeaponType.AR)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (m_currentWeapon.m_currentBullets > 0 && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing)
                    {
                        m_currentWeapon.m_isFiring = true;
                        m_currentWeapon.Fire();
                        UIManager.Instance.Update_Bullet(m_currentWeapon.m_currentBullets, m_currentWeapon.m_bulletsRemain);
                    }
                }

                if (Input.GetButtonUp("Fire1") && !m_isRunning)
                {
                    m_currentWeapon.m_isFiring = false;
                    m_currentWeapon.StopFiring();
                }
            }
            else if(m_WPType == eWeaponType.PISTOL)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (m_currentWeapon.m_currentBullets > 0 && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing)
                    {
                        m_currentWeapon.Fire();
                        UIManager.Instance.Update_Bullet(m_currentWeapon.m_currentBullets, m_currentWeapon.m_bulletsRemain);
                    }
                }
            }

            //투척물 투척하기
            if (Input.GetKeyDown(KeyCode.G) && !m_isRunning && !m_currentWeapon.m_isReloading && !m_currentWeapon.m_isAiming && !m_currentWeapon.m_isDrawing && (m_isLeanE == false && m_isLeanQ == false) && !m_currentWeapon.m_isFiring)
            {
                if(m_currentATW.m_remainNum > 0 && m_currentATW.m_remainNum <= 2)
                {
                    GameObject grenade = Instantiate(m_currentATW.gameObject, m_currentATW.transform.position, m_currentATW.transform.rotation);
                    var scr = grenade.GetComponent<ATW>();
                    grenade.transform.localScale = Vector3.one;
                    grenade.SetActive(true);

                    SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.ATW_THROW, 1f);
                    scr.m_rigid.AddForce(grenade.transform.up * 5f + grenade.transform.forward * 15f, ForceMode.Impulse);
                    scr.Operation();

                    m_currentATW.m_remainNum--;
                    UIManager.Instance.Update_RemainATW(m_currentATW.m_remainNum);
                }
            }

            // 줌인 줌아웃
            if (Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && !m_currentWeapon.m_isAiming && !m_isRunning && !m_currentWeapon.m_isDrawing)
            {
                m_currentWeapon.AimIn();
            }
            else if (Input.GetButtonDown("Fire2") && !m_currentWeapon.m_isReloading && m_currentWeapon.m_isAiming && !m_isRunning)
            {
                m_currentWeapon.AimOut();
            }

            // 재장전
            if (Input.GetKeyDown(KeyCode.R) && !m_currentWeapon.m_isReloading && !m_isRunning && !m_currentWeapon.m_isDrawing && m_currentWeapon.m_currentBullets != m_currentWeapon.m_bulletsPerMag)
            {
                if (m_currentWeapon.m_isAiming)
                {
                    m_currentWeapon.AimOut();
                }

                //기울이기를 취소시킴.
                m_isLeanQ = false;
                m_isLeanE = false;
                m_isLeanDouble = true;
                m_currentWeapon.Reload();
            }

            // sight 스왑
            if (Input.GetKeyDown(KeyCode.T) && !m_currentWeapon.m_isAiming && !m_currentWeapon.m_isReloading && !m_currentWeapon.m_isDrawing && !m_isRunning)
            {
                m_currentWeapon.ChangeSight();
            }

            // 무기 스왑
            for (int i = 49; i < 58; i++)
            {
                if (Input.GetKeyDown((KeyCode)i) && m_weapons.Length > i - 49)
                {
                    if(m_WPType != (eWeaponType)(i-49))
                    {
                        if((eWeaponType)(i - 49) == eWeaponType.Grenade)
                        {
                            SwitchATW(i - 49);
                        }
                        else
                        {
                            SwitchWeapons(i - 49);
                        }
                    }
                }
            }

            // 총 발사 간격
            if (m_currentWeapon.m_fireTimer < m_currentWeapon.m_fireRate)
            {
                m_currentWeapon.m_fireTimer += Time.deltaTime;
            }
        }
        #endregion
        #region FlashLight
        if(Input.GetKeyDown(KeyCode.Y))
        {
            if(!m_flashLight.enabled)
            {
                m_flashLight.enabled = true;
                SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.FLASH_ON, 1f);
            }
            else
            {
                m_flashLight.enabled = false;
                SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.FLASH_OFF, 1f);
            }
        }
        #endregion
    }
    #endregion

    #region Private Methods
    void Init()
    {
        m_charCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_crouchCheckPos = m_groundCheck.localPosition + new Vector3(0f, 0.2f, 0f); //앉을때 그라운드체크 포지션이 바닥밑으로 깔리는것을 방지하기 위함.
        m_normalCheckPos = m_groundCheck.localPosition;
        m_isGrounded = false;
        m_isCrouching = false;
        m_isWalking = false;
        m_crouchAccuracyApply = false;
        m_jumpAccruacyApply = false;
        m_isLeanQ = false;
        m_isLeanE = false;
        m_isLeanDouble = false;
        m_flashLight.enabled = false;

        // Default Weapon Setting.
        for (int i=0; i<m_weapons.Length; i++)
        {
            m_weapons[i].SetActive(false);
        }
        SwitchWeapons(0);
        SwitchATW(2);

        // Default Weapon Setting END

        UIManager.Instance.Update_HP(m_hp);
    }

    void SetCurrentWeapon(Weapon weapon)
    {
        m_currentWeapon = weapon;
        UIManager.Instance.Update_Bullet(weapon.m_currentBullets, weapon.m_bulletsRemain);
        UIManager.Instance.Update_CurWeaponText(weapon.m_weaponName);
        UIManager.Instance.Update_CurWeaponImage(m_currentWeapon.m_weaponName);
    }

    void SetCurrentATW(ATW atw)
    {
        m_currentATW = atw;
        UIManager.Instance.Update_CurATWImg(atw.m_name);
    }

    private void SwitchWeapons(int newWeapon)
    {
        for (int i = 0; i < m_weapons.Length; i++)
        {
            if (m_weapons[i].activeSelf)
            {
                m_weapons[i].SetActive(false);
                break;
            }
        }

        m_weapons[newWeapon].SetActive(true);
        m_WPType = (eWeaponType)newWeapon;

        SoundManager.Instance.Play2DSound_Play((int)SoundManager.eAudioClip.AKM_DRAW + newWeapon, 0.7f); //나중에 총기가 많아지면 총기의 이름으로 호출해줘야함.
        SetCurrentWeapon(m_weapons[newWeapon].GetComponent<Weapon>());
    }

    private void SwitchATW(int newWeapon)
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.ATW_CHANGE, 1.8f);
        SetCurrentATW(m_weapons[newWeapon].GetComponent<ATW>());
    }
    #endregion

    #region Public Methods
    public void Damaged(float dmg)
    {
        if(m_hp <= 0f)
        {
            return;
        }

        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
            ChangeState(Player_DIE.Instance);
        }
        else
        {
            m_hp = m_hp - dmg;
            SoundManager.Instance.Play2DSound(Random.Range((int)SoundManager.eAudioClip.GRUNT1, (int)SoundManager.eAudioClip.ENEMY_DEATH1), 0.4f);
            UIManager.Instance.BloodScreenOn(dmg * 12.5f);
        }

        UIManager.Instance.Update_HP(m_hp);
    }

    public void GetAmmoSupplyment()
    {
        //총알보급
        for (int i = 0; i < 2; i++)
        {
            Weapon weapon = m_weapons[i].GetComponent<Weapon>();
            weapon.m_currentBullets = weapon.m_bulletsPerMag;
            weapon.m_bulletsRemain = weapon.m_totalMag * weapon.m_bulletsPerMag;
        }

        //투척물보급
        for(int i = 2; i < m_weapons.Length; i++)
        {
            ATW atw = m_weapons[i].GetComponent<ATW>();
            atw.m_remainNum = 2;
        }

        UIManager.Instance.Update_Bullet(m_currentWeapon.m_currentBullets, m_currentWeapon.m_bulletsRemain);
        UIManager.Instance.Update_RemainATW(m_currentATW.m_remainNum);
    }

    public void GetHealSupplyment()
    {
        if(m_hp + 50 >= 100)
        {
            m_hp = 100;
        }
        else
        {
            m_hp += 50;
        }

        UIManager.Instance.Update_HP(m_hp);
    }
    #endregion
}