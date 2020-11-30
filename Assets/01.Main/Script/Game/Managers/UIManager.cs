﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    #region Feild
    [SerializeField]
    GameObject m_UIPlay;
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
    public GameObject m_missionText;
    public GameObject m_keyStrokeEXP;
    public TextMeshProUGUI m_timerText;
    public TextMeshProUGUI m_startText;
    [SerializeField]
    GameObject m_UIDIe;
    public int m_leftTime; // 스테이지 정보로 받아와야함.
    #endregion

    protected override void OnStart()
    {
        m_keyStrokeEXP.SetActive(false);
        m_progressObj.SetActive(false);
        StartCoroutine("startGame");
        m_leftTime = 180;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            MissionTextOn();
            CancelInvoke();
        }

        if(Input.GetKeyUp(KeyCode.F1))
        {
            MissionTextOff();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            m_keyStrokeEXP.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.F2))
        {
            m_keyStrokeEXP.SetActive(false);
        }
    }

    #region Public Methods
    public void StopTimer()
    {
        StopCoroutine("startTimer");
    }

    public void StartTimer()
    {
        StartCoroutine("startTimer");
    }

    public void MissionTextOn()
    {
        m_missionText.SetActive(true);
    }

    public void MissionTextOff()
    {
        m_missionText.SetActive(false);
    }

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

        if(!OnOff)
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

    public void BloodScreenOn(float amt)
    {
        StartCoroutine(bloodScreen(amt));
    }

    public void PlayerDie()
    {
        m_UIPlay.SetActive(false);
        m_UIDIe.SetActive(true);
    }
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

    IEnumerator startGame()
    {
        for(int i=3; i>0; i--)
        {
            m_startText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        m_startText.text = "START";
        yield return new WaitForSecondsRealtime(1f);

        m_startText.enabled = false;
        MissionTextOff();
        GameManager.Instance.GameStart();
        StartCoroutine("startTimer");
    }

    IEnumerator startTimer()
    {
        while (m_leftTime != 0)
        {
            int minute, second;

            minute = m_leftTime / 60;
            second = m_leftTime % 60;

            m_timerText.text = string.Format("{0:00} : {1:00}", minute, second);
            m_leftTime--;
            yield return new WaitForSecondsRealtime(1f);
        }

        //타임오버
        if (m_leftTime == 0)
        {
            GameManager.Instance.SetState(GameManager.eGameState.TimeOver);
        }
    }
    #endregion
}
