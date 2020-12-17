using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour
{
    #region Field
    public AudioClip m_button;
    public AudioClip m_BGM;
    public AudioSource m_btnAudioSource;
    public AudioSource m_bgmAudioSource;
    public AudioMixer m_audioMixer;
    [SerializeField]
    GameObject m_startPanel;
    [SerializeField]
    GameObject m_optionPanel;
    [SerializeField]
    GameObject m_keyStrokePanel;
    [SerializeField]
    GameObject m_volumePanel;
    [SerializeField]
    Slider m_BGMSlider;
    [SerializeField]
    Slider m_SFXSlider;
    [SerializeField]
    TextMeshProUGUI[] m_bestScore;
    [SerializeField]
    TextMeshProUGUI[] m_bestTime;
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        m_btnAudioSource.clip = m_button;
        m_bgmAudioSource.clip = m_BGM;
        m_bgmAudioSource.loop = true;

        BGMAudioControl(PlayerPrefs.GetFloat("BGMVolume"));
        SfxAudioControl(PlayerPrefs.GetFloat("SFXVolume"));

        m_BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        m_SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        for(int i=0; i<m_bestScore.Length; i++)
        {
            m_bestScore[i].text = "Best Score : " + PlayerDataManager.Instance.GetBestScore((PlayerData.eStage)i).ToString();
            m_bestTime[i].text = "Best Time  : " + PlayerDataManager.Instance.GetBestTime((PlayerData.eStage)i).ToString();
        }

        m_bgmAudioSource.Play();
    }
    #endregion

    #region Public Methods

    #region UI
    public void ClickStart()
    {
        m_btnAudioSource.Play();

        if (m_startPanel.activeSelf)
        {
            m_startPanel.SetActive(false);
        }
        else
        {
            m_startPanel.SetActive(true);
        }
    }

    public void ClickOption()
    {
        m_btnAudioSource.Play();

        if (m_optionPanel.activeSelf)
        {
            m_keyStrokePanel.SetActive(false);
            m_volumePanel.SetActive(false);
            m_optionPanel.SetActive(false);
        }
        else
        {
            m_optionPanel.SetActive(true);
        }
    }

    public void ClickKeyStrokeEXPBtn()
    {
        m_btnAudioSource.Play();
        m_volumePanel.SetActive(false);

        if (m_keyStrokePanel.activeSelf)
        {
            m_keyStrokePanel.SetActive(false);
        }
        else
        {
            m_keyStrokePanel.SetActive(true);
        }
    }

    public void ClickVolumeBtn()
    {
        m_btnAudioSource.Play();
        m_keyStrokePanel.SetActive(false);

        if (m_volumePanel.activeSelf)
        {
            m_volumePanel.SetActive(false);
        }
        else
        {
            m_volumePanel.SetActive(true);
        }
    }

    public void ClickStage1()
    {
        m_btnAudioSource.Play();

        LoadSceneManager.LoadScene("1"); //stage1 의미
    }

    public void ClickExit()
    {
        m_btnAudioSource.Play();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit(); // 어플리케이션 종료
        #endif
    }

    public void BGMAudioControl(float volume)
    {

        if (volume == -40f)
        {
            m_audioMixer.SetFloat("BGMVolume", -80); //Mute 해주기위함
        }
        else
        {
            m_audioMixer.SetFloat("BGMVolume", volume);
        }

        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }

    public void SfxAudioControl(float volume)
    {
        if (volume == -40f)
        {
            m_audioMixer.SetFloat("SFXVolume", -80); //Mute 해주기위함
        }
        else
        {
            m_audioMixer.SetFloat("SFXVolume", volume);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    #endregion

    #endregion
}
