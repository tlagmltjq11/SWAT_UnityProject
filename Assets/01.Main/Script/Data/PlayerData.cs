using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public enum eStage
    {
        stage1,
        stage2,
        stage3,
        Max
    }

    public int[] m_bestScore;
    public int[] m_bestTime;

    public PlayerData()
    {
        m_bestScore = new int[(int)eStage.Max];
        m_bestTime = new int[(int)eStage.Max];
    }
}
