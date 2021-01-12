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
<summary>무기관련 Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Weapon code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public abstract class Weapon : MonoBehaviour
{
    	#region Field
    
    	#region References
    	// References
    	public Transform m_shootPoint; //레이(총알) 발사 지점
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
	public int m_bulletsPerMag; //탄창 당 탄약
	public int m_bulletsRemain; //남은 전체 탄약
	public int m_totalMag; //총 탄창
	public int m_currentBullets; //현재 탄약
	public float m_range; //사정거리
	public float m_fireRate; //연사력
	public float m_accuracy; //현재 정확도
	public float m_power; //데미지
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
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Weapon_AKM code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public class Weapon_AKM : Weapon
{
	#region Abstract Methods Implement
    	public override void Fire() //총 발사
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
					blood.gameObject.transform.position = hit.point; //hit 포인트로 이동
					//법선벡터를 이용해서 잘보이게 회전시킴.
					blood.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					blood.gameObject.SetActive(true); //활성화 시켜서 재생시킨다.
				}

				//총을 맞은 적
				Enemy_StateManager enemy = hit.transform.GetComponentInParent<Enemy_StateManager>();

				if (enemy)
				{
					//compareTag를 이용!
					if (hit.collider.gameObject.CompareTag("HeadShot")) //헤드샷 판별
					{
						//헤드샷 사운드 재생
						SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.HEADSHOT, 1.5f);
						enemy.Damaged(m_power * 100f); //바로 죽이기 위해 x 100
					}
					else
					{
						//Hit 사운드 재생
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
					//법선벡터를 이용해서 잘보이게 회전시킴.
					hitHole.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					// 탄흔이 오브젝트를 따라가게끔 유도하기 위해 리턴되기 전까지만 부모로 지정
					hitHole.transform.SetParent(hit.transform);
					hitHole.gameObject.SetActive(true);
				}

				var hitSpark = ObjPool.Instance.m_hitSparkPool.Get(); //사격으로 인한 스파크이펙트를 풀에서 꺼냄.

				if (hitSpark != null)
				{
					hitSpark.gameObject.transform.position = hit.point;
					//법선벡터를 이용해서 잘보이게 회전시킴.
					hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					hitSpark.gameObject.SetActive(true); //활성화시켜 재생시킴.
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

		m_currentBullets--; //탄약 --
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

	public override void Reload() //재장전
	{
		if (m_currentBullets == m_bulletsPerMag || m_bulletsRemain == 0) //탄창이 꽉차있거나, 남은 탄약이 없다면 return
		{
			return;
		}

		SoundManager.Instance.Play2DSound_Play((int)SoundManager.eAudioClip.AKM_RELOAD, 1f);
		m_anim.CrossFadeInFixedTime("RELOAD", 0.01f); //애니메이션을 즉시 RELOAD로 바꿔줌.
		//-> UI와 실제 남은 탄약을 바꿔주는 부분은 애니메이션 Event로 ReloadComplete() 메소드를 호출시켜줌.
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

	public override void AimOut() //정조준 해제
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
			UIManager.Instance.CrossHairOnOff(true); //크로스헤어 활성화
		}
	}

    	public override void ChangeSight() //sight 파츠를 변경.
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

			var rigid = casing.gameObject.GetComponent<Rigidbody>();
			
			rigid.isKinematic = false; //물리힘을 가하기 위해.
			casing.gameObject.SetActive(true);
			//매번 랜덤한 힘을 가해준다.
			rigid.AddRelativeForce(
				new Vector3(Random.Range(50f, 100f), Random.Range(50f, 100f), Random.Range(-10f, 20f)));
			                                                             						     
			rigid.MoveRotation(randomQuaternion.normalized);
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
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ATW code 접기/펼치기</summary>
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
    public float m_power; //데미지
    public float m_explosionRadius; //폭발범위
    #endregion

    #region Public Methods
    public void Starter() //스타터
    {
        Invoke("Operation", m_timeToOper);
    }
    #endregion

    #region Abstract Methods
    public abstract void Operation(); //실제 동작
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ATW_Grenade code 접기/펼치기</summary>
<div markdown="1">
	
```c#
public class ATW_Grenade : ATW
{
    #region Unity Methods
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_remainNum = 2; //디폴트갯수 2개
        m_timeToOper = 3f; //3초뒤 폭발
        m_name = "Grenade";
        m_power = 6.5f;
        m_explosionRadius = 5f; //폭발 범위 반지름
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground"))) //땅이나 벽에 부딪힐 경우
        {	
	    //ImpactGround 클립 재생
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.ATW_IMPACTONTGROUND, gameObject.transform.position, 20f, 1.5f);
        }
    }
    #endregion

    #region Abstract Methods Implement
    public override void Operation() //실제 동작
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

	//모든 hit 검사
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
		//방향
                var dir = hit.transform.position - gameObject.transform.position;
                int layer = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Interactable")) |
		              (1 << LayerMask.NameToLayer("Movable"));
			      
		//적과 수류탄사이에 다른 무엇인가 존재한다면 장애물로 판단하고 데미지를 주지 않은채 다음 hit 검사로 넘어간다. 
                if(Physics.Raycast(gameObject.transform.position, dir.normalized, out check, m_explosionRadius, layer))
                {
                    continue;
                }

                var enemy = hit.transform.GetComponentInParent<Enemy_StateManager>(); //데미지를 주기위해 적 FSM매니저를 가져옴.

                //사이에 장애물이 없으므로 거리별 데미지를 준다.
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
                cb.Crash(); //박스가 여러 조각의 오브젝트들로 대체되는 메소드 -> 부서지는 현상을 보여주는것

                var rigs = hit.transform.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rig in rigs)
                {
		    //각 박스조각 오브젝트들에게 물리힘을 가해 날려준다.
                    rig.AddExplosionForce(100 * (m_power / distance), gameObject.transform.position, m_explosionRadius, 5f);
                }
            }
        }
	
	//2초뒤 삭제
        Destroy(gameObject, 2f);
    }
    #endregion
}
```

</div>
</details>

<br>

**Explanation**:gun:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
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


<details>
<summary>FSM Code 접기/펼치기</summary>
<div markdown="1">
	
<br>	
	
<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;적 FSM Code 접기/펼치기</summary>
<div markdown="1">

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM Class 접기/펼치기</summary>
<div markdown="1">

```c#
public class FSM <T>  : MonoBehaviour //상태 매니저는 해당 FSM 클래스를 상속받아서 모든 기능들을 이용함.
{
	private T m_owner; //상태 매니저를 나타냄.
	private IFSMState<T> m_currentState = null; //현재 상태
	private IFSMState<T> m_previousState = null; //이전 상태

	public IFSMState<T> CurrentState{ get {return m_currentState;} } //현재 상태 프로퍼티
	public IFSMState<T> PreviousState{ get {return m_previousState;} } //이전 상태 프로퍼티

	//	초기 상태 설정..
	protected void InitState(T owner, IFSMState<T> initialState)
	{
		this.m_owner = owner;
		ChangeState(initialState);
	}

	//	각 상태의 Execute 처리..
	protected void  FSMUpdate() 
	{ 
		if (m_currentState != null)
		{
			m_currentState.Execute(owner);
		}
	}

	//	상태 변경..
	public void  ChangeState(IFSMState<T> newState)
	{
		m_previousState = m_currentState;
 
		if (m_currentState != null)
		{
			m_currentState.Exit(owner); //상태전환 이전에 Exit 호출
		}
 
		m_currentState = newState;
 
		if (m_currentState != null)
		{
			m_currentState.Enter(owner); //상태전환과 동시에 Enter 호출
		}
	}

	//	이전 상태로 전환..
	public void  RevertState()
	{
		if (m_previousState != null)
		{
			ChangeState(m_previousState);
		}
	}

	public override string ToString() 
	{ 
		return m_currentState.ToString(); //현재상태 string 반환
	}
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IFSM Interface 접기/펼치기</summary>
<div markdown="1">

```c#
public interface IFSMState<T> //각 상태들이 포함해야할 메소드를 정의한 인터페이스
{	
    //  상태 진입..
    void Enter(T e);

    //  상태 진행..
    void Execute(T e);

    //  상태 종료..
    void Exit(T e);
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Manager 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_StateManager : FSM<Enemy_StateManager> //상태매니저
{
    #region Field
    public Animator m_anim;
    public GameObject m_player; //플레이어
    public NavMeshAgent m_navAgent;
    public GameObject m_shootPoint; //레이(총알) 발사 위치
    public LineRenderer m_lineRenderer; //총알의 발사 경로를 그려줄 라인렌더러
    public AnimatorStateInfo m_info; //애니메이터 상태 정보
    public ParticleSystem muzzleFlash; //총구화염 이펙트
    public Rigidbody m_bodyRig;

    public float m_idleTime; //Idle 필수 지속시간
    public float m_dieTime; //적이 죽고 난 후 5초 뒤 비활성화해주기 위한 변수
    public float m_detectSight = 30f; //플레이어 감지거리
    public float m_attackSight = 15f; //공격가능거리
    public int m_check; //플레이어를 발견했는지 검사.
    public float m_footstepTimer; //발소리 m_footstepCycle 마다 발소리를 재생시키기 위해 시간초를 카운팅하는 변수.
    public float m_footstepCycle; //발소리 재생 간격
    public float m_hp; //체력
    public float m_bullets; //탄약
    public int m_footstepTurn = 0; //왼쪽 발소리, 오른쪽 발소리 차례대로 한번씩 재생시키기 위한 변수.

    public Transform m_wayPointObj; //웨이포인트를 자식으로 갖고있는 상위오브젝트
    public Transform[] m_wayPoints; //웨이포인트들.
    public bool m_isStayPos; //True라면 현재 포지션을 지키는 AI, False라면 Patrol하는 AI
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        Init(); //초기화
        RagdollOnOff(true); //살아있는 상태에는 isKinematic을 true로 해줌. -> false로 할 시 RayCast에 감지되지않는 문제가 생김.
        InitState(this, Enemy_IDLE.Instance); //상태를 Idle로 초기화
    }

    void OnDisable()
    {
        RagdollOnOff(true);
    }

    void Start()
    {
        if(m_wayPointObj != null)
        {
            m_wayPoints = m_wayPointObj.GetComponentsInChildren<Transform>(); //웨이포인트들을 가져온다.
        }
    }

    void Update()
    {
        m_info = m_anim.GetCurrentAnimatorStateInfo(0); //현재 동작중인 애니메이션 정보를 받아옴.

        FSMUpdate(); //현재 상태의 Execute() 실행
    }
    #endregion

    #region Private Methods
    void Init() //초기화 메소드
    {
        m_anim = GetComponent<Animator>();
        m_navAgent = GetComponent<NavMeshAgent>();

	//라인렌더러 설정
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.startWidth = 0.01f;
        m_lineRenderer.endWidth = 0.01f;
        m_lineRenderer.startColor = Color.white;
        m_lineRenderer.endColor = Color.white;
        m_lineRenderer.enabled = false;
        
        m_idleTime = 0f;
        m_dieTime = 0f;
        m_check = 0;
        m_hp = 100f;
        m_bullets = 5f;

        m_player = GameObject.Find("Player");
        m_anim.Rebind();
    }
    #endregion

    #region Public Methods
    public bool SearchTarget() //플레이어를 감지하는 메소드
    {
        var dir = (m_player.transform.position + Vector3.up * 0.4f) - (gameObject.transform.position + Vector3.up * 1.3f); //눈높이를 맞춰서 방향을 구함.

        m_check = 0;

        RaycastHit m_hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | 
									(1 << LayerMask.NameToLayer("Interactable")));
        layerMask = ~layerMask; //위 레이어들을 제외한 나머지
	
	//동일선상 눈높이에서 플레이어의 방향으로 감지거리만큼 Ray를 발사.
        if (Physics.Raycast(gameObject.transform.position + Vector3.up * 1.3f, dir.normalized, out m_hit, m_detectSight, layerMask))
        {
            if (m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_check++;
            }
        }

        if (m_check == 1)
        {
            return true; //발견
        }
        else
        {
            return false; //미발견
        }
    }

    public bool canAttack() //공격 가능 거리인지 판단
    {
        var distance = Vector3.Distance(gameObject.transform.position, m_player.transform.position); //플레이어와의 거리를 계산

        if (distance <= m_attackSight) //공격 가능 거리 일 경우
        {
            return true;
        }

        return false; //아닐 경우
    }

    public void Fire() //Ray(총알) 발사
    {
        RaycastHit hit;

        int layerMask = ((1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Sfx")) | 
				(1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Enemy_ExplosionHitCol")));
				
        layerMask = ~layerMask; //위 레이어를 제외한 나머지 레이어

	//방향을 구함.
        Vector3 dir = new Vector3(m_player.transform.position.x, m_player.transform.position.y + 0.2f, m_player.transform.position.z) - m_shootPoint.transform.position;
        m_shootPoint.transform.forward = dir.normalized; //총구의 방향을 플레이어의 위치가있는 방향으로 돌려줌.

	// Random.onUnitSphere를 이용해서 정확도를 분산시켜주며, 총알 발사.
        if (Physics.Raycast(m_shootPoint.transform.position, m_shootPoint.transform.forward + Random.onUnitSphere * 0.05f, out hit, 100f, layerMask))
        {
            m_lineRenderer.SetPosition(0, m_shootPoint.transform.position);
            m_lineRenderer.SetPosition(1, hit.point); //라인렌더러로 BulletLine을 그려줌.
            StartCoroutine(ShowBulletLine()); //해당 라인을 몇초간 지속 후 지워주기 위함.

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) //플레이어가 맞았을 경우
            {
                if(hit.collider.gameObject.name.Equals("Player")) //몸에 맞았을 경우 5데미지
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(5f);
                    }
                }
                else if(hit.collider.gameObject.name.Equals("UpperBodyLean")) //상체에 맞았을 경우 10데미지
                {
                    Player_StateManager player = hit.collider.gameObject.GetComponentInParent<Player_StateManager>();

                    if (player != null)
                    {
                        player.Damaged(10f);
                    }
                }
            }
            else //맞은 대상이 플레이어가 아닐 경우
            {
                var hitSpark = ObjPool.Instance.m_hitSparkPool.Get(); //총알 스파크이펙트를 풀에서 꺼냄.

                if (hitSpark != null)
                {
                    hitSpark.gameObject.transform.position = hit.point; //힛 포인트에 위치시킴.
                    hitSpark.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); //법선벡터를 이용해 잘보이게함.
                    hitSpark.gameObject.SetActive(true); //파티클시스템 재생
                }
            }
        }

	//발포음 재생
        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_SHOOT, gameObject.transform.position, 30f, 0.7f);
        muzzleFlash.Play(); //총구화염 재생
        m_bullets--; //탄약--

        if(m_bullets == 0) //현재 탄약이 다 떨어지면 재장전
        {
            m_anim.SetBool("ISATTACK", false);
            m_anim.CrossFadeInFixedTime("RELOAD", 0.01f); //즉시 재장전 애니메이션 작동
            SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.M4_RELOAD, gameObject.transform.position, 20f, 0.5f); //재장정 클립 재생
            m_bullets = 5f; 
        }
    }

    public void Damaged(float dmg) //적이 데미지를 입었을 경우
    {
        if (m_hp <= 0)
        {
            return;
        }

	//데미지로인해 죽을 경우
        if (m_hp - dmg <= 0f)
        {
            m_hp = 0f;
	    //죽을때 사운드클립을 랜덤하게 선택해 재생.
            SoundManager.Instance.Play3DSound(Random.Range((int)SoundManager.eAudioClip.ENEMY_DEATH1, (int)SoundManager.eAudioClip.HITSOUND), gameObject.transform.position, 20f, 1.2f);
            ChangeState(Enemy_DIE.Instance); //현재상태를 Die로 변경.
        }
        else 
        {
            m_hp = m_hp - dmg; //데미지만큼 체력 차감.
        }
    }

    public void RagdollOnOff(bool OnOff) //래그돌로인한 Rigidbody들의 isKinematic을 OnOff하는 메소드
    {
        m_anim.enabled = OnOff; //죽을경우 애니메이터도 비활성화 시켜줘야 래그돌로인해 자연스럽게 죽게됨.
        Rigidbody[] rigs = gameObject.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = OnOff;
        }
    }
    #endregion

    #region Coroutine
    IEnumerator ShowBulletLine()
    {
        m_lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); //0.1초 후 라인을 지우기 위해 비활성화
        m_lineRenderer.enabled = false;
    }
    #endregion
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Idle 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_IDLE : FSMSingleton<Enemy_IDLE>, IFSMState<Enemy_StateManager> //IDLE상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_anim.Rebind(); //애니메이터 초기화
        e.m_navAgent.ResetPath(); //navAgent path 초기화
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        e.m_idleTime += Time.deltaTime; //IDLE 필수 지속시간을위한 체크

        if (e.m_idleTime >= 1.5f) //필수 지속시간을 넘어섰다면, 상태변경 가능.
        {
            if (e.SearchTarget()) //플레이어를 감지했을 경우
            {
                if (e.canAttack()) //공격 가능 거리내에 플레이어가 있을 경우
                {
                    e.ChangeState(Enemy_ATTACK.Instance); //공격 상태로 변경
                }
                else if(!e.m_isStayPos) //공격 가능 거리내에 플레이어가 없고, 자리를 지키는 AI가 아니라면
                {
                    e.ChangeState(Enemy_RUN.Instance); //질주 상태로 변경
                }
            }
            else if(!e.m_isStayPos) //플레이어를 찾지 못했고, 자리를 지키는 AI가 아니라면
            {
                e.ChangeState(Enemy_PATROL.Instance); //패트롤로 상태 변경
            }
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_idleTime = 0f; //
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Patrol 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_PATROL : FSMSingleton<Enemy_PATROL>, IFSMState<Enemy_StateManager> //PATROL상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.stoppingDistance = 0.5f;
        e.m_navAgent.speed = 1f; //패트롤 시 속도
        e.m_footstepCycle = 0.8f; //패트롤 속도에 맞춰 발걸음 재생 간격을 정해줌.
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (!e.SearchTarget()) //플레이어를 감지하지 못했을 경우
        {
            if(e.m_isStayPos)
            {
                return;
            }

            if (!e.m_navAgent.hasPath) //이미 path를 갖고있지 않을 경우
            {
                if(e.m_wayPointObj != null) //웨이포인트가 정해진 AI일 경우
                {
                    e.m_anim.SetBool("ISWALK", true); //걷는 애니메이션 작동
		    //웨이포인트 순서대로 이동시켜준다.
                    e.m_navAgent.SetDestination(e.m_wayPoints[Random.Range(1, e.m_wayPoints.Length)].position);
                }
                else //랜덤패트롤링 AI일 경우
                {
                    NavMeshHit hit;
                    Vector3 finalPosition = Vector3.zero;
                    Vector3 randomDirection = Random.insideUnitSphere * 5f; //랜덤한 방향
                    randomDirection.y = 0f;
                    randomDirection += e.gameObject.transform.position;

                    //randomDirection위치에 navMesh가 존재하여 갈 수 있는지 체크
                    if (NavMesh.SamplePosition(randomDirection, out hit, 1f, 1))
                    {
                        finalPosition = hit.position;
                    }

                    e.m_anim.SetBool("ISWALK", true); //걷는 애니메이션 작동
                    e.m_navAgent.SetDestination(finalPosition); //랜덤한 위치로 이동시켜준다.
                }
            }
            else //이미 path가 있을 경우
            {
                #region Footstep
                e.m_footstepTimer += Time.deltaTime; //발소리 재생간격 시간초 카운트

                if (e.m_footstepTimer > e.m_footstepCycle)
                {
                    if (e.m_footstepTurn == 0) //왼쪽
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 8f, 1f);
                        e.m_footstepTurn = 1;
                    }
                    else if (e.m_footstepTurn == 1) //오른쪽 발소리 재생
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 8f, 1f);
                        e.m_footstepTurn = 0;
                    }

                    e.m_footstepTimer = 0f; //초기화
                }
                #endregion

		//목적지에 거의 다가왔다면 멈춰준 후, 다시 IDLE 상태로 변경해준다.
                if (e.m_navAgent.remainingDistance <= e.m_navAgent.stoppingDistance)
                {
                    e.m_navAgent.ResetPath();
                    e.ChangeState(Enemy_IDLE.Instance);
                }
            }
        }
        else //플레이어를 감지했을 경우
        {
            e.ChangeState(Enemy_IDLE.Instance); //플레이어를 감지했으면 IDLE을 거쳐 RUN 혹은 ATTACK 상태로 변경되게 될 것임.
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_navAgent.ResetPath(); //path 초기화
        e.m_anim.SetBool("ISWALK", false);
        e.m_footstepCycle = 0f;
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Attack 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_ATTACK : FSMSingleton<Enemy_ATTACK>, IFSMState<Enemy_StateManager> //ATTACK상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
    
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if(e.SearchTarget()) //플레이어를 감지했을 경우
        {
            Vector3 dir = e.m_player.transform.position - e.gameObject.transform.position; //플레이어의 위치 방향을 구한다.
	    //보간을 이용해 부드럽게 플레이어를 바라보게 한다.
            e.gameObject.transform.forward = Vector3.Lerp(e.gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);

            if (!e.m_info.IsName("RELOAD")) //재장전 중이 아닐 경우
            {
                if (e.canAttack()) //플레이어가 공격 가능 거리내에 있을 경우
                {
                    e.m_anim.SetBool("ISATTACK", true); //공격 애니메이션 작동 -> 애니메이션 Event로 Enemy_StateManager의 Fire() 메소드를 호출함.
                }
                else
                {
                    e.m_idleTime = 1f;//IDLE로 넘어갔다가, 빠르게 재추적(RUN)하도록 시간을 1로 세팅해둠.
                    e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태변경
                }
            }
        }
        else //플레이어를 감지하지 못했을 경우
        {
            e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태변경
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_anim.SetBool("ISATTACK", false);
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Run 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_RUN : FSMSingleton<Enemy_RUN>, IFSMState<Enemy_StateManager> //RUN상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.m_navAgent.speed = 2.5f; //속도를 RUN에 맞게 설정
        e.m_footstepCycle = 0.5f; //속도에 맞게 발소리 재생간격 설정
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
        if (e.SearchTarget()) //플레이어를 감지했을 경우
        {
            e.m_anim.SetBool("ISRUN", true); //달리기 애니메이션 작동
            e.m_navAgent.SetDestination(e.m_player.transform.position); //플레이어 위치로 이동시켜준다.

            #region Footstep
            e.m_footstepTimer += Time.deltaTime; //발소리 재생간격 시간초 카운트

            if (e.m_footstepTimer > e.m_footstepCycle)
            {
                if (e.m_footstepTurn == 0) //왼발
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 1;
                }
                else if (e.m_footstepTurn == 1) //오른발 발소리 클립 재생
                {
                    SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, e.gameObject.transform.position, 20f, 1f);
                    e.m_footstepTurn = 0;
                }

                e.m_footstepTimer = 0f;
            }
            #endregion

            if (e.canAttack()) //플레이어가 공격 가능 거리내에 있을 경우
            {
                e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태 변경
            }
        }
        else //플레이어를 감지하지 못했을 
        {
            e.ChangeState(Enemy_IDLE.Instance); //IDLE로 상태 변경
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        e.m_navAgent.ResetPath();
        e.m_anim.SetBool("ISRUN", false);
        e.m_footstepCycle = 0f;
        e.m_idleTime = 1.5f; //대기시간없이 바로 attack 상태로 전이되게끔 유도.
    }
}
```

</div>
</details>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FSM State Die 접기/펼치기</summary>
<div markdown="1">

```c#
public class Enemy_DIE : FSMSingleton<Enemy_DIE>, IFSMState<Enemy_StateManager> //DIE상태 클래스 - 싱글턴 및 IFSMState 인터페이스 상속
{
    //  상태 진입..
    public void Enter(Enemy_StateManager e)
    {
        e.RagdollOnOff(false); //래그돌을 동작시키기 위해서, 모든 리지드바디들의 isKinematic을 false로 설정. 
        GameManager.Instance.AddScore(50); //플레이어가 적을 죽였으니 50점 추가.
    }

