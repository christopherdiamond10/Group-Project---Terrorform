//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Turret Animation Hash IDs
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 15, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script Contains the Hash IDs for the Enemy_Turret animation states &
//	  params.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TurretAnimationHashIDs
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct AnimationStateHashIDs
	{
		public int IdleStateID;
		public int SurfacingStateID;
		public int ShootingStateID;
	};

	public struct AnimationParamHashIDs
	{
		public int SurfacingParamID;
		public int ShootingParamID;
		public int HasSurfacedParamID;
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

		StateIDs.IdleStateID			=	Animator.StringToHash(		"Base Layer.Idle"		);
		StateIDs.SurfacingStateID		=	Animator.StringToHash(		"Base Layer.Surfacing"	);
		StateIDs.ShootingStateID		=	Animator.StringToHash(		"Base Layer.Shooting"	);

		return StateIDs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animation Params Hash IDs
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
	{
		AnimationParamHashIDs ParamIDs;

		ParamIDs.SurfacingParamID		=	Animator.StringToHash(		"Surfacing"		);
		ParamIDs.ShootingParamID		=	Animator.StringToHash(		"Shooting"		);
		ParamIDs.HasSurfacedParamID		=	Animator.StringToHash(		"HasSurfaced"	);
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
