using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ATTACK : FSMSingleton<Enemy_ATTACK>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
    
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if(e.SearchTarget())
        {
            Vector3 dir = e.m_player.transform.position - e.gameObject.transform.position;
            e.gameObject.transform.forward = Vector3.Lerp(e.gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);
            //e.m_upperBody.transform.forward = Vector3.Lerp(e.m_upperBody.transform.forward, dir, Time.deltaTime * 3f);
            //슛포인트도 조정해야함.

            if (!e.m_info.IsName("RELOAD"))
            {
                if (e.canAttack())
                {
                    e.m_anim.SetBool("ISATTACK", true);
                }
                else
                {
                    e.m_idleTime = 1f;//빠르게 재추적하도록 유도.
                    e.ChangeState(Enemy_IDLE.Instance);
                }
            }
        }
        else
        {
            e.ChangeState(Enemy_IDLE.Instance);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISATTACK", false);
    }
}
