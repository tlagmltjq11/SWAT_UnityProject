using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    #region Field
    public enum eAudioClip
    {
        FOOTSTEP1,
        FOOTSTEP2,
        FOOTSTEP3,
        FOOTSTEP4,
        JUMP,
        LAND,
        AKM_SHOOT,
        M4_SHOOT,
        GLOCK_SHOOT,
        AKM_DRAW,
        GLOCK_DRAW,
        AKM_RELOAD,
        GLOCK_RELOAD,
        AIM_IN,
        GRUNT1,
        GRUNT2,
        GRUNT3,
        GRUNT4,
        ENEMY_DEATH1,
        ENEMY_DEATH2,
        ENEMY_DEATH3,
        HITSOUND,
        HEADSHOT,
        SUPPLY,
        PLAYER_DEATH,
        M4_RELOAD,
        ATW_EXPLOSION1,
        ATW_EXPLOSION2,
        ATW_IMPACTONTGROUND,
        ATW_THROW,
        SUPPLYBOX_OPEN,
        SUPPLYBOX_CLOSE,
        ATW_CHANGE,
        HEALTH_SUPPLY,
        UNTIE_HOSTAGE,
        FLASH_ON,
        FLASH_OFF,
        BUTTON,
        MAX
    }

    [SerializeField]
    AudioClip[] m_clips;
    [SerializeField]
    AudioSource m_2DSoundSource;
    [SerializeField] //Play Method 사용
    AudioSource m_2DSoundSource_Play;
    [SerializeField]
    AudioSource m_BGMSource;
    [SerializeField]
    GameObject m_objPoolManager; //오브젝트풀 매니저 하위에 모든 풀링 오브젝트들이 들어가있음.
    [SerializeField]
    AudioSource m_helicopter;
    [SerializeField]
    AudioSource m_policeCar;
    [SerializeField]
    AudioMixer m_audioMixer;
    List<AudioSource> m_pausedAudios = new List<AudioSource>();
    #endregion

    #region Unity Methods
    void Start()
    {
        BGMAudioControl(PlayerPrefs.GetFloat("BGMVolume"));
        EffectAudioControl(PlayerPrefs.GetFloat("SFXVolume"));
    }
    #endregion

    #region Public Methods
    public void EffectAudioControl(float volume)
    {
        if(volume == -40f)
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

    //PlayOneShot Method 사용
    public void Play2DSound(eAudioClip clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[(int)clip], volume);
    }

    public void Play2DSound(int clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[clip], volume);
    }

    //Play Method 사용
    public void Play2DSound_Play(int clip, float volume)
    {
        m_2DSoundSource_Play.clip = m_clips[clip];
        m_2DSoundSource_Play.volume = volume;
        m_2DSoundSource_Play.Play();
    }

    public void Play3DSound(eAudioClip clip, Vector3 pos, float maxDistance, float volume)
    {
        var obj = ObjPool.Instance.m_audioPool.Get();

        if (obj != null)
        {
            AudioSource audio = obj.gameObject.GetComponent<AudioSource>();
            audio.priority = 128;
            audio.clip = m_clips[(int)clip];

            obj.transform.position = pos;
            audio.maxDistance = maxDistance;

            obj.gameObject.SetActive(true);
            audio.Play();
            obj.ReturnInvoke(m_clips[(int)clip].length);
        }
    }

    public void Play3DSound(int clip, Vector3 pos, float maxDistance, float volume)
    {
        var obj = ObjPool.Instance.m_audioPool.Get();

        if (obj != null)
        {
            AudioSource audio = obj.gameObject.GetComponent<AudioSource>();
            audio.priority = 128;
            audio.clip = m_clips[clip];

            obj.transform.position = pos;
            audio.maxDistance = maxDistance;

            obj.gameObject.SetActive(true);
            audio.Play();
            obj.ReturnInvoke(m_clips[clip].length);
        }
    }

    public void Cop_SoundStart()
    {
        m_helicopter.Play();
        m_policeCar.Play();
    }

    public void StopSound()
    {
        if (m_2DSoundSource.isPlaying)
        {
            m_2DSoundSource.Pause();
            m_pausedAudios.Add(m_2DSoundSource);
        }
        if (m_2DSoundSource_Play.isPlaying)
        {
            m_2DSoundSource_Play.Pause();
            m_pausedAudios.Add(m_2DSoundSource_Play);
        }
        if (m_BGMSource.isPlaying)
        {
            m_BGMSource.Pause();
            m_pausedAudios.Add(m_BGMSource);
        }

        m_helicopter.Pause();
        m_policeCar.Pause();

        //3d오브젝트풀링 오디오소스들 현재 재생중인것들만 처리중
        var sources = m_objPoolManager.GetComponentsInChildren<AudioSource>();

        foreach(AudioSource audio in sources)
        {
            audio.Pause();
            m_pausedAudios.Add(audio);
        }
    }

    public void ReStartSound()
    { 
        //퍼지된 오디오소스만 리스트에 저장해두었다가 재생시킴.
        for (int i=0; i<m_pausedAudios.Count; i++)
        {
            m_pausedAudios[i].Play();
        }

        m_pausedAudios.Clear(); //클리어

        m_helicopter.Play();
        m_policeCar.Play();
    }

    #endregion
}
