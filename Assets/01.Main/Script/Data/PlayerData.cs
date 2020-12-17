using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public enum eStage
    {
        Stage1 = 1,
        Stage2,
        Stage3,
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
