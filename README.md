# V_Project
프로젝트 설명은 아래 [링크](#1)를 통해 영상으로 확인할 수 있고, 코드와 같은 부가설명은 [About Dev](#2) 부분을 참고해주세요.<br>
<br>

### About Project.:two_men_holding_hands:
Irrational Games에서 개발한 택티컬 슈팅 게임 SWAT4를 모작한 프로젝트입니다.<br>
<br>

### Video.:video_camera: <div id="1">이미지를 클릭해주세요.</div>
[![시연영상](https://img.youtube.com/vi/TNQ0OKnjaWw/0.jpg)](https://www.youtube.com/watch?v=TNQ0OKnjaWw)
<br>
<br>

### About Dev.:nut_and_bolt: <div id="2"></div>
<details>
<summary>총기와 투척무기 Class 접기/펼치기</summary>
<div markdown="1">


<details>
<summary>Weapon code 접기/펼치기</summary>
<div markdown="1">
	
```c#
//추상클래스 Weapon
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
```
</div>
</details>

<details>
<summary>Weapon_AKM code 접기/펼치기</summary>
<div markdown="1">
	
```c#
//AKM 
public class Weapon_AKM : Weapon
{
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

		int layerMask = ((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Sfx")) | 
		(1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Player_Throw")) | 
		(1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
		
		layerMask = ~layerMask;
		
		if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward + Random.onUnitSphere * m_accuracy,
		    out hit, m_range, layerMask))
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
					// 탄흔이 오브젝트를 따라가게끔 유도하기 위해 리턴되기 전까지만 부모로 지정
					hitHole.transform.SetParent(hit.transform);
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

	public override void AimIn()
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

	public override void AimOut()
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);

		if(m_stateManager.m_isCrouching)
        	{
			m_accuracy = m_originAccuracy / 2f;
        	}
		else if(!m_stateManager.m_isGrounded)
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

    	public override void Recoil()
	{
		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		if (!m_isAiming)
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x),
			                                m_recoilKickBack.y, m_recoilKickBack.z);
							
			transform.localPosition = Vector3.Lerp(transform.localPosition, 
			                                       transform.localPosition + gunRecoil, m_recoilAmount);

			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation,
			Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x); //현재 이걸로 수직반동 올리는 중임.
		}
		else
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, 
			                                 m_recoilKickBack.z);
							 
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, 
			                                        m_recoilAmount);

			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation,
			Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil / 1.5f), m_recoilAmount);
			
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x / 2f); //현재 이걸로 수직반동 올리는 중임.
		}
	}

	public override void RecoilBack()
	{
		m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, 
		                                                              Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3f);
	}

	public override void CasingEffect()
	{
		Quaternion randomQuaternion = new Quaternion(Random.Range(0f, 360f), Random.Range(0f, 360f), 
		                                              Random.Range(0f, 360f), 1);
							      
		var casing = ObjPool.Instance.m_casingPool.Get();

		if (casing != null)
		{
			casing.transform.SetParent(m_casingPoint);
			casing.transform.localPosition = new Vector3(-1f, -3.5f, 0f);
			casing.transform.localScale = new Vector3(25, 25, 25);
			casing.transform.localRotation = Quaternion.identity;

			casing.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			casing.gameObject.SetActive(true);
			casing.gameObject.GetComponent<Rigidbody>().AddRelativeForce(
				new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
			                                                             						     
			casing.gameObject.GetComponent<Rigidbody>().MoveRotation(randomQuaternion.normalized);
		}
	}

	public override void JumpAccuracy(bool j)
    	{
		if(j)
        	{
			m_accuracy = m_accuracy * 5f;
        	}
		else
        	{
			if(m_isAiming)
            		{
				m_accuracy = m_originAccuracy / 4f;
            		}
			else
            		{
				m_accuracy = m_originAccuracy;
            		}
        	}
    	}

	public override void CrouchAccuracy(bool c)
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
}
    #endregion
```

</div>
</details>

공통된 내용(필드나 메소드)들을 추출하여 통일된 내용으로 작성하도록 상위 클래스인 Weapon 추상클래스를 구성했습니다.
모든 총기 클래스는 해당 Weapon 클래스를 상속받아, 각자 필요한 메소드나 필드만 추가로 정의하고, 추상 메소드를 오버라이딩하여 클래스마다 다르게 실행될 로직을 작성해 주면 됩니다.
이러한 구성을 통해서, 코드들을 규격화 할 수 있었고 아래와 같이 다형성 사용, 느슨한 결합 등을 이룰 수 있었습니다.

```c#
//플레이어 컨트롤 스크립트
public Weapon m_currentWeapon; //현재 무기
...
    if (Input.GetButton("Fire1"))
    {
        m_currentWeapon.Fire();
    }
...
    void SetCurrentWeapon(Weapon weapon)
    {
        m_currentWeapon = weapon;
    }
...
```
</div>
</details>

<br>


### Difficult Point.:sweat_smile:
* 
<br>

### Feeling.:pencil:

<br>



메인화면 이미지 출처 https://1freewallpapers.com/point-blank-swat-game/ko
