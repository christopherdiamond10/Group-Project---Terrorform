//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Explsove Unit Animation Hash IDs
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: August 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script Contains the Hash IDs for the Enemy_Explosive animation states &
//#	  params.
//#	
//#	  This script is referenced in the AI_Explosive Script as a static script 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ExplosiveUnitAnimationHashIDs
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*{} Class Declarations
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public struct AnimationStateHashIDs
    {
        public int IdleStateID;
        public int JumpStateID;
        public int AttackFrontStateID;
		public int AttackRightStateID;
		public int RollOffPlayerStateID;

    };

    public struct AnimationParamHashIDs
    {
        public int JumpingParamID;
		public int AttachSideParamID;
		public int PlayerBarrelRolledParamID;
		public int JumpEventParamID;
		public int ExplosionEventParamID;
    };
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static AnimationStateHashIDs m_StateHashIDs = SetupStateHashIDs();
	static AnimationParamHashIDs m_ParamHashIDs = SetupParamsHashIDs();
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup Animation State Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationStateHashIDs SetupStateHashIDs()
    {
		AnimationStateHashIDs StateIDs;

        StateIDs.IdleStateID			= Animator.StringToHash(    "Base Layer.Idle"				);
        StateIDs.JumpStateID			= Animator.StringToHash(    "Base Layer.Jump"				);
        StateIDs.AttackFrontStateID		= Animator.StringToHash(    "Base Layer.Attack Front"       );
		StateIDs.AttackRightStateID		= Animator.StringToHash(	"Base Layer.Attack Left/Right"	);
		StateIDs.RollOffPlayerStateID	= Animator.StringToHash(	"Base Layer.Roll-off Player"	);

		return StateIDs;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup Animation Params Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
    {
		AnimationParamHashIDs ParamIDs;

        ParamIDs.JumpingParamID				= Animator.StringToHash(    "Jumping"			 );
		ParamIDs.AttachSideParamID			= Animator.StringToHash(	"AttachSide"		 );
		ParamIDs.PlayerBarrelRolledParamID	= Animator.StringToHash(	"PlayerBarrelRolled" );
		ParamIDs.JumpEventParamID			= Animator.StringToHash(	"JumpEvent"			 );
		ParamIDs.ExplosionEventParamID		= Animator.StringToHash(	"ExplosionEvent"	 );

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