    //  상태 진행..
    public void Execute(Enemy_StateManager e)
    {
    	//5초뒤에 비활성화시켜줌.
        e.m_dieTime += Time.deltaTime; 

        if(e.m_dieTime >= 5f)
        {
            e.gameObject.SetActive(false);
        }
    }

    //  상태 종료..
    public void Exit(Enemy_StateManager e)
    {
        
    }
}
```

</div>
</details>

</div>
</details>

<br>

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;인질 FSM Code 접기/펼치기</summary>
<div markdown="1">

```c#
public class Hostage_Controller : MonoBehaviour
{
    #region Field
    public enum eState //인질이 가질 수 있는 상태들을 열거형으로 정리.
    {
        TIED,
        RESCUE,
        IDLE,
        RUN,
        MAX
    }

    eState m_state; //현재 상태
    Animator m_anim; //애니메이터
    NavMeshAgent m_nav;
    AnimatorStateInfo m_info; //현재 동작중인 애니메이션 정보
    GameObject m_player; //플레이어
    float m_disFromPlayer; //플레이어와의 유지거리
    float m_footstepTimer; //발소리 m_footstepCycle 마다 발소리를 재생시키기 위해 시간초를 카운팅하는 변수. 
    float m_footstepCycle; //발소리 재생 간격
    int m_footstepTurn; //왼발, 오른발 번갈아가며 재생하기 위함.
    #endregion

