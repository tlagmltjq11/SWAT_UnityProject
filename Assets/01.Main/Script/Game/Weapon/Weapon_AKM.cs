using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon_AKM : Weapon
{
    #region Unity Methods
    private void OnEnable()
    {
		m_anim.CrossFadeInFixedTime("DRAW", 0.01f);
	}

    private void OnDisable()
    {
		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(true);
		}

		if (m_isAiming)
        {
			AimOut();

			transform.localPosition = m_originalPosition;
			m_camera.fieldOfView = 60f;
		}
    }

    private void Awake()
    {
		m_anim = GetComponent<Animator>();
	}

    void Start()
	{
		Init();
		m_stateManager = m_player.GetComponent<Player_StateManager>();
		m_cameraRotate = m_camera.GetComponent<CameraRotate>();
	}

    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0);
        m_isReloading = m_info.IsName("RELOAD");
		m_isDrawing = m_info.IsName("DRAW");


		if (m_isAiming)
        {
			if(m_sights[0].activeSelf)
            {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_dotSightPosition, Time.deltaTime * 8f);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 40f, Time.deltaTime * 8f);
			}
			else if(m_sights[1].activeSelf)
            {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_acogSightPosition, Time.deltaTime * 8f);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 17.5f, Time.deltaTime * 8f);
			}
			else
            {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_aimPosition, Time.deltaTime * 8f);
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 40f, Time.deltaTime * 8f);
			}
        }
        else
        {
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_originalPosition, Time.deltaTime * 6f);
            m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, 60f, Time.deltaTime * 8f);
        }

		RecoilBack();
	}
    #endregion

    #region Private Methods
    private void Init()
	{
		// Weapon Specification
		m_weaponName = "AKM";
		m_bulletsPerMag = 30;
		m_bulletsRemain = 150;
		m_currentBullets = 30;
		m_totalMag = 5;
		m_range = 100f;
		m_fireRate = 0.1f;
		m_recoilAmount = 1f;
		m_recoilVert = 1.2f;
		m_recoiltHoriz = 0.4f;
		m_recoilKickBack = new Vector3(0.1f, 0.25f, -0.5f);
		m_originAccuracy = 0.015f;
		m_power = 25f;

		m_accuracy = m_originAccuracy;
		m_originalPosition = transform.localPosition;
		m_currentBullets = m_bulletsPerMag;

		for(int i=0; i<m_sights.Length; i++)
        {
			m_sights[i].SetActive(false);
        }

		m_isFiring = false;
	}
    #endregion

    #region Abstract Methods Implement
    public override void Fire()
	{
		if (m_fireTimer < m_fireRate)
		{
			return;
		}

		SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AKM_SHOOT, 0.8f);

		if (m_isFiring)
        {
			m_recoilVert += 0.15f;
			m_recoilVert = Mathf.Clamp(m_recoilVert, 1.2f, 3f);

			m_recoiltHoriz += 0.05f;
			m_recoiltHoriz = Mathf.Clamp(m_recoiltHoriz, 0.4f, 0.8f);
        }

		RaycastHit hit;

		int layerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Sfx")) | (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Player_Throw")) | (1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
		layerMask = ~layerMask;
		
		if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward + Random.onUnitSphere * m_accuracy, out hit, m_range, layerMask))
		{
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				var blood = ObjPool.Instance.m_bloodPool.Get();

				if (blood != null)
				{
					blood.gameObject.transform.position = hit.point;
					blood.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					blood.gameObject.SetActive(true);
				}

				Enemy_StateManager enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

				if (enemy)
				{
					if (hit.collider.gameObject.CompareTag("HeadShot"))
					{
						SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HEADSHOT, 1.5f);
						enemy.Damaged(m_power * 100f);
					}
					else
					{
						SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HITSOUND, 1.5f);
						enemy.Damaged(m_power);
					}
				}
			}
			else
			{
				var hitHole = ObjPool.Instance.m_hitHoleObjPool.Get();

				if (hitHole != null)
				{
					hitHole.gameObject.transform.position = hit.point;
					hitHole.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					hitHole.transform.SetParent(hit.transform); // 탄흔이 오브젝트를 따라가게끔 유도하기 위해 리턴되기 전까지만 부모로 지정
					hitHole.gameObject.SetActive(true);
				}

				var hitSpark = ObjPool.Instance.m_hitSparkPool.Get();

				if (hitSpark != null)
				{
					hitSpark.gameObject.transform.position = hit.point;
					hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					hitSpark.gameObject.SetActive(true);
				}

				if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable")))
				{
					Rigidbody rig = hit.transform.GetComponent<Rigidbody>();

					if (rig)
					{
						rig.AddForceAtPosition(m_shootPoint.forward * m_power * 70f, m_shootPoint.position);
					}
				}
			}
		}

		m_currentBullets--;
		m_fireTimer = 0.0f;
		m_anim.CrossFadeInFixedTime("FIRE", 0.01f);

		muzzleFlash.Play();
		Recoil();
		CasingEffect();
	}

	public override void StopFiring()
	{
		m_recoilVert = 1.2f;
		m_recoiltHoriz = 0.65f;
	}

    public override void Reload()
    {
		if (m_currentBullets == m_bulletsPerMag || m_bulletsRemain == 0)
		{
			return;
		}

		SoundManager.Instance.Play2DSound_Play((int)SoundManager.eAudioClip.AKM_RELOAD, 1f);
		m_anim.CrossFadeInFixedTime("RELOAD", 0.01f);
	}

    public override void ChangeSight()
    {
		bool check = false;
		int index = 0;

		for(int i=0; i<m_sights.Length; i++)
        {
			if(m_sights[i].activeSelf)
            {
				check = true;
				index = i;
				break;
            }
        }
		
		if(check)
        {
			m_sights[index].SetActive(false);
			
			if(index + 1 < m_sights.Length)
            {
				m_sights[index + 1].SetActive(true);
			}

        }
		else
        {
			m_sights[index].SetActive(true);
        }
    }
    #endregion

    #region Public Methods
    public void ReloadComplete()
    {
		int temp = 0;

		temp = m_bulletsPerMag - m_currentBullets; //장전되어야 할 총알의 수

		if(temp >= m_bulletsRemain)
        {
			m_currentBullets += m_bulletsRemain;
			m_bulletsRemain = 0;
        }
		else
        {
			m_currentBullets += temp;
			m_bulletsRemain -= temp;
        }

		UIManager.Instance.Update_Bullet(m_currentBullets, m_bulletsRemain);
	}
	#endregion
}