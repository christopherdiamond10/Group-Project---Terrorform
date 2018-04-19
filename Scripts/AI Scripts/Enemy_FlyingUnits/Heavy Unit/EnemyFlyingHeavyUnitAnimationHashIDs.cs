//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Heavy Unit Animation Hash IDs
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: August 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script Contains the Hash IDs for the Enemy_HeavyUnit animation states &
//#		params.
//#	
//#		This script is referenced in the AI_EnemyHeavyUnitBehaviour Script as a 
//#		static script 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class EnemyFlyingHeavyUnitAnimationHashIDs 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct AnimationStateHashIDs
	{
		public int IdleStateID;
		public int FlyingStateID;
		public int AttackStateID;
		public int DeathStateID;
		public int DeadStateID;
	};

	public struct AnimationParamHashIDs
	{
		public int FlyingParamID;
		public int AttackingParamID;
		public int AliveParamID;
		public int DeadStateParamID;
		public int ShootEventParamID;
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs m_StateHashIDs = SetupStateHashIDs();
	private static AnimationParamHashIDs m_ParamHashIDs = SetupParamsHashIDs();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs SetupStateHashIDs()
	{
		AnimationStateHashIDs StateIDs;

		StateIDs.IdleStateID			=	Animator.StringToHash(		"Base Layer.Idle Animation"		);
		StateIDs.FlyingStateID			=	Animator.StringToHash(		"Base Layer.Flying Animation"	);
		StateIDs.AttackStateID			=	Animator.StringToHash(		"Base Layer.Attack Animation"	);
		StateIDs.DeathStateID			=	Animator.StringToHash(		"Base Layer.Death Animation"	);
		StateIDs.DeadStateID			=	Animator.StringToHash(		"Base Layer.Dead"				);

		return StateIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
	{
		AnimationParamHashIDs ParamIDs;

		ParamIDs.FlyingParamID			=	Animator.StringToHash(		"Flying"		);
		ParamIDs.AttackingParamID		=	Animator.StringToHash(		"Attacking"		);
		ParamIDs.AliveParamID			=	Animator.StringToHash(		"Alive"			);
		ParamIDs.DeadStateParamID		=	Animator.StringToHash(		"DeadState"		);
		ParamIDs.ShootEventParamID		=	Animator.StringToHash(		"ShootEvent"	);

		return ParamIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation State Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AnimationStateHashIDs GetStateHashIDs()
	{
		return m_StateHashIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animation Param Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AnimationParamHashIDs GetParamHashIDs()
	{
		return m_ParamHashIDs;
	}
}
