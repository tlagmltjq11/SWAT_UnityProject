using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Field
    public enum eGameState
    {
        Normal,
        Pause,
        PlayerDead,
        Success,
        Max
    }

    eGameState m_state;
    [SerializeField]
    GameObject m_player;
    [SerializeField]
    GameObject m_camera;
    [SerializeField]
    GameObject m_WaitBox;
    GameObject m_failViewCam;
    Vector3 FailViewPosition;
    Player_StateManager m_playScr;
    CameraRotate m_camScr;
    WeaponSway m_sway;
    bool m_isStart;

    int m_time;
    int m_score;
    public GameObject[] m_wave2Enemys;
    public GameObject m_RescuePoint;
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_isStart = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_playScr = m_player.GetComponent<Player_StateManager>();
        m_camScr = m_camera.GetComponent<CameraRotate>();
        m_sway = m_player.GetComponentInChildren<WeaponSway>();

        m_time = 0;
        m_score = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && m_isStart && m_state != eGameState.PlayerDead) //게임시작 카운트다운중에는 못누르게끔 막아둔것.
        {
            if (Time.timeScale == 0)
            {
                SetState(eGameState.Normal);
                Time.timeScale = 1;
            }
            else
            {
                SetState(eGameState.Pause);
                Time.timeScale = 0;
            }
        }
    }
    #endregion

    #region Public Methods

    public void HostageRescued()
    {
        foreach (GameObject obj in m_wave2Enemys)
        {
            obj.SetActive(true);
        }

        m_RescuePoint.SetActive(true);
    }

    public void GameStart()
    {
        m_isStart = true;
        m_WaitBox.SetActive(false);
        StartCoroutine("Timer");
    }

    public void AddScore(int score)
    {
        m_score += score;
    }

    public eGameState GetState()
    {
        return m_state;
    }

    public void SetState(eGameState state)
    {
        if (m_state == state)
        {
            return;
        }

        m_state = state;

        switch (m_state)
        {
            case eGameState.Normal:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                m_playScr.enabled = true;
                m_camScr.enabled = true;
                m_sway.enabled = true;
                SoundManager.Instance.ReStartSound();
                UIManager.Instance.CloseMenu();
                break;

            case eGameState.Pause:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_playScr.enabled = false;
                m_camScr.enabled = false;
                m_sway.enabled = false;
                SoundManager.Instance.StopSound();
                UIManager.Instance.OpenMenu();
                break;

            case eGameState.PlayerDead:
                StopCoroutine("Timer");
                UIManager.Instance.GameResult(false, m_time, m_score);
                StartCoroutine("FailView");
                m_player.gameObject.SetActive(false);
                break;

            case eGameState.Success:
                StopCoroutine("Timer");
                UIManager.Instance.CloseMenu();
                UIManager.Instance.GameResult(true, m_time, m_score);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                m_playScr.enabled = false;
                m_camScr.enabled = false;
                m_sway.enabled = false;
                m_player.layer = LayerMask.NameToLayer("Default"); //적들이 공격하지 못하도록 레이어를 일시적으로 변경
                break;
        }
    }
    #endregion

    #region Private Methods
    public bool GetIsStart()
    {
        return m_isStart;
    }
    #endregion

    #region Coroutine
    IEnumerator FailView()
    {
        m_failViewCam = new GameObject("FailViewCam");
        m_failViewCam.gameObject.AddComponent<Camera>();
        m_failViewCam.gameObject.AddComponent<AudioListener>();

        m_failViewCam.gameObject.transform.position = m_camera.transform.position;
        m_failViewCam.gameObject.transform.eulerAngles = new Vector3(0f, m_camera.transform.eulerAngles.y, 0f);

        FailViewPosition = new Vector3(m_failViewCam.transform.position.x, m_failViewCam.transform.position.y + 2f, m_failViewCam.transform.position.z);

        while (true)
        {
            m_failViewCam.transform.rotation = Quaternion.Slerp(m_failViewCam.transform.rotation, Quaternion.Euler(90f, m_failViewCam.transform.eulerAngles.y, 0f), Time.deltaTime * 3f);
            m_failViewCam.transform.position = Vector3.Lerp(m_failViewCam.transform.position, FailViewPosition, Time.deltaTime * 2f);
            yield return null;

            if(m_failViewCam.transform.position == FailViewPosition)
            {
                Debug.Log("break");
                yield break;
            }
        }
    }

    IEnumerator Timer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            m_time++;
        }
    }

    //페이드인 아웃 코루틴 만들어야함.
    #endregion
}
