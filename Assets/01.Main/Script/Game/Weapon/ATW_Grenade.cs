using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATW_Grenade : ATW
{
    #region Unity Methods
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_remainNum = 2;
        m_timeToOper = 3f;
        m_name = "Grenade";
        m_power = 6.5f;
        m_explosionRadius = 5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.ATW_IMPACTONTGROUND, gameObject.transform.position, 20f, 1.5f);
        }
    }
    #endregion

    #region Abstract Methods Implement
    public override void Operation()
    {
        StartCoroutine("Operator");
    }
    #endregion

    #region Coroutine
    IEnumerator Operator()
    {
        yield return new WaitForSeconds(m_timeToOper);

        //파티클시스템을 작동시키기 위해서, 수류탄의 몸체를 정지시킨 후 똑바로 세워놓음.
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);

        //수류탄의 몸체를 보이지않게함.
        m_meshObj.SetActive(false);
        //수류탄 파티클시스템을 작동시킴.
        m_effectObj.SetActive(true);

        //랜덤으로 폭발음을 재생
        SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ATW_EXPLOSION1, (int)SoundManager.eAudioClip.ATW_IMPACTONTGROUND), gameObject.transform.position, 40f, 2f);

        //대상 레이어만 지정
        int layerMask = ((1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")) | (1 << LayerMask.NameToLayer("Movable")));
        //SphereCastAll을 통해서 수류탄의 폭발지점 반경내에있는 대상 레이어의 모든 hit정보를 받아온다.
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, m_explosionRadius, Vector3.up, 0, layerMask, QueryTriggerInteraction.UseGlobal);

        foreach(RaycastHit hit in hits)
        {
            //수류탄과의 거리별 데미지를 계산한다.
            
            //혹시 거리가 0이 나오는 경우를 대비
            var distance = Vector3.Distance(hit.transform.position, gameObject.transform.position);
            if(distance == 0)
            {
                distance = 1;
            }

            //대상이 적군일 경우
            if(hit.transform.root.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
            {
                //적군과 수류탄사이에 장애물이 가로막고있는지 Raycast로 판별 후, 가로막혀있다면 데미지를 주지않는다.
                RaycastHit check;
                var dir = hit.transform.position - gameObject.transform.position;
                int layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Movable"));
                if(Physics.Raycast(gameObject.transform.position, dir.normalized, out check, m_explosionRadius, layer))
                {
                    continue;
                }

                var enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

                //사이에 장애물이 없다면, 거리별 데미지를 준다.
                enemy.Damaged(100 * (m_power / distance));

                if (enemy.m_hp <= 0) //대상이 죽은 상태라면
                {
                    //대상 적군의 리지드바디를 받아온다.
                    var rigs = enemy.transform.root.gameObject.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rig in rigs)
                    {
                        //물리힘을 가하여 폭발에 날아가는 효과를 준다.
                        rig.AddExplosionForce(500 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 8f);
                    }
                }

            }
            else if(hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable"))) //적군이 아닌 크래쉬박스와 같은 Movalble 오브젝트일 경우
            {
                Crash_Box cb = hit.transform.gameObject.GetComponent<Crash_Box>();
                cb.Crash();

                var rigs = hit.transform.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rig in rigs)
                {
                    rig.AddExplosionForce(100 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 5f);
                }
            }
        }

        //삭제시킨다.
        Destroy(gameObject, 2f);
    }
    #endregion
}
