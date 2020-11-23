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
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        m_meshObj.SetActive(false);
        m_effectObj.SetActive(true);
        SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ATW_EXPLOSION1, (int)SoundManager.eAudioClip.ATW_IMPACTONTGROUND), gameObject.transform.position, 40f, 2f);

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")) | (1 << LayerMask.NameToLayer("Movable")));
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, m_explosionRadius, Vector3.up, 0, layerMask, QueryTriggerInteraction.UseGlobal);

        foreach(RaycastHit hit in hits)
        {
            var distance = Vector3.Distance(hit.transform.position, gameObject.transform.position);
            if(distance == 0)
            {
                distance = 1;
            }

            if(hit.transform.root.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
            {
                RaycastHit check;
                var dir = hit.transform.position - gameObject.transform.position;
                int layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Movable"));
                if(Physics.Raycast(gameObject.transform.position, dir.normalized, out check, m_explosionRadius, layer))
                {
                    Debug.Log(check.transform.name);
                    continue;
                }

                var enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

                enemy.Damaged(100 * (m_power / distance));
                Debug.Log(100 * (m_power / distance));

                if (enemy.m_hp <= 0) //대상이 죽은 상태라면
                {
                    var rigs = enemy.transform.root.gameObject.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rig in rigs)
                    {
                        rig.AddExplosionForce(500 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 8f);
                    }
                }

            }
            else if(hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable")))
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

        Destroy(gameObject, 2f); //파티클시스템의 모든 듀레이션이 2임
    }
    #endregion
}
