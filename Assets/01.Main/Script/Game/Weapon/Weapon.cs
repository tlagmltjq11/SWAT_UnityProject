using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
	#region Field
	#region References
	// References
	public Transform m_shootPoint;
	public ParticleSystem muzzleFlash;
	public Player_StateManager m_stateManager;
	public CameraRotate m_cameraRotate;
	public GameObject m_horizonCamRecoil;
	public GameObject m_verticalCamRecoil;
	public Transform m_casingPoint;
	public Camera m_camera;
	public GameObject[] m_sights;
	public Animator m_anim;
	public GameObject m_player;
	#endregion
	#region Weapon info
	// Weapon Specification
	protected float m_range;
	protected float m_accuracy;
	protected float m_power;
	protected float m_originAccuracy;
	public string m_weaponName;
	public int m_bulletsPerMag;
	public int m_bulletsRemain;
	public int m_totalMag;
	public int m_currentBullets;
	public float m_fireRate;
	// aim 만들때 사용
	public Vector3 m_aimPosition;
	public Vector3 m_dotSightPosition;
	public Vector3 m_acogSightPosition;
	public Vector3 m_originalPosition;
	// 반동 만들때 사용
	protected Vector3 m_recoilKickBack;
	protected float m_recoilAmount;
	protected float m_recoilVert;
	protected float m_recoiltHoriz;
	#endregion
	#region State Check vars
	// 각종 상태체크
	protected AnimatorStateInfo m_info;
	protected bool m_isAimOutOver;
	public bool m_isReloading;
	public bool m_isDrawing;
	public bool m_isAiming;
	public bool m_isFiring;
	public float m_fireTimer;
    #endregion
    #endregion

    #region Abstract Methods
    public abstract void Fire();
	public abstract void StopFiring();
	public abstract void ChangeSight();
	public abstract void Reload();
	#endregion

	#region Public Methods
	public void AimIn()
	{
		m_anim.SetBool("ISAIM", true);
		m_isAiming = true;

		m_accuracy = m_accuracy / 4f;

		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(false);
		}
		SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AIM_IN, 3.5f);
	}

	public void AimOut()
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);

		if (m_stateManager.m_isCrouching)
		{
			m_accuracy = m_originAccuracy / 2f;
		}
		else if (!m_stateManager.m_isGrounded)
		{
			m_accuracy = m_originAccuracy * 5f;
		}
		else
		{
			m_accuracy = m_originAccuracy;
		}

		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(true);
		}
	}

	public void JumpAccuracy(bool j)
	{
		if (j)
		{
			m_accuracy = m_accuracy * 5f;
		}
		else
		{
			if (m_isAiming)
			{
				m_accuracy = m_originAccuracy / 4f;
			}
			else
			{
				m_accuracy = m_originAccuracy;
			}
		}
	}

	public void CrouchAccuracy(bool c)
	{
		if (c)
		{
			m_accuracy = m_accuracy / 2f;
		}
		else
		{
			if (m_isAiming)
			{
				m_accuracy = m_originAccuracy / 4f;
			}
			else
			{
				m_accuracy = m_originAccuracy;
			}
		}
	}
	#endregion

	#region Protected Methods
	protected void Recoil()
	{
		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		if (!m_isAiming)
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x), m_recoilKickBack.y, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);

			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x); //현재 이걸로 수직반동 올리는 중임.
		}
		else
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, m_recoilKickBack.z);
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, m_recoilAmount);

			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil / 1.5f), m_recoilAmount);
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x / 2f); //현재 이걸로 수직반동 올리는 중임.
		}
	}

	protected void RecoilBack()
	{
		m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3f);
	}

	protected void CasingEffect()
	{
		Quaternion randomQuaternion = new Quaternion(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f), 1);
		var casing = ObjPool.Instance.m_casingPool.Get();

		if (casing != null)
		{
			casing.transform.SetParent(m_casingPoint);
			casing.transform.localPosition = new Vector3(-1f, -3f, 0f);
			casing.transform.localScale = new Vector3(23, 23, 23);
			casing.transform.localRotation = Quaternion.identity;

			var rigid = casing.gameObject.GetComponent<Rigidbody>();

			rigid.isKinematic = false; //물리힘을 가하기 위해.
			casing.gameObject.SetActive(true);
			//매번 랜덤한 힘을 가해준다.
			rigid.AddRelativeForce(new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
			rigid.MoveRotation(randomQuaternion.normalized);
		}
	}
	#endregion
}