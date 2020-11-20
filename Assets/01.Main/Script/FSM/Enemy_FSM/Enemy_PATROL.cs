using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_PATROL : FSMSingleton<Enemy_PATROL>, IFSMState<Enemy_StateManager>
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.stoppingDistance = 0.5f;
        e.m_navAgent.speed = 1f;
        e.m_footstepCycle = 0.8f;
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (!e.SearchTarget())
        {
            if (!e.m_navAgent.hasPath)
            {
                //랜덤 패트롤링
                NavMeshHit hit;
                Vector3 finalPosition = Vector3.zero;
                Vector3 randomDirection = Random.insideUnitSphere * 5f;
                randomDirection += e.gameObject.transform.position;

                //randomDirection위치에 navMesh가 존재하여 갈 수 있는지 체크
                if (NavMesh.SamplePosition(randomDirection, out hit, 1f, 1))
                {
                    finalPosition = hit.position;
                }

                e.m_anim.SetBool("ISWALK", true);
                e.m_navAgent.SetDestination(finalPosition);
            }
            else
            {
                #region Footstep
                e.m_footstepTimer += Time.deltaTime;

                if (e.m_footstepTimer > e.m_footstepCycle)
                {
                    if (e.m_footstepTurn == 0)
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 15f, 1f);
                        e.m_footstepTurn = 1;
                    }
                    else if (e.m_footstepTurn == 1)
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 15f, 1f);
                        e.m_footstepTurn = 0;
                    }

                    e.m_footstepTimer = 0f;
                }
                #endregion

                if (e.m_navAgent.remainingDistance <= e.m_navAgent.stoppingDistance)
                {
                    e.m_navAgent.ResetPath();
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
        e.m_navAgent.ResetPath();
        e.m_anim.SetBool("ISWALK", false);
        e.m_footstepCycle = 0f;
    }
}
