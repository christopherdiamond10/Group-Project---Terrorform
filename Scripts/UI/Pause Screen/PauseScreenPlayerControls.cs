using UnityEngine;
using System.Collections;

public class PauseScreenPlayerControls : MonoBehaviour 
{
	public Transform LeftGun;
	public Transform RightGun;
	public Transform MissileLauncher;
	public Transform StartPos;
	public Transform EndPos;
	
	public bool m_bHitTerrain;

	public AudioClip[] m_BulletSoundFiles;
	
	public float m_fSpeed;
	public float m_fNormSpeed = 0;
	public float m_fStrafeSpeed = 0;
	public float m_fBoostSpeed = 0;
	public float m_fSlowSpeed = 0;
	public float m_fAltSpeed = 0;
	public float m_fStopDistance = 0;
	public float m_fRollSpeed = 0;
	
	private Vector3 m_Velocity;
	
	private bool m_bRollingLeft;
	private bool m_bRollingRight;
	private bool m_bSlowed;
	
	private float m_fRollTimer;
	
	private float m_fBoostTimer;
	
	public float m_fHealth;
	
	private float m_fRollAmount;
	public float m_fMoveMultipier;
	
	public GameObject bullet;
	public GameObject missile;
	
	public PauseScreenAimer targetAim;
	private Vector3 AimDir;
	
	public Vector3 m_vStartPos;
	
	private bool m_bShootDown;
	private bool m_bMissileDown;
	
	private bool m_bDead;
	private float deathTimer;
	
	
	private float m_fCollReset = 0.5f;
	public float m_fCollTimer = 0;
	
	public float m_fCamMaxSpeed;
	
	public Transform m_tTrack;
	
	public bool m_bBrainBugged;
	public void SetBugged(bool a_bBugged)
	{
		m_bBrainBugged = a_bBugged;
		targetAim.SetBugged(a_bBugged);
	}
	
	public Vector2 m_v2MinBounds;
	public Vector2 m_v2MaxBounds;
	
	private float m_fBBTimer;
	private float m_fBBResetTime = 2;
	
	public GameObject m_goParticleRoot;
	public GameObject m_goMidEngine;
	public GameObject m_goRingBlast;
	
	public GameObject m_go100Health;
	
	public float m_fHitTimer = 0;
	public float m_fHitReset = 0.1f;
	
	private Vector3 m_v3PrevRot;
	
	//ADDED By Bernard
	//===============================================================
	public int m_iPlayerScore = 0;
	public int m_iBombCount = 0;
	//===============================================================
	public bool m_bIsHit = false;
	public bool m_bIsShot = false;

    private XboxInputHandler.Controls[] m_aFireTriggerAxis = new XboxInputHandler.Controls[] { XboxInputHandler.Controls.A, };
	
	// Use this for initialization
	void Start () 
	{
		Cursor.visible = false;
		
		m_fSpeed        = (m_fSpeed         != 0) ? m_fSpeed        : 100f;
		m_fNormSpeed    = (m_fNormSpeed     != 0) ? m_fNormSpeed    : 100f;
		m_fStrafeSpeed  = (m_fStrafeSpeed   != 0) ? m_fStrafeSpeed  : 0.8f;
		m_fBoostSpeed   = (m_fBoostSpeed    != 0) ? m_fBoostSpeed   : 40f;
		m_fSlowSpeed    = (m_fSlowSpeed     != 0) ? m_fSlowSpeed    : 5f;
		m_fAltSpeed     = (m_fAltSpeed      != 0) ? m_fAltSpeed     : 0.5f;
		m_fStopDistance = (m_fStopDistance  != 0) ? m_fStopDistance : 2f;
		m_fCamMaxSpeed  = (m_fCamMaxSpeed   != 0) ? m_fCamMaxSpeed  : 80f;
		m_fRollSpeed    = (m_fRollSpeed     != 0) ? m_fRollSpeed    : m_fNormSpeed * 2;
		
		
		m_v2MaxBounds.x = (m_v2MaxBounds.x != 0) ? m_v2MaxBounds.x :  100f;
		m_v2MaxBounds.y = (m_v2MaxBounds.y != 0) ? m_v2MaxBounds.y :  100f;
		m_v2MinBounds.x = (m_v2MinBounds.x != 0) ? m_v2MinBounds.x : -100f;
		m_v2MinBounds.y = (m_v2MinBounds.y != 0) ? m_v2MinBounds.y : -100f;
		
		m_Velocity      = new Vector3(0,0,0);
		m_bRollingLeft  = false;
		m_bRollingRight = false;
		m_bSlowed       = false;
		
		m_bDead         = false;
		deathTimer      = 0;
		
		m_fHealth = (m_fHealth != 0) ? m_fHealth : 100;
		
		m_vStartPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		
		m_bHitTerrain = false;		
	}
	
