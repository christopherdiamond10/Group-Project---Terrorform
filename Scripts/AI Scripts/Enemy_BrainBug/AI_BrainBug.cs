//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI BrainBug
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: August 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script defines the Behaviour of the AI Brainbug Unit. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class AI_BrainBug : AI_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum Stance
	{
		IDLE,
        ATTACKING,
		DEATH,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fAttackDistance				= 500.0f;		// How Close does the Player need to be?

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Stance      m_eCurrentStance		= Stance.IDLE;	// Current Enemy Stance
	private bool		m_bHasAlreadyAttacked	= false;		// Has not Already Attacked Player


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void Start()
	{
		base.Start();
		SetupInstanceVariables();
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
	//	* New Method: Setup Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupInstanceVariables()
	{
		m_fAttackDistance = (m_fAttackDistance < 0.0001f) ?   500.0f	: m_fAttackDistance;
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

            case Stance.ATTACKING:
            {
                //UpdateAttackingStance();
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
		if (Vector3.Distance(GetPlayerPosition(), GetWorldPosition()) < m_fAttackDistance)		// Move to Attacking Stance if Target is Approaching Within Range
		{
			StartPlayingAttackAnimation();														// Switch Animation To Attack State
            SetCurrentStance( Stance.ATTACKING );												// Switch Stance to this state as well.
		}
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Update Attacking Stance
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void UpdateAttackingStance()
    {
		if( !m_bHasAlreadyAttacked )
		{
			GetPlayerInfoScript().SetBugged( true );
			m_bHasAlreadyAttacked = true;
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Death Stance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDeathStance()
	{
		// Apply Gravity
		transform.position += ( ((GetWorldPosition() + (Vector3.down * 20f)) - GetWorldPosition()).normalized * (25f * Time.deltaTime) );
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Rotate Towards Player
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected override void RotateTowardsPlayer()
    {
        // Only Rotate to Player if Idle
        if (GetCurrentStance() == Stance.IDLE)
        {
            base.RotateTowardsPlayer();
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Playing 'Attack' Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartPlayingAttackAnimation()
	{
		GetAnimatorComponent().SetBool(GetAnimationParamHashIDs().AttackingParamID, true);
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
    //	* New Method: Get Animation State Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static BrainBugAnimationHashIDs.AnimationStateHashIDs GetAnimationStateHashIDs()
    {
        return BrainBugAnimationHashIDs.GetStateHashIDs();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Animation Params Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static BrainBugAnimationHashIDs.AnimationParamHashIDs GetAnimationParamHashIDs()
    {
        return BrainBugAnimationHashIDs.GetParamHashIDs();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Get Enemy Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override UnitType GetEnemyType()
	{
		return UnitType.BRAINBUG;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmosSelected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.1333f, 0.4156f, 0.5019f, 0.5f);	// Dark Blue Colour
		Gizmos.DrawSphere(GetWorldPosition(), m_fAttackDistance);
	}
}
