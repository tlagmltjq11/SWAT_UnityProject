using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    #region Feild
    [SerializeField]
    GameObject m_UIPlay;
    public GameObject m_crossHair;
    public Text m_bulletText;
    public Text m_remainATWText;
    public Text m_curWeaponText;
    public Text m_HpText;
    public Image m_bloodScreen;
    public SpriteAtlas m_atlas;
    public Image m_curGunImg;
    public Image m_curATWImg;
    public GameObject m_progressObj;
    public Image m_progressBar;
    [SerializeField]
    GameObject m_UIDIe;
    #endregion

    protected override void OnStart()
    {
        m_progressObj.SetActive(false);
    }

    #region Public Methods
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
    #endregion
}
