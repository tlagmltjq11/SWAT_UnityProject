using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Feild
    CharacterController m_charCon;
    Vector3 m_moveDir = Vector3.zero;
    Animator m_animator;
    public float m_runSpeed = 3f;
    public float m_walkSpeed = 1.5f;
    public float m_crouchSpeed = 1.5f;
    float m_horiz = 0f;
    float m_vert = 0f;
    float m_diagonalSpeedModifier;

    [SerializeField]
    Transform m_groundCheck;
    Vector3 m_velocity;
    float m_groundDistance = 0.2f;
    float m_gravity = -13f;
    float m_jumpHeight = 1.2f;
    public LayerMask m_groundMask;

    bool m_isGrounded;
    bool m_isCrouching;
    bool m_isWalking;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_charCon = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_isGrounded = false;
        m_isCrouching = false;
        m_isWalking = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
    }

    void Movement()
    {
        //앞이나 뒤로가는 키가 눌렸으면 1 또는 -1일것임. 이 값을 현재 플레이어의 forward에 곱해준 값과 right도 마찬가지로 왼쪽 오른쪽 값을 곱해줘서 두 벡터3를 합해주면
        //어느 방향으로 나아가야할지 정해지게 되는 것.
        m_horiz = Input.GetAxis("Horizontal");
        m_vert = Input.GetAxis("Vertical");
        m_isWalking = Input.GetKey(KeyCode.LeftShift);
        m_isCrouching = Input.GetKey(KeyCode.LeftControl);

        if(m_isCrouching)
        {
            m_animator.SetBool("ISCROUCH", true);
            //높이낮춰줘야함.************
        }
        else //서있는 상태
        {
            m_animator.SetBool("ISCROUCH", false);
        }

        //대각선방향으로 갈때 루트값으로 인해 더 빠른것을 방지하기 위함. 그냥 m_moveDir을 정규화해도되지만, 밀려나는 현상이 생겨서 이걸로 바꿈. <밀려나는 현상은 GetAxisRaw로도 해결할 수 있음>
        if (m_horiz != 0 && m_vert != 0)
        {
            m_diagonalSpeedModifier = 0.714f;
        }
        else
        {
            m_diagonalSpeedModifier = 1f;
        }

        Vector3 moveDirSide = transform.right * m_horiz * m_diagonalSpeedModifier;
        Vector3 moveDirForward = transform.forward * m_vert * m_diagonalSpeedModifier;

        m_moveDir = moveDirSide + moveDirForward;

        if (m_moveDir != Vector3.zero)
        {
            if(m_isWalking)
            {
                //m_animator.SetBool("ISWALK", true);
                m_charCon.Move(m_moveDir * m_walkSpeed * Time.deltaTime);
            }
            else if(m_isCrouching)
            {
                //m_animator.SetBool("ISCROUCHMOVE", true);
                m_charCon.Move(m_moveDir * m_crouchSpeed * Time.deltaTime);
            }
            else
            {
                m_animator.SetBool("ISRUN", true);
                m_charCon.Move(m_moveDir * m_runSpeed * Time.deltaTime);
            }
        }
        else
        {
            m_animator.SetBool("ISRUN", false);
        }
    }

    void Jump()
    {
        //CheckSphere(위치, 반지름, 어떤 레이어와의 충돌을 감지? -> 즉 특정 위치에 반지름을 받아 원을 생성하고 그 원내에 해당 레이어와의 충돌이 존재한다면 True를 반환함
        //고로 발밑에 m_groundCheck를 두고, 0.4f 반지름만큼 원을 생성하고 바닥레이어와 충돌 시 바닥에 닿았다는 것을 처리하게됨.
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_groundDistance, m_groundMask);

        if(!m_isGrounded && m_velocity.y < 0f)
        {
            m_animator.SetInteger("JUMPSTATE", 2);
        }

        if (m_isGrounded && m_velocity.y < 0f)
        {
            m_velocity.y = -2f;
            m_animator.SetInteger("JUMPSTATE", 0);
        }

        if(Input.GetKeyDown(KeyCode.Space) && m_isGrounded)
        {
            m_velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
            m_animator.SetTrigger("ISJUMP");
            m_animator.SetInteger("JUMPSTATE", 1);
            Invoke("fall", 0.533f);
        }

        m_velocity.y += m_gravity * Time.deltaTime;

        m_charCon.Move(m_velocity * Time.deltaTime);
    }

    void fall()
    {
        m_animator.SetTrigger("ISJUMP");
        m_animator.SetInteger("JUMPSTATE", 2);
    }
}
