//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             BrainBug Animation Hash IDs
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: August 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script Contains the Hash IDs for the Enemy_BrainBug animation states &
//#	  params.
//#	
//#	  This script is referenced in the AI_BrainBug Script as a static script 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class BrainBugAnimationHashIDs 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*{} Class Declarations
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public struct AnimationStateHashIDs
    {
        public int IdleStateID;
        public int AttackingStateID;
        public int DeathStateID;
		public int NonMovingStateID;
    };

    public struct AnimationParamHashIDs
    {
        public int AttackingParamID;
		public int DeathParamID;
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

        StateIDs.IdleStateID		= Animator.StringToHash(    "Base Layer.Idle"       );
        StateIDs.AttackingStateID	= Animator.StringToHash(    "Base Layer.Attacking"  );
        StateIDs.DeathStateID		= Animator.StringToHash(    "Base Layer.Death"      );
		StateIDs.NonMovingStateID	= Animator.StringToHash(	"Base Layer.Non-Moving"	);

		return StateIDs;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup Animation Params Hash IDs
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static AnimationParamHashIDs SetupParamsHashIDs()
    {
		AnimationParamHashIDs ParamIDs;

        ParamIDs.AttackingParamID	= Animator.StringToHash(    "Attacking"	);
		ParamIDs.DeathParamID		= Animator.StringToHash(	"Death"		);

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
