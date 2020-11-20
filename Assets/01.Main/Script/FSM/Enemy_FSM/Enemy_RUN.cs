using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RUN : FSMSingleton<Enemy_RUN>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.speed = 2.5f;
        e.m_footstepCycle = 0.5f;
        //e.m_navAgent.stoppingDistance = e.m_attackSight;
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (e.SearchTarget())
        {
            e.m_anim.SetBool("ISRUN", true);
            e.m_navAgent.SetDestination(e.m_player.transform.position);

            #region Footstep
            e.m_footstepTimer += Time.deltaTime;

            if (e.m_footstepTimer > e.m_footstepCycle)
            {
                if (e.m_footstepTurn == 0)
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 1;
                }
                else if (e.m_footstepTurn == 1)
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 0;
                }

                e.m_footstepTimer = 0f;
            }
            #endregion

            if (e.canAttack())
            {
                e.ChangeState(Enemy_IDLE.Instance);
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
        e.m_navAgent.ResetPath();
        e.m_anim.SetBool("ISRUN", false);
        e.m_footstepCycle = 0f;
        e.m_idleTime = 1.5f; //대기시간없이 바로 attack 상태로 전이되게끔 유도.
    }
}
