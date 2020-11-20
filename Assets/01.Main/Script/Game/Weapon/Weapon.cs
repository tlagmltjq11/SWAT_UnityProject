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
	public Animator m_anim;
	public ParticleSystem muzzleFlash;
	public GameObject m_player;
	public Player_StateManager m_stateManager;
	public Camera m_camera;
	public CameraRotate m_cameraRotate;
	public GameObject m_horizonCamRecoil;
	public GameObject m_verticalCamRecoil;
	public Transform m_casingPoint;
	public GameObject[] m_sights;
	#endregion
	#region Weapon info
	// Weapon Specification
	public string m_weaponName;
	public int m_bulletsPerMag;
	public int m_bulletsRemain;
	public int m_totalMag;
	public int m_currentBullets;
	public float m_range;
	public float m_fireRate;
	public float m_accuracy;
	public float m_power;
	public float m_originAccuracy;
	// aim 만들때 사용
	public Vector3 m_aimPosition;
	public Vector3 m_dotSightPosition;
	public Vector3 m_acogSightPosition;
	public Vector3 m_originalPosition;
	// 반동 만들때 사용
	public Vector3 m_recoilKickBack;
	public float m_recoilAmount;
	public float m_recoilVert;
	public float m_recoiltHoriz;
    #endregion
    #region State Check vars
    // 각종 상태체크
    public AnimatorStateInfo m_info;
	public bool m_isReloading;
	public bool m_isDrawing;
	public bool m_isAiming;
	public bool m_isAimOutOver;
	public bool m_isFiring;

	public float m_fireTimer;
    #endregion
    #endregion

    #region Abstract Methods
    public abstract void Fire();
	public abstract void StopFiring();
	public abstract void Reload();
	public abstract void AimIn();
	public abstract void AimOut();
	public abstract void ChangeSight();
	public abstract void Recoil();
	public abstract void RecoilBack();
	public abstract void CasingEffect();
	public abstract void JumpAccuracy(bool j);
	public abstract void CrouchAccuracy(bool c);
    #endregion
}