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
<summary>무기관련 Class 접기/펼치기</summary>
<div markdown="1">

<br>

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
	public ParticleSystem muzzleFlash; //총기화염
	public GameObject m_player;
	public Player_StateManager m_stateManager;
	public Camera m_camera;
	public CameraRotate m_cameraRotate;
	public GameObject m_horizonCamRecoil; //수평반동
	public GameObject m_verticalCamRecoil; //수직반동
	public Transform m_casingPoint; //탄피포인트
	public GameObject[] m_sights; //sight 파츠
	#endregion
	
	#region Weapon info
	// Weapon Specification
	public string m_weaponName;
	public int m_bulletsPerMag;
	public int m_bulletsRemain;
	public int m_totalMag;
	public int m_currentBullets;
	public float m_range; //사정거리
	public float m_fireRate; //연사력
	public float m_accuracy; //현재 정확도
	public float m_power;
	public float m_originAccuracy; //원래 정확도
	
	// 현재 장착한 sight 파츠에 따라 정조준의 최종 포지션이 다름.
	public Vector3 m_aimPosition;
	public Vector3 m_dotSightPosition;
	public Vector3 m_acogSightPosition;
	public Vector3 m_originalPosition;
	
	// 반동 만들때 사용
	public Vector3 m_recoilKickBack; //총기가 뒤로 밀리는 위치
	public float m_recoilAmount; //반동의 세기
	public float m_recoilVert; //수직
	public float m_recoiltHoriz; //수평
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
	public abstract void AimIn(); //정조준
	public abstract void AimOut();
	public abstract void ChangeSight(); //파츠 변경
	public abstract void Recoil(); //반동
	public abstract void RecoilBack(); //수평반동 회복
	public abstract void CasingEffect(); //탄피이펙트
	public abstract void JumpAccuracy(bool j); //점프했을때 정확도
	public abstract void CrouchAccuracy(bool c); //앉았을때 정확도
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
		if (m_fireTimer < m_fireRate) //연사력을 시간으로 구현
		{
			return;
		}

		SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AKM_SHOOT, 0.8f); //발포음 재생

		if (m_isFiring) //연사중이라면 반동을 지속해서 키워줌
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
		
		//레이캐스트 발사 == 총알
		if (Physics.Raycast(m_shootPoint.position, m_shootPoint.transform.forward + Random.onUnitSphere * m_accuracy,
		    out hit, m_range, layerMask))
		{
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) //적이 맞았을 경우
			{
				var blood = ObjPool.Instance.m_bloodPool.Get(); //블러드 이펙트를 풀에서 꺼냄

				if (blood != null)
				{
					blood.gameObject.transform.position = hit.point;
					//법선벡터를 이용해서 잘보이게 회전시킴.
					blood.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					blood.gameObject.SetActive(true);
				}

				//총을 맞은 적
				Enemy_StateManager enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

				if (enemy)
				{
					//compareTag를 이용!
					if (hit.collider.gameObject.CompareTag("HeadShot")) //헤드샷 판별
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
			else //적이 아닐 경우
			{
				var hitHole = ObjPool.Instance.m_hitHoleObjPool.Get(); //탄흔 이펙트를 풀에서 꺼냄.

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
				
				//Movalble은 리지드바디를 가진 오브젝트들의 레이어임.
				if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Movable")))
				{
					Rigidbody rig = hit.transform.GetComponent<Rigidbody>();

					if (rig)
					{
						//피격된 지점에서 물리힘을 가해줌.
						rig.AddForceAtPosition(m_shootPoint.forward * m_power * 70f, m_shootPoint.position);
					}
				}
			}
		}

		m_currentBullets--;
		m_fireTimer = 0.0f;
		m_anim.CrossFadeInFixedTime("FIRE", 0.01f); //애니메이션을 즉시 FIRE로 바꿔줌.

		muzzleFlash.Play(); //총기화염 play
		Recoil(); //반동
		CasingEffect(); //탄피 이펙트 생성
	}

	public override void StopFiring() //연사를 멈출 경우 반동 회복
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
		m_anim.CrossFadeInFixedTime("RELOAD", 0.01f); //애니메이션을 즉시 RELOAD로 바꿔줌.
	}

	public override void AimIn() //정조준
	{
		m_anim.SetBool("ISAIM", true); //IDLE 애니메이션이 재생되지 않도록 ISAIM으로 변경.
		m_isAiming = true;

		m_accuracy = m_accuracy / 4f; //정확도를 높여줌.

		if (UIManager.Instance != null)
		{
			UIManager.Instance.CrossHairOnOff(false); //크로스헤어를 비활성화 시킨다.
		}
		SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.AIM_IN, 3.5f);
	}

	public override void AimOut()
	{
		m_isAiming = false;
		m_anim.SetBool("ISAIM", false);
		
		//정확도를 다시 낮춰준다.
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

    	public override void ChangeSight() //
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

    	public override void Recoil() //반동
	{
		Vector3 HorizonCamRecoil = new Vector3(0f, Random.Range(-m_recoiltHoriz, m_recoiltHoriz), 0f);
		Vector3 VerticalCamRecoil = new Vector3(-m_recoilVert, 0f, 0f);

		if (!m_isAiming)
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x),
			                                m_recoilKickBack.y, m_recoilKickBack.z);
							
			//총기가 뒤로 밀리는 반동
			transform.localPosition = Vector3.Lerp(transform.localPosition, 
			                                       transform.localPosition + gunRecoil, m_recoilAmount);
			
			//수평반동
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation,
			Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil), m_recoilAmount);
			
			//수직반동
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x);
		}
		else
		{
			Vector3 gunRecoil = new Vector3(Random.Range(-m_recoilKickBack.x, m_recoilKickBack.x) / 2f, 0, 
			                                 m_recoilKickBack.z);
							 
			//총기가 뒤로 밀리는 반동		 
			transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + gunRecoil, 
			                                        m_recoilAmount);
			
			//수평반동
			m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation,
			Quaternion.Euler(m_horizonCamRecoil.transform.localEulerAngles + HorizonCamRecoil / 1.5f), m_recoilAmount);
			
			//수직반동
			m_cameraRotate.VerticalCamRotate(-VerticalCamRecoil.x / 2f); //현재 이걸로 수직반동 올리는 중임.
		}
	}

	public override void RecoilBack() //수평반동 -> Update문에서 호출해준다.
	{
		m_horizonCamRecoil.transform.localRotation = Quaternion.Slerp(m_horizonCamRecoil.transform.localRotation, 
		                                                              Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3f);
	}

	public override void CasingEffect() //탄피 이펙트
	{
		//매번 랜덤한 각도로 튕겨져 나감.
		Quaternion randomQuaternion = new Quaternion(Random.Range(0f, 360f), Random.Range(0f, 360f), 
		                                              Random.Range(0f, 360f), 1);
							      
		var casing = ObjPool.Instance.m_casingPool.Get(); //풀에서 탄피를 꺼냄.

		if (casing != null)
		{
			casing.transform.SetParent(m_casingPoint);
			casing.transform.localPosition = new Vector3(-1f, -3.5f, 0f);
			casing.transform.localScale = new Vector3(25, 25, 25);
			casing.transform.localRotation = Quaternion.identity;

			casing.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			casing.gameObject.SetActive(true);
			//매번 랜덤한 힘을 가해준다.
			casing.gameObject.GetComponent<Rigidbody>().AddRelativeForce(
				new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
			                                                             						     
			casing.gameObject.GetComponent<Rigidbody>().MoveRotation(randomQuaternion.normalized);
		}
	}

	public override void JumpAccuracy(bool j) //점프시에 정확도 조정
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

	public override void CrouchAccuracy(bool c) //앉았을 시 정확도 조정
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

<br>

<details>
<summary>ATW code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public abstract class ATW : MonoBehaviour //Ahead Thrown Weapon 투척무기
{
    #region Field
    public Rigidbody m_rigid;
    public GameObject m_effectObj; //폭발 이펙트
    public GameObject m_meshObj; //몸체 Mesh -> 폭발 시 비활성화
    public int m_remainNum; //남은 갯수
    public float m_timeToOper; //동작하기까지 걸리는 시간
    public string m_name;
    public float m_power;
    public float m_explosionRadius; //폭발범위
    #endregion

    #region Public Methods
    public void Starter()
    {
        Invoke("Operation", m_timeToOper);
    }
    #endregion

    #region Abstract Methods
    public abstract void Operation(); //동작
    #endregion
}
```

</div>
</details>

<details>
<summary>ATW_Grenade code 접기/펼치기</summary>
<div markdown="1">
	
```c#
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
        //파티클시스템을 작동시키기 위해서, 수류탄의 몸체를 정지시킨 후 똑바로 세워놓음.
        m_rigid.velocity = Vector3.zero;
        m_rigid.angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);

        //수류탄의 몸체를 보이지않게함.
        m_meshObj.SetActive(false);
        //수류탄 파티클시스템을 작동시킴.
        m_effectObj.SetActive(true);

        //랜덤으로 폭발음을 재생
        SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ATW_EXPLOSION1, 
			                  (int)SoundManager.eAudioClip.ATW_IMPACTONTGROUND), gameObject.transform.position, 40f, 2f);

        //대상 레이어만 지정
        int layerMask = ((1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")) | (1 << LayerMask.NameToLayer("Movable")));
        //SphereCastAll을 통해서 수류탄의 폭발지점 반경내에있는 대상 레이어의 모든 hit정보를 받아온다.
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, m_explosionRadius, Vector3.up, 0, layerMask,
						   QueryTriggerInteraction.UseGlobal);

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
                int layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactable")) |
		              (1 << LayerMask.NameToLayer("Movable"));
			      
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
	    //적군이 아닌 크래쉬박스와 같은 Movalble 오브젝트일 경우
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
	
        Destroy(gameObject, 2f);
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**<br>
공통된 내용(필드나 메소드)들을 추출하여 통일된 내용으로 작성하도록 상위 클래스인 Weapon, ATW 추상클래스를 구현했습니다. 모든 총기 및 투척무기 클래스는 해당 추상클래스를 상속받아, 
각자 필요한 메소드나 필드만 추가로 정의하고, 추상 메소드를 오버라이딩하여 클래스마다 다르게 실행될 로직을 작성해 주면 됩니다. 이러한 구성을 통해서, 코드들을 규격화 할 수 있었고 
아래와 같이 다형성 사용, 느슨한 결합 등을 이룰 수 있었습니다.

```c#
//플레이어 컨트롤 스크립트(간략화)
public Weapon m_currentWeapon; //현재 총기
public ATW m_currentATW; //현재 투척무기
...
    if (Input.GetButton("Fire1"))
    {
        m_currentWeapon.Fire(); //다형성
    }
...
    void SetCurrentWeapon(Weapon weapon) //느슨한 결합
    {
        m_currentWeapon = weapon;
    }
...
    if (Input.GetKeyDown(KeyCode.G))
    {
        if(m_currentATW.m_remainNum > 0)
        {
            scr.m_rigid.AddForce(grenade.transform.up * 5f + grenade.transform.forward * 15f, ForceMode.Impulse);
            scr.Starter(); //다형성
        }
    }
...
    void SetCurrentATW(ATW atw) //느슨한 결합
    {
        m_currentATW = atw;
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
