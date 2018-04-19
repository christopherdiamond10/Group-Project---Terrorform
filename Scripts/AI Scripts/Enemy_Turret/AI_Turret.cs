//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             AI Turret
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 15, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script Contains the Behaviour for Turrets as well as their Animation
//	  commands. Since the Turret inherits from AI_Base it also maintains the
//	  health & damage variables.
//
//	  This Scripts handles the Turret Animation State changes. This is done by 
//	  interacting with the Animator Component and adjusting variables.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class AI_Turret : AI_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum Stance
	{
		SURFACING,
		IDLE,
		SHOOTING,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public int			m_iFireRate		= 1;				// How Often Can I Fire Every Second?
	public float		m_fBulletSpeed	= 1200.0f;			// How Quick Do My Bullets Fire?

    public GameObject	m_goBullet;							// The Prefab for this Unit to Fire towards Player.
	public GameObject	m_goTurretHead;						// Head of Turret
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TimeTracker m_TTSurfaceTime;						// Randomise a Time Between Two Seconds, once Timer has hit Two Seconds, surface
	private TimeTracker m_TTFireCooldown;						// Cooldown Timer
	private Stance		m_eCurrentStance = Stance.SURFACING;	// Current Stance
	private Vector3		m_vOriginalPosition;					// Original Position Before it was changed for Surfacing.

	static float		sm_fShootEventCurveClimax = 0.05f;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Start()
    {
        base.Start();
        SetupFireRate();
		SetupPosition();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup FireRate
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void SetupFireRate()
    {
        m_iFireRate = (m_iFireRate == 0) ? 1 : m_iFireRate;						// Default to One Bullet Per Second

		m_TTFireCooldown = new TimeTracker( (1.0f / m_iFireRate), false );		// FireRate in Seconds.
		m_TTSurfaceTime = new TimeTracker( 2.0f, false );

		float fNewTime = Random.Range(0, 200) * 0.01f;
		m_TTSurfaceTime.m_fCurrentTime = fNewTime;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupPosition()
	{
		m_vOriginalPosition		= GetWorldPosition();

		Vector3 NewPosition		= GetWorldPosition();
		Vector3 NewDownPosition = Vector3.Scale(GetDownVector() * 20.0f, GetScale());

		SetWorldPosition( NewPosition + NewDownPosition );
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Update
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Update()
    {
		base.Update();
		UpdateSelf();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Update Self
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void UpdateSelf()
    {
		switch (GetCurrentStance())
		{
			case Stance.SURFACING:
			{
				UpdateSurfacingStance();
				break;
			}

			case Stance.IDLE:
			{
				UpdateIdleStance();
				break;
			}

			case Stance.SHOOTING:
			{
				UpdateShootingStance();
				break;
			}

			default:
			{
				break;
			}
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Surfacing Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSurfacingStance()
	{
		m_TTSurfaceTime.Update();

		if( m_TTSurfaceTime.TimeUp() )
		{
			// Start Playing Surface Animation (begins next frame)
			StartPlayingSurfaceAnimation();

			// If Playing Surfacing Animation, Set Surfaced bool to true, so this Animation never plays again.
			if( IsPlayingAnimation(GetAnimationStateHashIDs().SurfacingStateID) )
			{
				float fMovementSpeed = 100.0f * Time.deltaTime;
				GetLocalTransform().position += (m_vOriginalPosition - GetWorldPosition()).normalized * fMovementSpeed;
				if ((m_vOriginalPosition - GetWorldPosition()).magnitude < (fMovementSpeed * 2.0f))
				{
					GetLocalTransform().position = m_vOriginalPosition;
				}
				GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().HasSurfacedParamID, true);
			}
		
			// If Not playing Surfacing Animation and HasSurfaced? is ON.
			else if (IsNotPlayingAnimation(GetAnimationStateHashIDs().SurfacingStateID) && GetAnimatorComponent().GetBool(GetAnimationParamHashIDs().HasSurfacedParamID))
			{
				SetCurrentStance( Stance.IDLE );				// Change to Idle Stance
				m_TTFireCooldown.Reset();						// Reset Shoot Cooldown Timer
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Idle Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIdleStance()
	{
		m_TTFireCooldown.Update();
		if (m_TTFireCooldown.TimeUp())                      // If Shoot Cooldown Timer is Complete
		{
			SetCurrentStance( Stance.SHOOTING );			// Begin Shoot Stance
			StartPlayingShootAnimation();					// Play Shoot Animation
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Shooting Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateShootingStance()
	{
		StopPlayingShootAnimation();
		if (GetAnimatorComponent().GetFloat(GetAnimationParamHashIDs().ShootEventParamID) > sm_fShootEventCurveClimax)	// When Shoot Animation is Completed
		{
			FireTowardsPlayer();							// Fire Bullet
			SetCurrentStance( Stance.IDLE );				// Go Back to Idle
			m_TTFireCooldown.Reset();						// Reset Shoot Cooldown Timer
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fire Towards Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void FireTowardsPlayer()
    {
		// Create Projectile
		GameObject Projectile						= Instantiate(m_goBullet, m_goTurretHead.transform.position, Quaternion.identity) as GameObject;
		BasicBulletMovementScript ProjectleMovement = Projectile.GetComponent< BasicBulletMovementScript >();
		
		// Set Forward Vector, this is the direction the projectile moves towards
		Projectile.transform.forward = GetForwardVector();

		// Set Projectile Damage, Speed, and Travel Distance until Self.Destroy
		ProjectleMovement.SetImpactDamage( m_fOutputDamage );
		ProjectleMovement.SetMovementSpeed( m_fBulletSpeed );
		ProjectleMovement.SetExpectedTravelDistance( m_fBulletSpeed * 2.0f );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Surface Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartPlayingSurfaceAnimation()
	{
		GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().SurfacingParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Playing Surface Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopPlayingSurfaceAnimation()
	{
		GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().SurfacingParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Shoot Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartPlayingShootAnimation()
	{
		GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().ShootingParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Playing Shoot Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopPlayingShootAnimation()
	{
		GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().ShootingParamID, false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentStance(Stance stance)
	{
		m_eCurrentStance = stance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Stance GetCurrentStance()
	{
		return m_eCurrentStance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Enemy Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override UnitType GetEnemyType()
	{
		return UnitType.TURRET;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static TurretAnimationHashIDs.AnimationStateHashIDs GetAnimationStateHashIDs()
	{
		return TurretAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static TurretAnimationHashIDs.AnimationParamHashIDs GetAnimationParamHashIDs()
	{
		return TurretAnimationHashIDs.GetParamHashIDs();
	}
}
