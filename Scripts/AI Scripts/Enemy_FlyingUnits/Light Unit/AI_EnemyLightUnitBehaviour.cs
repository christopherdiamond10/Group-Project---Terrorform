//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Heavy Unit Behaviour
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: June 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script defines the Behaviour of the Light Unit. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AI_EnemyLightUnitBehaviour : AI_FlyingEnemyBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum PathChoosing
	{
		STRAIGHT,
		BEZIER_CURVES,
		FOLLOW_PLAYER,
	};

	public enum Stance
	{
		FOLLOW_PLAYER_BEGIN,
		IDLE,
		ATTACKING,
		DEATH,
	}

	private struct PlayerFollowerPositions
	{
		public Vector3		vTargetPosition;					// Once Connected to Player, where do I want to move?
		public float		fMovementSpeed;						// Speed I Move With Once Attached
		public bool			bMovingOntoView;					// Moving In front of the Player?
		public bool			bFinalMovement;						// Final Movement? Am I Leaving the Player Alone?
		public TimeTracker	TTHangAroundTimer;					// The TimeTracker for How Long the Unit will fly in front of the player once it is there.
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AudioClip	m_HissSound;													// Audio Clip for Hiss Sound
	public PathChoosing m_ePathChoosing					= PathChoosing.FOLLOW_PLAYER;	// The Path Choice
	public float		m_fFollowPlayerTimeBegin		= 5.0f;							// How Long Should I Hang out in front of the player?
    public float        m_fFollowPlayerTimeEnd			= 10.0f;
	public float		m_fPlayerFollowSpeedRangeBegin	= 75.0f;
	public float		m_fPlayerFollowSpeedRangeEnd	= 200.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AudioSource				m_HissSoundAudioSource;					// Audio Source Component for Hiss Sound
	private Stance					m_eCurrentStance;						// Current Stance
	private PlayerFollowerPositions m_PlayerFollowerPositions;				// Struct Instance

    private static float            sm_ShootEventCurveClimax = 0.05f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Start()
	{
		base.Start();
		CreateFullPath();

		// Add Audio
		m_HissSoundAudioSource = SoundPlayerManager.AddAudio(gameObject, m_HissSound);

		SetCurrentStance( (m_ePathChoosing == PathChoosing.FOLLOW_PLAYER) ? Stance.FOLLOW_PLAYER_BEGIN : Stance.IDLE );
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
		switch (m_eCurrentStance)
		{
			
            case Stance.FOLLOW_PLAYER_BEGIN:
			{
				UpdateFollowPlayerBeginStance();
				break;
			}
			
			case Stance.IDLE:
			{
				UpdateMovement();
				UpdateIdleStance();
				break;
			}

			case Stance.ATTACKING:
			{
				UpdateMovement();
				UpdateAttackStance();
				break;
			}

			case Stance.DEATH:
			{
				UpdateDeathStance();
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Follow Player Begin Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateFollowPlayerBeginStance()
	{
		// Setup Player Following Variables
        m_fFollowPlayerTimeBegin                        = (m_fFollowPlayerTimeBegin < 0.01f) ? 5.0f  : m_fFollowPlayerTimeBegin;
        m_fFollowPlayerTimeEnd                          = (m_fFollowPlayerTimeEnd   < 0.01f) ? 10.0f : m_fFollowPlayerTimeEnd;
        float fWaitTime                                 = Random.Range(m_fFollowPlayerTimeBegin, m_fFollowPlayerTimeEnd);

		m_PlayerFollowerPositions						= new PlayerFollowerPositions();
		m_PlayerFollowerPositions.TTHangAroundTimer		= new TimeTracker(fWaitTime, false);

		transform.parent = Camera.main.transform;
		SetLocalPosition(GetRandomOffscreenPoint());

		m_PlayerFollowerPositions.fMovementSpeed		= Random.Range(m_fPlayerFollowSpeedRangeBegin, m_fPlayerFollowSpeedRangeEnd);
		m_PlayerFollowerPositions.vTargetPosition		= GetRandomPointAheadOfPlayer();
		m_PlayerFollowerPositions.bFinalMovement		= false;
		m_PlayerFollowerPositions.bMovingOntoView		= true;

		SetCurrentStance( Stance.IDLE );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Idle Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIdleStance()
	{
        if( !IsLeaving() )
        {
			if( m_ePathChoosing != PathChoosing.FOLLOW_PLAYER || !m_PlayerFollowerPositions.bMovingOntoView )
			{
				m_TTFireCooldownTimer.Update();
				if (m_TTFireCooldownTimer.TimeUp())
				{
					if( m_HissSoundAudioSource != null )
					{
						m_HissSoundAudioSource.Play();
					}

					StartPlayingAttackAnimation();
					SetCurrentStance( Stance.ATTACKING );
				}
			}

			if (m_ePathChoosing == PathChoosing.FOLLOW_PLAYER && !m_PlayerFollowerPositions.bMovingOntoView && !m_PlayerFollowerPositions.bFinalMovement)
			{
				m_PlayerFollowerPositions.TTHangAroundTimer.Update();

				if (m_PlayerFollowerPositions.TTHangAroundTimer.TimeUp())
				{
					m_PlayerFollowerPositions.bFinalMovement = true;
					m_PlayerFollowerPositions.vTargetPosition = GetRandomOffscreenPoint();
				}
			}
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Attack Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAttackStance()
	{
        if( GetAnimatorComponent().GetFloat(GetAnimationParamHashIDs().ShootEventParamID) > sm_ShootEventCurveClimax )
		{
			FireTowardsPlayer();
			m_TTFireCooldownTimer.Reset();
		    StopPlayingAttackAnimation();
			SetCurrentStance( Stance.IDLE );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Death Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDeathStance()
	{
		DestroySelf();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Run End Of Flight Path Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RunEndOfFlightPathCommand()
	{
		SetCurrentStance( Stance.DEATH );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Playing Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartPlayingAttackAnimation()
	{
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().ShootingParamID, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Playing Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopPlayingAttackAnimation()
	{
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().ShootingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void UpdateMovement()
	{
		if( m_ePathChoosing == PathChoosing.FOLLOW_PLAYER )
		{
			MoveTowardsTarget();

			if( Vector3.Distance(GetLocalPosition(), GetTargetPosition()) < GetDeltaMovementSpeed() )
			{
				if( m_PlayerFollowerPositions.bMovingOntoView )
				{
					m_PlayerFollowerPositions.bMovingOntoView = false;
				}
				else if( m_PlayerFollowerPositions.bFinalMovement )
				{
					DestroySelf();
				}
				else
				{
					m_PlayerFollowerPositions.vTargetPosition = GetRandomPointAheadOfPlayer();
				}
			}
		}

		else
		{
			base.UpdateMovement();
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Rotate Towards Player 
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected override void RotateTowardsPlayer()
    {
        if ( IsLeaving() )
        {
            SetLookAtRotation(Camera.main.transform.position + m_PlayerFollowerPositions.vTargetPosition);
        }
        else
        {
            base.RotateTowardsPlayer();
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Full Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateFullPath()
	{
		if( m_ePathChoosing != PathChoosing.FOLLOW_PLAYER )
		{
			if (m_ePathChoosing == PathChoosing.BEZIER_CURVES)
			{
				CreateBezierCurvesPath();	
			}
			else
			{
				CreateStraightPath();
			}
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Is Leaving?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool IsLeaving()
    {
        return (m_ePathChoosing == PathChoosing.FOLLOW_PLAYER) && m_PlayerFollowerPositions.bFinalMovement;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentStance(Stance stance)
	{
		m_eCurrentStance = stance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Contact With Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnContactWithPlayer()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Target Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override Vector3 GetTargetPosition()
	{
		return (m_ePathChoosing == PathChoosing.FOLLOW_PLAYER) ? m_PlayerFollowerPositions.vTargetPosition : base.GetTargetPosition();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Delta Movement Speed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override float GetDeltaMovementSpeed()
	{
		// If Following Player, move at slower speed; else just go about with your normal speed.
		if( m_ePathChoosing == PathChoosing.FOLLOW_PLAYER )
		{
			if (m_PlayerFollowerPositions.bMovingOntoView)
				return m_PlayerFollowerPositions.fMovementSpeed * 3.0f * Time.deltaTime;

			if (m_PlayerFollowerPositions.bFinalMovement)
				return m_PlayerFollowerPositions.fMovementSpeed * 2.0f * Time.deltaTime;

			return m_PlayerFollowerPositions.fMovementSpeed * (m_PlayerFollowerPositions.bMovingOntoView ? (3.0f * Time.deltaTime) : Time.deltaTime);
		}

		return base.GetDeltaMovementSpeed();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Random Point In Front of Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 GetRandomPointAheadOfPlayer()
	{
		Vector3 vMinBounds = new Vector3( -210.0f, -120.0f, 0.0f );
		Vector3 vMaxBounds = new Vector3(  210.0f,  120.0f, 0.0f );

		Vector3 vRandPos = new Vector3();
		vRandPos.x = Random.Range( vMinBounds.x, vMaxBounds.x );
		vRandPos.y = Random.Range( vMinBounds.y, vMaxBounds.y );
		vRandPos.z = Random.Range( 330.0f, 450.0f );
		return vRandPos;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Random Offscreen Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 GetRandomOffscreenPoint()
	{
		int iRandNum = Random.Range(0, 4);
		switch( iRandNum )
		{
			case 0:	 return  Camera.main.transform.up	 * (Camera.main.fieldOfView * 3.0f); // UP
			case 1:  return -Camera.main.transform.right * (Camera.main.fieldOfView * 3.0f); // LEFT
			case 2:  return  Camera.main.transform.right * (Camera.main.fieldOfView * 3.0f); // RIGHT
			default: return -Camera.main.transform.up	 * (Camera.main.fieldOfView * 3.0f); // DOWN
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Enemy Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override UnitType GetEnemyType()
	{
		return UnitType.LIGHT_UNIT;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static EnemyFlyingLightUnitAnimationHashIDs.AnimationStateHashIDs GetAnimationStateHashIDs()
	{
		return EnemyFlyingLightUnitAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static EnemyFlyingLightUnitAnimationHashIDs.AnimationParamHashIDs GetAnimationParamHashIDs()
	{
		return EnemyFlyingLightUnitAnimationHashIDs.GetParamHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmosSelected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmosSelected()
	{
		if (m_ePathChoosing != PathChoosing.FOLLOW_PLAYER && GameHandler.UnitySceneOnly())
		{
			CreateFullPath();
			Gizmos.color = GetFlightPathColour();
			Vector3 vCurrentPos = GetWorldPosition();
			for (int i = 0; i < m_FlightPath.GetSize(); ++i)
			{
				Gizmos.DrawLine(vCurrentPos, m_FlightPath[i]);
				vCurrentPos = m_FlightPath[i];
			}
		}
	}
}
