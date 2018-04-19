//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Explosive Unit Tracker
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script simply contains information on the free areas of the Player.
//#		Should the player be full, no other Explosive unit can attach themselves.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosiveUnitTracker : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool[]  m_abAttachedSides;
	private bool	m_bAllSidesOccupied;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		m_abAttachedSides = new bool[3] { false, false, false };
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		m_bAllSidesOccupied = CheckIfEverySideIsOccupied();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Attached Side
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetAttachedSide( AI_ExplosiveUnit.AttachSide WhichSide, bool Attached )
	{
		int index = (WhichSide == AI_ExplosiveUnit.AttachSide.FRONT) ? 0 : 
					(WhichSide == AI_ExplosiveUnit.AttachSide.LEFT)  ? 1 : 
																	   2 ;

		m_abAttachedSides[index] = Attached;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Attached Sides
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CheckForFreeSide( AI_ExplosiveUnit.AttachSide WhichSide )
	{
		switch (WhichSide)
		{
			case AI_ExplosiveUnit.AttachSide.FRONT:		return !m_abAttachedSides[0];
			case AI_ExplosiveUnit.AttachSide.LEFT:		return !m_abAttachedSides[1];
			case AI_ExplosiveUnit.AttachSide.RIGHT:		return !m_abAttachedSides[2];
			default:									return false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check If Every Side Is Occupied
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool CheckIfEverySideIsOccupied()
	{
		for( int i = 0; i < m_abAttachedSides.Length; ++i )
		{
			if (!m_abAttachedSides[i])
			{
				return false;
			}
		}

		return true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Every Side Occupied?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsEverySideOccupied()
	{
		return m_bAllSidesOccupied;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Any Free Side
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AI_ExplosiveUnit.AttachSide GetAnyFreeSide()
	{
		AI_ExplosiveUnit.AttachSide[] aeSlots = new AI_ExplosiveUnit.AttachSide[3] { AI_ExplosiveUnit.AttachSide.FRONT, AI_ExplosiveUnit.AttachSide.LEFT, AI_ExplosiveUnit.AttachSide.RIGHT };
		
		// Randomising Array Elements, do it twice
		for( int i = 0; i < 2; ++i )
		{
			int iRandomIndexOne									= Random.Range( 0, 3 );
			int iRandomIndexTwo									= Random.Range( 0, 3 );
			AI_ExplosiveUnit.AttachSide eFirstSwapIndexValue	= aeSlots[iRandomIndexOne];

			aeSlots[iRandomIndexOne] = aeSlots[iRandomIndexTwo];
			aeSlots[iRandomIndexTwo] = eFirstSwapIndexValue;
		}
		
		// Go Through Each Element To Determine a Position
		foreach( AI_ExplosiveUnit.AttachSide Side in aeSlots )
		{
			if( CheckForFreeSide(Side) )
			{
				return Side;
			}
		}

		// If All Are Full, Return Unavailable
		return AI_ExplosiveUnit.AttachSide.NOT_ATTACHED;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Closest Side
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AI_ExplosiveUnit.AttachSide GetClosestSide( Transform SelfTransform, Transform TargetTransform )
	{
		float fDistanceFront	= Mathf.Abs((SelfTransform.position - (TargetTransform.transform.position +  TargetTransform.forward)).magnitude);
		float fDistanceLeft		= Mathf.Abs((SelfTransform.position - (TargetTransform.transform.position + -TargetTransform.right)  ).magnitude);
		float fDistanceRight	= Mathf.Abs((SelfTransform.position - (TargetTransform.transform.position +  TargetTransform.right)  ).magnitude);

		// Get Closest Side
		AI_ExplosiveUnit.AttachSide eAttachSide = ((fDistanceFront < fDistanceLeft)  && (fDistanceFront < fDistanceRight)) ? AI_ExplosiveUnit.AttachSide.FRONT	:
												  ((fDistanceLeft  < fDistanceFront) && (fDistanceLeft  < fDistanceRight)) ? AI_ExplosiveUnit.AttachSide.LEFT	:
																															 AI_ExplosiveUnit.AttachSide.RIGHT	;

		// Return Side if it's available, else return N/A
		return (CheckForFreeSide(eAttachSide)) ? eAttachSide : AI_ExplosiveUnit.AttachSide.NOT_ATTACHED;
	}
}