	// Update is called once per frame
	void Update () 
	{		
		m_Velocity = new Vector3(0,0,0);
		
		MovePlayer();
		
		GetInput();
		
		RollShip();
		
		//CameraFollow();
		
		if(m_bIsShot)
		{
			m_fHitTimer += DynamicUpdateManager.GetDeltaTime();
		}
		
		if(m_fHitTimer > m_fHitReset)
		{
			m_bIsShot = false;
			gameObject.GetComponent<Animator>().SetBool("PlayerHit", m_bIsShot);
			m_fHitTimer = 0;
		}
		
		if(m_bDead)
		{
			deathTimer += DynamicUpdateManager.GetDeltaTime();
		}
		
		if(m_bBrainBugged)
		{
			m_fBBTimer += DynamicUpdateManager.GetDeltaTime();
			
			if(m_fBBTimer > m_fBBResetTime)
			{
				SetBugged(false);
			}
		}
		
		if(Input.GetKey(KeyCode.Escape))
		{
			GameHandler.Quit ();
		}
		
		
		if(m_fCollTimer > m_fCollReset)
		{
			gameObject.GetComponent<Collider>().enabled = true;
		}
		/////////////////////////////////////////////////////
		
		//Controls the state of the rolling ship
		if(m_bRollingLeft || m_bRollingRight)
		{
			m_fRollTimer += DynamicUpdateManager.GetDeltaTime();
		}
		
		if(m_fRollTimer > 0.5)
		{
			m_bRollingLeft = false;
			targetAim.m_bRollLeft = false;
			gameObject.GetComponent<Animator>().SetBool("RollingLeft",m_bRollingLeft);
			m_bRollingRight = false;
			targetAim.m_bRollRight = false;
			gameObject.GetComponent<Animator>().SetBool("RollingRight",m_bRollingRight);
			m_fRollTimer = 0f;
			m_fMoveMultipier = 0f;
		}
		
		//Rolls the ship left or right based on input
		if(Input.GetKey (KeyCode.Q))
		{
			RollShipLeft();
		}
		else if(Input.GetKey(KeyCode.E))
		{
			RollShipRight();
		}
		
		//////////////////////////////////////////////////////////////////////////
		//Read the buttons used to pick up shooting a bullet or missile
		//Only shoots as fast as you can press
		//////////////////////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Space) || XboxInputHandler.KeyTriggered(m_aFireTriggerAxis))
        {
            ShootBullet();
        }
	}
	
	/// <summary>
	/// Gets the velocity of the player.
	/// </summary>
	/// <returns>
	/// The velocity of the player.
	/// </returns>
	public Vector3 GetVelocity()
	{
		Vector3 temp = new Vector3(1, m_Velocity.y, m_Velocity.z);
		
		return temp;
	}
	
	/// <summary>
	/// Boosts the speed.
	/// </summary>
	void BoostSpeed()
	{
	}
	
	/// <summary>
	/// Slows the speed.
	/// </summary>
	void SlowSpeed()
	{
		if(!m_bSlowed)
		{
			m_bSlowed = true;
		}
	}	
	
