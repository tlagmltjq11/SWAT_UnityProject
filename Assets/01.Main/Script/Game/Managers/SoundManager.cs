using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float m_totalVolume = 1f;
    #endregion

    #region Public Methods
    //PlayOneShot Method 사용
    public void Play2DSound(eAudioClip clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[(int)clip], volume * m_totalVolume);
    }

    public void Play2DSound(int clip, float volume)
    {
        m_2DSoundSource.PlayOneShot(m_clips[clip], volume * m_totalVolume);
    }

    //Play Method 사용
    public void Play2DSound_Play(int clip, float volume)
    {
        m_2DSoundSource_Play.clip = m_clips[clip];
        m_2DSoundSource_Play.volume = volume * m_totalVolume;
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
    #endregion
}
