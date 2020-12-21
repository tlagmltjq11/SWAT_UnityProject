using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    #region Feild
    [SerializeField]
    GameObject m_UIPlay;
    [SerializeField]
    GameObject m_UIResult;
    [SerializeField]
    GameObject m_Menu;
    [SerializeField]
    GameObject m_missionPanel;
    [SerializeField]
    GameObject m_keyStrokeEXPPanel;
    [SerializeField]
    GameObject m_volumePanel;
    [SerializeField]
    GameObject m_exitStagePanel;
    [SerializeField]
    GameObject m_gameOffPanel;
    [SerializeField]
    Slider m_BGMSlider;
    [SerializeField]
    Slider m_SFXSlider;
    [SerializeField]
    TextMeshProUGUI m_bestTimeText;
    [SerializeField]
    TextMeshProUGUI m_bestScoreText;
    [SerializeField]
    Image m_fadeInOut;

    public GameObject m_crossHair;
    public TextMeshProUGUI m_bulletText;
    public TextMeshProUGUI m_remainATWText;
    public TextMeshProUGUI m_curWeaponText;
    public TextMeshProUGUI m_HpText;
    public Image m_bloodScreen;
    public SpriteAtlas m_atlas;
    public Image m_curGunImg;
    public Image m_curATWImg;
    public GameObject m_progressObj;
    public Image m_progressBar;
    public TextMeshProUGUI m_startText;
    public TextMeshProUGUI m_resultText;
    public TextMeshProUGUI m_timerText;
    public TextMeshProUGUI m_scoreText;
    public TextMeshProUGUI m_missionText;

    public GameObject m_progressObj_Tl;
    public Image m_progressBar_Tl;
    public PlayableDirector m_introTl;
    public GameObject m_camTl;

    Coroutine m_startCrt = null;
    #endregion

    protected override void OnStart()
    {
        m_progressObj.SetActive(false);

        StartCoroutine("FadeIn"); //페이드인

        m_BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        m_SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        m_bestTimeText.gameObject.SetActive(false);
        m_bestScoreText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(m_introTl.state == PlayState.Playing)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (UIManager.Instance.ProgressBarFill_Tl(0.2f))
                {
                    GameManager.Instance.ReleasePlayer();
                    SoundManager.Instance.Cop_SoundStart();

                    m_introTl.Stop();
                    m_camTl.SetActive(false);
                    m_progressObj_Tl.SetActive(false);
                    
                    m_UIPlay.SetActive(true);

                    m_startCrt = StartCoroutine("startGame");
                }
            }
            else
            {
                UIManager.Instance.ProgressBarFill_Tl(-0.2f);
            }
        }

        //인트로 캠이 비활성화된 경우는 컷신이 끝났을 경우임.
        //고로 건너뛰기를 했거나 컷신이 끝난것으로 판단하는데, 이때 건너뛰기를 통해 이미 게임이 시작됐는지를 코루틴변수로 판단함.
        //고로 이 경우는 정상적으로 건너뛰기없이 컷신이끝난 후 게임을 시작시키는 경우.
        if(m_camTl.activeSelf == false && m_startCrt == null)
        {
            GameManager.Instance.ReleasePlayer();
            SoundManager.Instance.Cop_SoundStart();

            m_UIPlay.SetActive(true);
            m_progressObj_Tl.SetActive(false);

            m_startCrt = StartCoroutine("startGame");
        }
    }

    #region Public Methods

    #region Menu
    public void Intro()
    {
        GameManager.Instance.HoldPlayer();

        //-----컷신 시작-----//
        m_introTl.gameObject.SetActive(true);
        m_introTl.Play();
        m_progressObj_Tl.SetActive(true);
        //-------------------//
    }

    public void OpenMenu()
    {
        m_Menu.SetActive(true);
    }

    public void CloseMenu()
    {
        m_Menu.SetActive(false);
        m_missionPanel.SetActive(false);
        m_keyStrokeEXPPanel.SetActive(false);
        m_volumePanel.SetActive(false);
        m_exitStagePanel.SetActive(false);
        m_gameOffPanel.SetActive(false);
    }

    public void ClickCloseMenu()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        GameManager.Instance.SetState(GameManager.eGameState.Normal);
        Time.timeScale = 1;
    }

    public void ClickMissionBtn()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        m_keyStrokeEXPPanel.SetActive(false);
        m_volumePanel.SetActive(false);

        if (m_missionPanel.activeSelf)
        {
            m_missionPanel.SetActive(false);
        }
        else
        {
            m_missionPanel.SetActive(true);
        }
    }

    public void ClickKeyStrokeEXPBtn()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        m_missionPanel.SetActive(false);
        m_volumePanel.SetActive(false);

        if (m_keyStrokeEXPPanel.activeSelf)
        {
            m_keyStrokeEXPPanel.SetActive(false);
        }
        else
        {
            m_keyStrokeEXPPanel.SetActive(true);
        }
    }

    public void ClickVolumeBtn()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        m_missionPanel.SetActive(false);
        m_keyStrokeEXPPanel.SetActive(false);

        if (m_volumePanel.activeSelf)
        {
            m_volumePanel.SetActive(false);
        }
        else
        {
            m_volumePanel.SetActive(true);
        }
    }

    public void ClickStageOffBtn()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        if (m_exitStagePanel.activeSelf)
        {
            m_exitStagePanel.SetActive(false);
        }
        else
        {
            m_exitStagePanel.SetActive(true);
        }
    }

    public void ClickGameOffBtn()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        if (m_gameOffPanel.activeSelf)
        {
            m_gameOffPanel.SetActive(false);
        }
        else
        {
            m_gameOffPanel.SetActive(true);
        }
    }

    public void ClickGameOffYes()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }

    public void ClickExitStageYes()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        StartCoroutine(FadeOut("TitleScene"));
    }

    public void ClickRePlayStage()
    {
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.BUTTON, 1f);
        StartCoroutine(FadeOut(SceneManager.GetActiveScene().name));
    }
    #endregion

    #region Play
    public void Update_Bullet(float currentBullets, float bulletsRemain)
    {
        m_bulletText.text = currentBullets + " / " + bulletsRemain;
    }

    public void Update_RemainATW(float Remain)
    {
        m_remainATWText.text = Remain.ToString();
    }

    public void Update_HP(float hp)
    {
        m_HpText.text = hp.ToString();
    }

    public void Update_CurWeaponText(string name)
    {
        m_curWeaponText.text = name;
    }
    public void Update_CurATWImg(string name)
    {
        m_curATWImg.sprite = m_atlas.GetSprite(name);
    }

    public void Update_CurWeaponImage(string name)
    {
        m_curGunImg.sprite = m_atlas.GetSprite(name);
    }

    public void CrossHairOnOff(bool OnOff)
    {
        m_crossHair.SetActive(OnOff);
    }

    public void ProgressObjOnOff(bool OnOff)
    {
        m_progressObj.SetActive(OnOff);

        if (!OnOff)
        {
            m_progressBar.fillAmount = 0;
        }
    }

    public bool ProgressBarFill(float amt)
    {
        m_progressBar.fillAmount = Mathf.Lerp(m_progressBar.fillAmount, m_progressBar.fillAmount + amt, Time.deltaTime * 2f);

        if (m_progressObj.activeSelf && m_progressBar.fillAmount >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ProgressBarFill_Tl(float amt)
    {
        m_progressBar_Tl.fillAmount = Mathf.Lerp(m_progressBar_Tl.fillAmount, m_progressBar_Tl.fillAmount + amt, Time.deltaTime * 2f);

        if (m_progressObj_Tl.activeSelf && m_progressBar_Tl.fillAmount >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void BloodScreenOn(float amt)
    {
        StartCoroutine(bloodScreen(amt));
    }
    #endregion

    #region GameResult
    public void GameResult(bool flag, int time, int score)
    {
        if (flag)
        {
            m_resultText.text = "Mission Success";
        }
        else
        {
            m_resultText.text = "Mission Failed";
        }

        /*
        PlayerData.eStage temp;

        for (temp = PlayerData.eStage.stage1; temp < PlayerData.eStage.Max; temp++)
        {
            if (temp.ToString().Equals(SceneManager.GetActiveScene().name))
            {
                break;
            }
        }


        PlayerData.eStage temp = PlayerData.eStage.stage1;
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            temp = PlayerData.eStage.stage1;
        }*/

        PlayerData.eStage temp = (PlayerData.eStage)int.Parse(SceneManager.GetActiveScene().name);

        int bestTime = PlayerDataManager.Instance.GetBestTime(temp);
        int bestScore = PlayerDataManager.Instance.GetBestScore(temp);

        if (flag)
        {
            if (bestTime == 0f)
            {
                m_bestTimeText.gameObject.SetActive(true);
                PlayerDataManager.Instance.SetBestTime(temp, time);
            }
            else
            {
                if (time >= bestTime)
                {
                    m_bestTimeText.gameObject.SetActive(false);
                }
                else if (time < bestTime) //시간이 더 적게걸렸을때 미션도 성공해야 타임은 갱신.
                {
                    m_bestTimeText.gameObject.SetActive(true);
                    PlayerDataManager.Instance.SetBestTime(temp, time);
                }
            }
        }
        

        int minute, second;
        minute = time / 60;
        second = time % 60;
        m_timerText.text = string.Format("Time  : {0:00} : {1:00}", minute, second);

        if(score <= bestScore)
        {
            m_bestScoreText.gameObject.SetActive(false);
        }
        else
        {
            m_bestScoreText.gameObject.SetActive(true);
            PlayerDataManager.Instance.SetBestScore(temp, score);
        }

        m_scoreText.text = "Score : " + score.ToString();

        if(flag)
        {
            Invoke("GameResultUI", 0.5f);
        }
        else
        {
            Invoke("GameResultUI", 3f);
        }
    }

    public void GameResultUI()
    {
        m_UIPlay.SetActive(false);
        m_UIResult.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #endregion

    #region Coroutine
    IEnumerator bloodScreen(float a)
    {
        for (float i = a; i >= 0; i -= 1f)
        {
            Color color = new Vector4(m_bloodScreen.color.r, m_bloodScreen.color.g, m_bloodScreen.color.b, i / 255f);
            m_bloodScreen.color = color;
            yield return 0;
        }

        yield break;
    }

    IEnumerator FadeIn()
    {
        float FadeTime = 1f;
        float time;

        Color fadecolor = m_fadeInOut.color;

        time = 0f;

        while (fadecolor.a > 0f)
        {
            time += Time.deltaTime / FadeTime;

            fadecolor.a = Mathf.Lerp(1f, 0f, time);

            m_fadeInOut.color = fadecolor;

            yield return null;
        }

        m_missionText.gameObject.SetActive(true);
        m_startText.gameObject.SetActive(true);

        Intro();
    }

    IEnumerator FadeOut(string sceneName)
    {
        Time.timeScale = 1;

        float FadeTime = 2f;
        float time;

        Color fadecolor = m_fadeInOut.color;

        time = 0f;

        while (fadecolor.a < 1f)
        {
            time += Time.deltaTime / FadeTime;

            fadecolor.a = Mathf.Lerp(0f, 1f, time);

            m_fadeInOut.color = fadecolor;

            yield return null;
        }

        LoadSceneManager.LoadScene(sceneName);
    }

    IEnumerator startGame()
    {
        for (int i = 3; i > 0; i--)
        {
            m_startText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        m_startText.text = "START";
        yield return new WaitForSecondsRealtime(1f);

        m_startText.enabled = false;
        m_missionText.enabled = false;
        GameManager.Instance.GameStart();
    }
    #endregion
}