	/// <summary>
	/// Rolls the ship left.
	/// </summary>
	void RollShipLeft()
	{
		if(!m_bRollingLeft && !m_bRollingRight && m_fRollTimer == 0)
		{
			m_bRollingLeft = true;
			targetAim.m_bRollLeft = true;
			gameObject.GetComponent<Animator>().SetBool("RollingLeft",m_bRollingLeft);	
			m_fMoveMultipier = 1.5f;
		}
	}
	
	/// <summary>
	/// Rolls the ship right.
	/// </summary>
	void RollShipRight()
	{
		if(!m_bRollingRight && !m_bRollingLeft && m_fRollTimer == 0)
		{
			m_bRollingRight = true;
			targetAim.m_bRollRight = true;
			gameObject.GetComponent<Animator>().SetBool("RollingRight",m_bRollingRight);
			m_fMoveMultipier = -1.5f;
		}
	}
	
	/// <summary>
	/// Fires the main guns.
	/// </summary>
	void ShootBullet()
	{
		GameObject LeftBullet = Instantiate(bullet) as GameObject;
		GameObject RightBullet = Instantiate(bullet) as GameObject;
		LeftBullet.transform.position = RightGun.position;
		RightBullet.transform.position = LeftGun.position;
		LeftBullet.GetComponent<PauseScreenPlayerBulletMovement>().owningPlayer = this;
		RightBullet.GetComponent<PauseScreenPlayerBulletMovement>().owningPlayer = this;
		LeftBullet.GetComponent<PauseScreenPlayerBulletMovement>().targetAim = targetAim;
		RightBullet.GetComponent<PauseScreenPlayerBulletMovement>().targetAim = targetAim;

		LeftBullet.GetComponent<PauseScreenPlayerBulletMovement>().m_fSpeed = 100.0f;
		LeftBullet.transform.LookAt(LeftBullet.transform.position + transform.forward);
		RightBullet.GetComponent<PauseScreenPlayerBulletMovement>().m_fSpeed = 100.0f;
		RightBullet.transform.LookAt(RightBullet.transform.position + transform.forward);

		// Play Random Bullet Sound
		if(m_BulletSoundFiles != null)
		{
			GetComponent< AudioSource >().PlayOneShot( m_BulletSoundFiles[ Random.Range(0, m_BulletSoundFiles.Length) ] );
		}
	}
	
	/// <summary>
	/// Shoots the missiles.
	/// </summary>
	void ShootMissile()
	{
		//if there are still missiles ready to fire.
		missile.transform.position = MissileLauncher.position;
		Instantiate(missile);
	}
	
	/// <summary>
	/// Sets the bounce directions.
	/// </summary>
	/// <param name='bounce'>
	/// Does the player need to bounce?
	/// </param>
	/// <param name='bounceDir'>
	/// Calculated bounce direction.
	/// </param>
	public void SetBounce(bool bounce, Vector3 bounceDir)
	{
		m_bHitTerrain = bounce;
	}
	
	/// <summary>
	/// Moves the player based on where the aimer is.
	/// Rotates the ship to face the aimer and adjusts x and y locations.
	/// </summary>
	public void MovePlayer()
	{
		float dist = Vector2.Distance(new Vector2(targetAim.transform.localPosition.x,targetAim.transform.localPosition.y), new Vector2(transform.localPosition.x, transform.localPosition.y));
		
		if(isRolling())
		{
			m_fSpeed = m_fRollSpeed;
		}
		
		//rotates the ship to look at the aimer
		Quaternion rotation = Quaternion.LookRotation(new Vector3(targetAim.transform.position.x,targetAim.transform.position.y,targetAim.transform.position.z) - transform.position);
		
		Vector3 tempAngles = new Vector3(rotation.eulerAngles.x,rotation.eulerAngles.y,m_v3PrevRot.z);
		float TiltMod = targetAim.GetTiltMod();
		
		if(tempAngles.z > -30 && tempAngles.z < 30 || tempAngles.z > 330)
		{
			tempAngles.z = 15 * TiltMod;
		}
		
		if(TiltMod == 0)
		{
			tempAngles.z = 0;
		}
		
		rotation.eulerAngles = tempAngles;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, DynamicUpdateManager.GetDeltaTime() * 6.0f);
		
