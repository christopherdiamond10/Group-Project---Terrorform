//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Heavy Unit Behaviour
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: June 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script defines the Behaviour of the Heavy Unit. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AI_EnemyHeavyUnitBehaviour : AI_FlyingEnemyBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum PathChoosing
	{
		STRAIGHT,
		BEZIER_CURVES,
	};
	
	public enum Stance
	{
		IDLE,
		ATTACKING,
		DEATH,				// Death Animation
		DEAD,				// Just Dead, No Moving
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AudioClip m_DeathHissSoundClip;
	public PathChoosing m_ePathChoosing = PathChoosing.BEZIER_CURVES;	// The Path Choice
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AudioSource m_DeathHissAudioSource;
	private Stance m_eCurrentStance = Stance.IDLE;						// Current Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Start()
	{
		base.Start();
		CreateFullPath();

		m_DeathHissAudioSource = SoundPlayerManager.AddAudio(gameObject, m_DeathHissSoundClip);
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
	//	* New Method: Update Idle Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIdleStance()
	{
		m_TTFireCooldownTimer.Update();
		if (m_TTFireCooldownTimer.TimeUp())
		{
			StartPlayingAttackAnimation();
			SetCurrentStance(Stance.ATTACKING);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Attack Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAttackStance()
	{
		StopPlayingAttackAnimation();
		if (IsNotPlayingAnimation(GetAnimationStateHashIDs().AttackStateID))
		{
			FireTowardsPlayer();
			m_TTFireCooldownTimer.Reset();
			SetCurrentStance(Stance.IDLE);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Death Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDeathStance()
	{
		SetLocalPosition( GetLocalPosition() + (Vector3.down * (GetDeltaMovementSpeed() * 8.0f)) );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Full Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateFullPath()
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
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Run End Of Flight Path Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RunEndOfFlightPathCommand()
	{
		// Switch Animation to Idle
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().FlyingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Playing Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartPlayingAttackAnimation()
	{
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().AttackingParamID, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Playing Attack Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopPlayingAttackAnimation()
	{
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().AttackingParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Switch To Corpse Animation State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SwitchToCorpseAnimationState()
	{
		GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().DeadStateParamID, true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentStance(Stance stance)
	{
		m_eCurrentStance = stance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Enemy Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override UnitType GetEnemyType()
	{
		return UnitType.HEAVY_UNIT;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Distance To Ground
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float GetDistanceToGround()
	{
		RaycastHit Hit;
		float fDropDetection = 10000.0f;
		if( Physics.Raycast( GetWorldPosition(), -Vector3.up, out Hit, fDropDetection ) )
		{
			return Hit.distance;
		}

		return fDropDetection;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Die
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Die()
	{
		base.Die();
		m_vDropPosition = (Vector3.down * GetDistanceToGround());
		SetCurrentStance( Stance.DEATH );

		if( m_DeathHissAudioSource != null )
		{
			m_DeathHissAudioSource.Play();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Run Death Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RunDeathCommand()
	{
		// Start Death Animation
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().AliveParamID, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static EnemyFlyingHeavyUnitAnimationHashIDs.AnimationStateHashIDs GetAnimationStateHashIDs()
	{
		return EnemyFlyingHeavyUnitAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static EnemyFlyingHeavyUnitAnimationHashIDs.AnimationParamHashIDs GetAnimationParamHashIDs()
	{
		return EnemyFlyingHeavyUnitAnimationHashIDs.GetParamHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmosSelected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmosSelected()
	{
		if (GameHandler.UnitySceneOnly())
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