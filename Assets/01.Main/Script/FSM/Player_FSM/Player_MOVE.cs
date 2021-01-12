using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_MOVE : FSMSingleton<Player_MOVE>, IFSMState<Player_StateManager>
{
    public void Enter(Player_StateManager e)
    {

    }

    public void Execute(Player_StateManager e)
    {
        #region Movement
        //앞이나 뒤로가는 키가 눌렸으면 1 또는 -1일것임. 이 값을 현재 플레이어의 forward에 곱해준 값과 right도 마찬가지로 왼쪽 오른쪽 값을 곱해줘서 두 벡터3를 합해주면
        //어느 방향으로 나아가야할지 정해지게 되는 것.
        e.m_horiz = Input.GetAxis("Horizontal");
        e.m_vert = Input.GetAxis("Vertical");

        e.m_isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && !e.m_currentWeapon.m_isReloading && !e.m_currentWeapon.m_isAiming;
        e.m_isCrouching = Input.GetKey(KeyCode.C) && !e.m_isRunning;
        e.m_isWalking = Input.GetKey(KeyCode.LeftControl) && !e.m_isRunning;

        #region Lean
        if (!e.m_currentWeapon.m_isReloading && !e.m_isRunning)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (!e.m_isLeanQ)
                {
                    if (e.m_isLeanE)
                    {
                        e.m_isLeanE = false;
                    }

                    e.m_isLeanQ = true;
                    e.m_isLeanDouble = false;
                }
                else
                {
                    e.m_isLeanDouble = true;
                    e.m_isLeanQ = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!e.m_isLeanE)
                {
                    if (e.m_isLeanQ)
                    {
                        e.m_isLeanQ = false;
                    }

                    e.m_isLeanE = true;
                    e.m_isLeanDouble = false;
                }
                else
                {
                    e.m_isLeanDouble = true;
                    e.m_isLeanE = false;
                }
            }
        }
        #endregion

        if (e.m_isRunning)
        {
            e.m_currentWeapon.GetComponent<Animator>().SetBool("ISRUNNING", true);
            
            e.m_isLeanQ = false;
            e.m_isLeanE = false;
            e.m_isLeanDouble = true;
        }
        else
        {
            e.m_currentWeapon.GetComponent<Animator>().SetBool("ISRUNNING", false);
        }

        if(e.m_isCrouching)
        {
            e.m_charCon.height = Mathf.Lerp(e.m_charCon.height, e.m_crouchHeight, Time.deltaTime * 8f);
            e.m_groundCheck.localPosition = e.m_crouchCheckPos;
            
            if(!e.m_crouchAccuracyApply)
            {
                e.m_currentWeapon.CrouchAccuracy(true);
            }

            e.m_crouchAccuracyApply = true;
        }
        else
        {
            float lastHeight = e.m_charCon.height;
            e.m_charCon.height = Mathf.Lerp(e.m_charCon.height, e.m_playerHeight, Time.deltaTime * 8f);
            e.m_groundCheck.localPosition = e.m_normalCheckPos;
            Vector3 tmpPosition = e.gameObject.transform.position;
            tmpPosition.y += (e.m_charCon.height - lastHeight);
            e.gameObject.transform.position = tmpPosition;

            if(e.m_crouchAccuracyApply)
            {
                e.m_currentWeapon.CrouchAccuracy(false);
            }

            e.m_crouchAccuracyApply = false;
        }

        if(!e.m_isLeanDouble)
        {
            if (e.m_isLeanQ)
            {
                //부모 오브젝트와의 로컬위치를 조절시켜서 RotateAround처럼 동작하게함.
                e.m_upperBodyLean.transform.localRotation = Quaternion.Slerp(e.m_upperBodyLean.transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, 10f)), Time.deltaTime * 7f);
            }

            if (e.m_isLeanE)
            {
                e.m_upperBodyLean.transform.localRotation = Quaternion.Slerp(e.m_upperBodyLean.transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, -10f)), Time.deltaTime * 7f);
            }
        }
        else
        {
            e.m_upperBodyLean.transform.localRotation = Quaternion.Slerp(e.m_upperBodyLean.transform.localRotation, Quaternion.Euler(new Vector3(0f, 0f, 0f)), Time.deltaTime * 7f);
        }

        //대각선방향으로 갈때 루트값으로 인해 더 빠른것을 방지하기 위함. 그냥 m_moveDir을 정규화해도되지만, 밀려나는 현상이 생겨서 이걸로 바꿈. <밀려나는 현상은 GetAxisRaw로도 해결할 수 있음>
        if (e.m_horiz != 0 && e.m_vert != 0)
        {
            e.m_diagonalSpeedModifier = 0.714f;
        }
        else
        {
            e.m_diagonalSpeedModifier = 1f;
        }

        Vector3 moveDirSide = e.transform.right * e.m_horiz * e.m_diagonalSpeedModifier;
        Vector3 moveDirForward = e.transform.forward * e.m_vert * e.m_diagonalSpeedModifier;

        e.m_moveDir = moveDirSide + moveDirForward;

        if (e.m_moveDir != Vector3.zero)
        {
            if (e.m_isWalking)
            {
                e.m_charCon.Move(e.m_moveDir * e.m_walkSpeed * Time.deltaTime);
                e.m_footstepCycle = 0.78f;
                e.m_gunMoveSpeed = 0.33f;
                e.m_footstepVolume = 0.3f;
            }
            else if (e.m_isCrouching)
            {
                e.m_charCon.Move(e.m_moveDir * e.m_crouchSpeed * Time.deltaTime);
                e.m_footstepCycle = 0.65f;
                e.m_gunMoveSpeed = 0.4f;
                e.m_footstepVolume = 0.6f;
            }
            else if(e.m_isRunning)
            {
                e.m_charCon.Move(e.m_moveDir * e.m_runSpeed * Time.deltaTime);
                e.m_footstepCycle = 0.3f;
                e.m_footstepVolume = 1f;
            }
            else
            {
                e.m_charCon.Move(e.m_moveDir * e.m_normalSpeed * Time.deltaTime);
                e.m_footstepCycle = 0.5f;
                e.m_gunMoveSpeed = 0.5f;
                e.m_footstepVolume = 0.8f;
            }

            if(!e.m_isRunning)
            {
                e.m_currentWeapon.m_anim.SetBool("ISMOVE", true);
                e.m_currentWeapon.m_anim.SetFloat("GUNMOVE_SPEED", e.m_gunMoveSpeed);
            }

            //풋스텝 타이머로 각 상태마다 사이클시간까지 시간을재서 풋스텝사운드의 재생간격을 맞추어준다.
            e.m_footstepTimer += Time.deltaTime;

            if(e.m_footstepTimer > e.m_footstepCycle)
            {
                if (e.m_isGrounded)
                {
                    if (e.m_footstepTurn == 0)
                    {
                        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.FOOTSTEP1, e.m_footstepVolume);
                        e.m_footstepTurn = 1;
                    }
                    else if(e.m_footstepTurn == 1)
                    {
                        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.FOOTSTEP2, e.m_footstepVolume);
                        e.m_footstepTurn = 0;
                    }

                    e.m_footstepTimer = 0f;
                }
            }
            //풋스텝 타이머 END
        }
        else
        {
            e.m_footstepTimer = 0f;
            e.m_footstepCycle = 0f;
            e.m_currentWeapon.m_anim.SetBool("ISMOVE", false);
        }

        #endregion

        #region Jump

        //CheckSphere(위치, 반지름, 어떤 레이어와의 충돌을 감지? -> 즉 특정 위치에 반지름을 받아 원을 생성하고 그 원내에 해당 레이어와의 충돌이 존재한다면 True를 반환함
        //고로 발밑에 m_groundCheck를 두고, 0.4f 반지름만큼 원을 생성하고 바닥레이어와 충돌 시 바닥에 닿았다는 것을 처리하게됨.
        e.m_isGrounded = Physics.CheckSphere(e.m_groundCheck.position, e.m_groundDistance, e.m_groundMask);

        e.m_PreviouslyGrounded = e.m_isGrounded;
        
        if (e.m_isGrounded && e.m_velocity.y < 0f)
        {
            e.m_velocity.y = -2f;

            if (e.m_jumpAccruacyApply)
            {
                e.m_currentWeapon.JumpAccuracy(false);
            }

            e.m_jumpAccruacyApply = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && e.m_isGrounded)
        {
            e.m_velocity.y = Mathf.Sqrt(e.m_jumpHeight * -2f * e.m_gravity);
            
            if(!e.m_jumpAccruacyApply)
            {
                e.m_currentWeapon.JumpAccuracy(true);
            }

            e.m_jumpAccruacyApply = true;

            SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.JUMP, 0.8f);
        }

        e.m_velocity.y += e.m_gravity * Time.smoothDeltaTime;

        e.m_charCon.Move(e.m_velocity * Time.smoothDeltaTime);
        #endregion
    }

    public void Exit(Player_StateManager e)
    {
        
    }
}