		m_v3PrevRot = transform.localRotation.eulerAngles;
		
		//transform.localRotation = modifiedRot;
		
		//grab targets rotation
		//add it to player rotation
		
		targetAim.MoveAdjust = m_fMoveMultipier;
		targetAim.StrafeSpeed = m_fStrafeSpeed;
		targetAim.AltSpeed = m_fAltSpeed;
		
		
		Vector3 targetPos = targetAim.transform.localPosition;
		targetPos.z -= targetAim.m_PlayerOffset;
		
		
		
		m_Velocity = targetPos - transform.localPosition;
		m_Velocity.z = 0;
		m_Velocity.Normalize();
		
		Vector3 PositionCheck = transform.localPosition + m_Velocity * (m_fSpeed * DynamicUpdateManager.GetDeltaTime());
		
		if(PositionCheck.x > m_v2MaxBounds.x || PositionCheck.x < m_v2MinBounds.x)
		{
			m_Velocity.x = 0;
		}
		if(PositionCheck.y > m_v2MaxBounds.y || PositionCheck.y < m_v2MinBounds.y)
		{
			m_Velocity.y = 0;
		}
		
		if(dist > m_fStopDistance)// * targetAim.GetMoveValue())
		{
			transform.localPosition += m_Velocity * (m_fSpeed * DynamicUpdateManager.GetDeltaTime());
		}	
		
	}

	
	void GetInput()
	{
	}
	
	/// <summary>
	/// Rolls the ship based on player input.
	/// </summary>
	void RollShip()
	{
	}	
	
	public Vector3 Lerp(float min, float max, float optimal, Vector2 pos1, Vector2 pos2)
	{
	
		float dist = Vector2.Distance(pos1,pos2);
		
		if(dist > 0.01f)
		{
			//Lerp camera to player
			
			//greater distance faster camera
			//less distance slower camera
			
			float MaxSpeed = max;
			
			float MinSpeed = min;
			
			float t = dist / optimal;
			
			t = Mathf.Clamp(t,0f,1f);
			
			float Speed = (MinSpeed + (MaxSpeed - MinSpeed) * t);
			
			Vector2 dir = pos1 - pos2;
			
			dir = dir.normalized;
			
			return new Vector2(dir.x, dir.y) * (-Speed * DynamicUpdateManager.GetDeltaTime());
		}
		return new Vector2(0,0);
	}
	
	/// <summary>
	/// Gets the position of the player.
	/// </summary>
	/// <returns>
	/// The players transform position.
	/// </returns>
	public Vector3 GetPosition()
	{
		return transform.position;
	}
	
	/// <summary>
	/// Makes the player take damage.
	/// Kills the player if health is less zero.
	/// </summary>
	/// <param name='Damage'>
	/// The damage value.
	/// </param>
	public void TakeDamage(float Damage)
	{
		m_fHealth -= Damage;
		m_bIsShot = true;
		m_bIsHit = true;
		m_fHitTimer = 0;
		
		gameObject.GetComponent<Animator>().SetBool("PlayerHit",m_bIsShot);
		
		if(m_fHealth > 75)
		{
			m_go100Health.GetComponent<Renderer>().enabled = true;
		}
		
		if(m_fHealth <= 0f && !m_bDead)
		{
			KillMe();
		}
	}
	
	/// <summary>
	/// Kills the player ship.
	/// </summary>
	private void KillMe()
	{
		m_Velocity = Vector3.zero;
		
		foreach( Renderer r in GetComponentsInChildren< Renderer >() )
		{
			r.enabled = false;
		}
	}
	
	public bool isRolling()
	{
		return m_bRollingLeft || m_bRollingRight;
	}
}