    #region Unity Methods
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_nav = GetComponent<NavMeshAgent>();
        m_disFromPlayer = 3f; //유지거리 설정
        m_state = eState.TIED; //묶여있는 상태로 초기화
        m_footstepTimer = 0f;
        m_footstepCycle = 0.5f; //인질의 이동속도에 맞춰 재생간격 설정
        m_footstepTurn = 0;
    }

    void Update()
    {
        if(m_player != null && m_state == eState.TIED) //인질이 묶여있는 상태이고, 플레이어가 감지되었을 경우
        {
            if (Input.GetKey(KeyCode.F)) //F키를 누르고 있을 경우
            {
                if (UIManager.Instance.ProgressBarFill(0.2f)) //진행바의 이미지를 서서히 채워준다. 만약 진행바가 전부 채워졌다면 내부로 진입
                {
                    m_state = eState.RESCUE; //구출된 상태로 변경
                    UIManager.Instance.ProgressObjOnOff(false); //진행바를 비활성화
                    SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.UNTIE_HOSTAGE, 1f); //밧줄을 풀어주는 사운드 재생.
                }
            }
            else //F키에서 손을 뗐을 경우
            {
                UIManager.Instance.ProgressBarFill(-0.2f); //진행바의 이미지를 서서히 비워준다.
            }
        }

        switch(m_state) //FSM
        {
            case eState.TIED: //묶여있는 상태
                break;

            case eState.RESCUE: //구출된 직후 상태
                m_anim.SetTrigger("ISRESCUE"); //구출 애니메이션 작동

                m_info = m_anim.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 정보를 받아옴.
                if (m_info.IsName("Hostage_IDLE")) //만약 구출 애니메이션 작동이 끝나고, IDLE 애니메이션으로 넘어간 상태일 경우
                {
                    GameManager.Instance.AddScore(500); //500점 추가.
                    GameManager.Instance.HostageRescued(); //탈출 시 만나게될 새로운 적들을 활성화시키고, SafeZone 파티클시스템을 활성화시켜줌.
                    m_state = eState.IDLE; //IDLE상태로 변경
                }
                break;

            case eState.IDLE: //Idle 상태
                m_nav.ResetPath(); //path 초기화
                Vector3 dir = m_player.transform.position - gameObject.transform.position; //플레이어의 위치 방향을 구함.
		//보간을 이용해 부드럽게 플레이어를 바라보도록 함.
                gameObject.transform.forward = Vector3.Lerp(gameObject.transform.forward, new Vector3(dir.x, 0f, dir.z), Time.deltaTime * 3f);

		//플레이어와의 거리가 유지거리 이상으로 멀어질 경우
                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) > m_disFromPlayer)
                {
                    m_state = eState.RUN; //RUN 상태로 변경한다.
                }
                break;

            case eState.RUN: //달리기 상태
                m_anim.SetBool("ISRUN", true); //달리기 애니메이션 작동
                m_nav.SetDestination(m_player.transform.position); //플레이어의 위치로 이동시켜줌.

                #region Footstep
                m_footstepTimer += Time.deltaTime; 

                if (m_footstepTimer > m_footstepCycle) //발소리 재생간격 시간초 카운트
                {
                    if (m_footstepTurn == 0) //왼발
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP3, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 1;
                    }
                    else if (m_footstepTurn == 1) //오른발 클립 재생
                    {
                        SoundManager.Instance.Play3DSound(SoundManager.eAudioClip.FOOTSTEP4, gameObject.transform.position, 10f, 1f);
                        m_footstepTurn = 0;
                    }

                    m_footstepTimer = 0f;
                }
                #endregion

		//플레이어와의 거리가 유지거리 내일 경우
                if (Vector3.Distance(gameObject.transform.position, m_player.transform.position) <= m_disFromPlayer)
                {
                    m_anim.SetBool("ISRUN", false); //달리기 애니메이션 중지
                    m_state = eState.IDLE; //Idle 상태로 변경
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    	//인질이 묶여있는 상태이며, 플레이어가 인질의 trigger 콜라이더 내에 진입했을 경우
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED )
        {
            m_player = other.transform.root.gameObject; //m_player가 구출가능 거리로 다가왔음을 알리기 위함.
            UIManager.Instance.ProgressObjOnOff(true); //구출 진행바를 활성화
        }
    }

    private void OnTriggerExit(Collider other)
    {
    	//인질이 묶여있는 상태이며, 플레이어가 인질의 trigger 콜라이더 바깥으로 나갔을 경우
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) || other.gameObject.name.Equals("Player")) && m_state == eState.TIED)
        {
	    m_player = null; //m_player가 구출가능 거리가 아니라는것을 알리기 위함.
            UIManager.Instance.ProgressObjOnOff(false); //구출 진행바를 비활성화
        }
    }
    #endregion
}
```

</div>
</details>

**Explanation**:gun:<br>
(구현설명은 주석으로 간단하게 처리했습니다!)<br>
적 AI 같은 경우, 상태들을 클래스로 관리하는 FSM으로 구현했습니다. 각 상태들은 싱글턴패턴을 적용시켜 상태변환시마다 new, delete가 난무하는것을 예방함으로써, 오버헤드와 메모리 낭비를
방지하고자 했습니다. 또한 각 상태들이 동일한 메소드(동작)를 포함하도록 강제하고, 다중상속 문제를 해결하며 다형성을 이용하기위해 IFSM 인터페이스를 구현하도록 했습니다. 
이러한 구조를 이용하니 클래스간 느슨한 결합도를 유지할 수 있어서 Open-Closed Principle(확장에는 열리게 하고, 수정에는 닫히게 해야 한다는 객체지향 원칙)을 지킬 수 있었습니다.
또한, 구조가 심플하다보니 개발하는데 있어서 좀 더 수월해지는 이점도 존재했습니다.

인질같은 경우 정말 간단한 상태만을 갖는 AI로 구현하면 됐기 때문에, 위처럼 개발하는것이 오히려 낭비라고 생각하여 한 class내에서 switch-case문으로 간단하게 구현하였습니다.

</div>
</details>

<br>


### Difficult Point.:sweat_smile:
* 
<br>

### Feeling.:pencil:

<br>



메인화면 이미지 출처 https://1freewallpapers.com/point-blank-swat-game/ko
