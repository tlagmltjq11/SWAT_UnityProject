using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerDataManager : DonDestroy<PlayerDataManager>
{
    PlayerData m_myData = new PlayerData();

    public int GetBestScore(PlayerData.eStage stage)
    {
        return m_myData.m_bestScore[(int)stage];
    }

    public void SetBestScore(PlayerData.eStage stage, int score)
    {
        m_myData.m_bestScore[(int)stage] = score;
        PlayerPrefs.SetInt(stage.ToString() + "Score", score);
        PlayerPrefs.Save();
    }

    public int GetBestTime(PlayerData.eStage stage)
    {
        return m_myData.m_bestTime[(int)stage];
    }

    public void SetBestTime(PlayerData.eStage stage, int time)
    {
        m_myData.m_bestTime[(int)stage] = time;
        PlayerPrefs.SetInt(stage.ToString() + "Time", time);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        //key만 알고있어도 가져올 수 있지만, 저장한적이 없다면 뒤에 있는 디폴트값을 가져오게 된다.
        m_myData.m_bestScore[1] = PlayerPrefs.GetInt("Stage1" + "Score");
        m_myData.m_bestScore[2] = PlayerPrefs.GetInt("Stage2" + "Score");
        m_myData.m_bestScore[3] = PlayerPrefs.GetInt("Stage3" + "Score");

        m_myData.m_bestTime[1] = PlayerPrefs.GetInt("Stage1" + "Time");
        m_myData.m_bestTime[2] = PlayerPrefs.GetInt("Stage2" + "Time");
        m_myData.m_bestTime[3] = PlayerPrefs.GetInt("Stage3" + "Time");
    }


    private void Start()
    {
        LoadData();
    }
}
