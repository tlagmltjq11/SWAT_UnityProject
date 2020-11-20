using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DIE : FSMSingleton<Enemy_DIE>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.RagdollOnOff(false);
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        e.m_dieTime += Time.deltaTime;

        if(e.m_dieTime >= 5f)
        {
            e.gameObject.SetActive(false);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        
    }
}
