using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_IDLE : FSMSingleton<Enemy_IDLE>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_anim.Rebind();
        e.m_navAgent.ResetPath();
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        e.m_idleTime += Time.deltaTime;

        if (e.m_idleTime >= 1.5f)
        {
            if (e.SearchTarget())
            {
                if (e.canAttack())
                {
                    e.ChangeState(Enemy_ATTACK.Instance);
                }
                else
                {
                    e.ChangeState(Enemy_RUN.Instance);
                }
            }
            else
            {
                e.ChangeState(Enemy_PATROL.Instance);
            }
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_idleTime = 0f;
    }
}
