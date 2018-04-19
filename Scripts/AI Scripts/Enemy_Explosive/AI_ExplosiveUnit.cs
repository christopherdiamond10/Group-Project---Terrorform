//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Explosive Unit Behaviour
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script defines the Behaviour of the Explosive Enemy Unit.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class AI_ExplosiveUnit : AI_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum AttackState
	{
		ATTACK_PLAYER,
		PUFF_AND_EXPLODE,
		EXPLODE,
	};
	
	public enum Stance
	{
		IDLE,
		JUMPING,
		ATTACHED,
		EXPLODING,

		PUFF_AND_EXPLODE,
	};

	public enum AttachSide
	{
		NOT_ATTACHED	= 0,
		FRONT			= 1,
		LEFT			= 2,
		RIGHT			= 3,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public	AudioClip	 m_AttachHissSound;
	public	GameObject	 m_goExplosionPrefab;	
	public	float		 m_fJumpVelocity			= 300.0f;

	public  AttackState  m_eAttackState				= AttackState.ATTACK_PLAYER;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AudioSource m_AttachHissSource;
	private Stance		m_eCurrentStance			= Stance.IDLE;
	private AttachSide	m_eAttachedSide				= AttachSide.NOT_ATTACHED;
	private bool		m_bJumped					= false;



	// Event Curve Climax Points
	static float	sm_fJumpEventCurveClimax		= 0.05f;
	static float	sm_fExplosionEventCurveClimax	= 0.05f;

	// Position Offsets For When Attaching to the PlayerShip
	static Vector3	sm_vFrontPositionOffset			= new Vector3(  0.0f,    3.3f,   24.0f );
	static Vector3	sm_vLeftPositionOffset			= new Vector3( -16.3f,  -1.3f,  -0.6f  );
	static Vector3	sm_vRightPositionOffset			= new Vector3(  16.4f,  -1.6f,   1.9f  );
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Start() 
	{
		if( m_eAttackState == AttackState.PUFF_AND_EXPLODE )
		{
            GetAnimatorComponent().SetInteger(GetAnimationParamHashIDs().AttachSideParamID, 1);		// Begin Puff
			SetCurrentStance(Stance.PUFF_AND_EXPLODE);
		}
		else if( m_eAttackState == AttackState.EXPLODE )
		{
			CreateExplosion();
		}

		base.Start();
		SetupInstanceVariables();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupInstanceVariables()
	{
		m_fJumpVelocity = (m_fJumpVelocity < 0.001f) ? 300.0f : m_fJumpVelocity;

		m_AttachHissSource = SoundPlayerManager.AddAudio(gameObject, m_AttachHissSound);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Update() 
	{
		if( m_eAttackState == AttackState.ATTACK_PLAYER )
		{
			base.Update();
		}

		UpdateSelf();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Self
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSelf()
	{
		switch (GetCurrentStance())
		{
			case Stance.IDLE:
			{
				UpdateIdleStance();
				break;
			}

			case Stance.JUMPING:
			{
				UpdateJumpingStance();
				break;
			}

			case Stance.ATTACHED:
			{
				UpdateAttachedStance();
				break;
			}

			case Stance.EXPLODING:
			{
				UpdateExplodingStance();
				break;
			}

			case Stance.PUFF_AND_EXPLODE:
			{
				UpdatePuffAndExplodeStance();
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
		if( !GetExplosiveUnitTrackerInstance().IsEverySideOccupied() )										// If Able to Land on Player?
		{
			float fJumpActivationRange = (m_fJumpVelocity * 1.5f);											// Define a Jump Range
			if( (GetPlayerPosition() - GetWorldPosition()).magnitude < fJumpActivationRange )				// If Close to Target
			{
				SetCurrentStance( Stance.JUMPING );															// Switch To Jumping Stance
				GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().JumpingParamID, true );			// Start 'Jumping' Animation
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Jumping Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJumpingStance()
	{
		if( IsReadyToJump() )																				// If Ready To Jump
		{
			GetComponent<Rigidbody>().AddForce(GetPlayerTargetVector(), ForceMode.Impulse);									// Jump Towards Target
			m_bJumped = true;																				// Not Allowed to Jump Anymore
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Attached Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAttachedStance()
	{
		if( GetPlayerInfoScript().isRolling() )																// If Player Has Performed a Barrel Roll
		{
			GetAnimatorComponent().SetBool( GetAnimationParamHashIDs().PlayerBarrelRolledParamID, true );	// Switch Animations
			GetExplosiveUnitTrackerInstance().SetAttachedSide(m_eAttachedSide, false);						// Allow This Slot to be 'Occupied' by another explosive unit
			transform.parent = null;																		// Remove Parent
			GetPlayerInfoScript().m_uiPlayerScore += (uint)m_iBaseScore;
			SetCurrentStance( Stance.JUMPING );																// Modify Stance back to JUMPING, Where it will not be able to do anything until it is destroyed.
		}

		else if( IsReadyToExplode() )																		// Otherwise if Ready to Explode
		{
			GetExplosiveUnitTrackerInstance().SetAttachedSide(m_eAttachedSide, false);						// Allow This Slot to be 'Occupied' by another explosive unit
			SetCurrentStance( Stance.EXPLODING );															// Move on to the Exploding Stance. "Nothing Can Save You Now, Player ( º ~ º)"
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Exploding
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateExplodingStance()
	{
		CreateExplosion();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update PUFF & EXPLODE Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdatePuffAndExplodeStance()
	{
		if( IsReadyToExplode() )
		{
			CreateExplosion();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Ready To Jump?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsReadyToJump()
	{
		return (	(!m_bJumped)	&&	(GetAnimatorComponent().GetFloat(GetAnimationParamHashIDs().JumpEventParamID) > sm_fJumpEventCurveClimax)	);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Ready To Explode?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsReadyToExplode()
	{
		return (GetAnimatorComponent().GetFloat(GetAnimationParamHashIDs().ExplosionEventParamID) > sm_fExplosionEventCurveClimax);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Attach To Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AttachToTarget()
	{
		// Attempt To Attach To Closest Side
		m_eAttachedSide = GetExplosiveUnitTrackerInstance().GetClosestSide( GetLocalTransform(), GetPlayerObject().transform );

		// If Unsuccessful, Attempt to Attach to any available side
		if (m_eAttachedSide == AttachSide.NOT_ATTACHED)
		{
			m_eAttachedSide = GetExplosiveUnitTrackerInstance().GetAnyFreeSide();
		}

		// If Successful this time around/or Already was Successful
		if (m_eAttachedSide != AttachSide.NOT_ATTACHED)
		{
			transform.parent	= GetPlayerObject().transform;															// Child Self To Target
			GetAnimatorComponent().SetInteger(GetAnimationParamHashIDs().AttachSideParamID, (int)m_eAttachedSide);		// Change Animation To Appropriate Explosion Build-up
			ApplyAttachTransformModifications();																		// Apply Position and Rotation To Match Which Side the AI is located on
			GetExplosiveUnitTrackerInstance().SetAttachedSide( m_eAttachedSide, true );									// Set This Side as 'Occupied', so no other Explosive Unit Can land here
			SetCurrentStance( Stance.ATTACHED );																		// Change Current Stance

			if( m_AttachHissSource != null )
			{
				m_AttachHissSource.Play();
			}
		}

		// These Commands will be used regardless of whether or not the Unit was Successful in Attaching to a Target.
		GetComponent<Collider>().enabled = false;																						// Don't Collide With Things Anymore
		GetComponent<Rigidbody>().Sleep();																								// Stop Physics on Self
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Apply Transform Modifications (When Attaching)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ApplyAttachTransformModifications()
	{
		switch (m_eAttachedSide)
		{
			case AttachSide.FRONT:
			{
				// Set New Position
				Vector3 vNewPosition = Vector3.Scale(sm_vFrontPositionOffset, Vector3.Scale(GetScale(), GetPlayerObject().transform.localScale));
				SetLocalPosition(vNewPosition);

				// Set New Rotation
				Quaternion qNewRotation = GetPlayerObject().transform.rotation;
				SetRotation(qNewRotation);
				break;
			}

			case AttachSide.LEFT:
			{
				Vector3 vNewPosition = Vector3.Scale(sm_vLeftPositionOffset, Vector3.Scale(GetScale(), GetPlayerObject().transform.localScale));
				SetLocalPosition(vNewPosition);

				Quaternion qNewRotation = GetPlayerObject().transform.rotation;
				qNewRotation.y -= 1.0f;
				SetRotation( qNewRotation );
				break;
			}

			case AttachSide.RIGHT:
			{
				Vector3 vNewPosition = Vector3.Scale(sm_vRightPositionOffset, Vector3.Scale(GetScale(), GetPlayerObject().transform.localScale));
				SetLocalPosition(vNewPosition);

				Quaternion qNewRotation = GetPlayerObject().transform.rotation;
				qNewRotation.y += 1.0f;
				SetRotation( qNewRotation );
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Explosion
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateExplosion()
	{
		GameObject NewExplosion = Instantiate(m_goExplosionPrefab, GetWorldPosition(), GetWorldRotation()) as GameObject;
		
		if( transform.parent.gameObject == GetPlayerObject() )
		{
			ExplosionHugScript ExplosionHugComponent	= NewExplosion.AddComponent<ExplosionHugScript>();
			ExplosionHugComponent.m_goObjectToHug		= GetPlayerObject();
			ExplosionHugComponent.m_v3Direction			= GetPlayerObject().transform.forward;

			GetPlayerInfoScript().TakeDamage( m_fOutputDamage );
		}

		DestroySelf();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Rotate Towards Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RotateTowardsPlayer()
	{
		if (GetCurrentStance() == Stance.IDLE)
		{
			base.RotateTowardsPlayer();
		}
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
		return UnitType.EXPLOSIVE;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Explosive Unit Tracker Instance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ExplosiveUnitTracker GetExplosiveUnitTrackerInstance()
	{
		return GetPlayerObject().GetComponent< ExplosiveUnitTracker >();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Collision Trigger Enter
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject == GetPlayerObject())
		{
			OnContactWithPlayer();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Contact With Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnContactWithPlayer()
	{
		if( GetCurrentStance() == Stance.IDLE || GetCurrentStance() == Stance.PUFF_AND_EXPLODE )
		{
			base.OnContactWithPlayer();
		}
		else
		{
			AttachToTarget();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static ExplosiveUnitAnimationHashIDs.AnimationStateHashIDs GetAnimationStateHashIDs()
	{
		return ExplosiveUnitAnimationHashIDs.GetStateHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static ExplosiveUnitAnimationHashIDs.AnimationParamHashIDs GetAnimationParamHashIDs()
	{
		return ExplosiveUnitAnimationHashIDs.GetParamHashIDs();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmosSelected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.8f, 0.0f, 0.0f, 0.5f);
		Gizmos.DrawSphere(GetWorldPosition(), m_fJumpVelocity);
	}
}
