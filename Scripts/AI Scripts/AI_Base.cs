//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Base
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: June 14, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script is the Base Class for All AI's. This Script Contains the Following:
//#     * AI Position
//#		* Damage Ratios
//#     * Status (Alive|Dead)
//#		* Unit Type
//#		* Flash Effect (When Hit, Flashes Red)
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AI_Base : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum UnitType
	{
		EXPLOSIVE,
		TURRET,
		BRAINBUG,
		LIGHT_UNIT,
		HEAVY_UNIT
	};

	protected struct MaterialHolder
	{
		public Renderer		MaterialRenderer;
		public Color		MaterialOriginalColour;
	};

	protected struct FlashEffectStruct
	{
		public bool					 bFlashEffectActive;			// Is Flash Effect Turned on?
		public bool					 bDisplayingFlash;				// Is Attacked Flash Currently being Displayed? (Either Normal Model or Flash Effect)
		public Color				 cFlashEffectColour;			// The Colour For the Flash Effect
		public TimeTracker			 TTFlashEffectLengthTimer;		// The Period the Effect Lasts
		public TimeTracker			 TTFlashEffectSwitchTimer;		// The Time it takes to switch between Normal and Damage colour (prevents seizures)
		public List<MaterialHolder>  lMaterialsList;				// List that Holds the Entire Materials Selection for this GameObject
	};
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int					 m_iHealth              = 10;		// Remaining Health
	public float				 m_fOutputDamage		= 5.0f;		// What Damage Will I Cause With My Main Attack?
	public float				 m_fCollisionDamage		= 10.0f;	// What Damage Will I Cause if I Collide Directly With the Player?
	public float				 m_fRotationSpeed		= 2.0f;		// My SLERP Rotation Speed
	public int					 m_iBaseScore			= 10;		// Score Received When I Die?

    public int                   m_iChanceToHitPlayer   = 100;      // What Chance Do We Have To Hit The Player (Out of 100)?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*. Protected Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected int                m_ID					= -1;		// Unique Enemy ID (used for AI_Manager)
    protected int				 m_iCurrentHealth;					// Current Health
	protected FlashEffectStruct	 m_FlashEffectStruct;				// Flash Effect Instance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Awake            (Constructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Awake()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Destroy     (Deconstructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnDestroy()
	{
		AI_Manager.DestroyElement( m_ID );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public virtual void Start()
    {
		SetupDamageValues();
		SetupRotationSpeed();
		SetupHealth();
        SetupPlayerHitChance();
		SetupFlashEffect();
        AddSelfToAIManager();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Damage Values
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetupDamageValues()
	{
		m_fOutputDamage		= (m_fOutputDamage		< 0.001f)	? 5.0f	: m_fOutputDamage;
		m_fCollisionDamage	= (m_fCollisionDamage	< 0.001f)	? 10.0f : m_fCollisionDamage;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Rotation Speed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetupRotationSpeed()
	{
		m_fRotationSpeed = (m_fRotationSpeed < 0.001f) ? 2.0f : m_fRotationSpeed;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Health
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetupHealth()
	{
        m_iCurrentHealth = m_iHealth;
		
		if( Get_IsDead() )
		{
			m_iHealth = 10;
			SetupHealth();
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup Chance To Hit Player
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected virtual void SetupPlayerHitChance()
    {
        m_iChanceToHitPlayer = (m_iChanceToHitPlayer < 1 || m_iChanceToHitPlayer > 100) ? 90 : m_iChanceToHitPlayer;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void SetupFlashEffect()
	{
		int	  iTotalFlashes								= 15;
		float fTotalEffectTime							= 3.0f;
		float fTimeUntilSwitch							= (fTotalEffectTime / iTotalFlashes);

		m_FlashEffectStruct.bFlashEffectActive			= false;
		m_FlashEffectStruct.bDisplayingFlash			= false;
		m_FlashEffectStruct.cFlashEffectColour			= Color.red;
		m_FlashEffectStruct.TTFlashEffectLengthTimer	= new TimeTracker(fTotalEffectTime, false);
		m_FlashEffectStruct.TTFlashEffectSwitchTimer	= new TimeTracker(fTimeUntilSwitch, false);
		m_FlashEffectStruct.lMaterialsList				= new List<MaterialHolder>();

		ReceiveAllMaterials( transform );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Receive All Materials
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ReceiveAllMaterials( Transform CurrentChild )
	{
		foreach (Transform Child in CurrentChild)
		{
			ReceiveAllMaterials(Child);
		}

		if (CurrentChild.GetComponent<Renderer>() != null)
		{
			MaterialHolder MH			= new MaterialHolder();
			MH.MaterialRenderer			= CurrentChild.GetComponent<Renderer>();
			MH.MaterialOriginalColour	= CurrentChild.GetComponent<Renderer>().sharedMaterial.color;
			m_FlashEffectStruct.lMaterialsList.Add(MH);
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Add Self to AI Manager
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void AddSelfToAIManager()
    {
        AI_Manager.SetupNewAI( this );
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Update
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public virtual void Update()
    {
		UpdateFlashEffect();

		// If Alive, Rotate Towards Player
		if (Get_IsAlive())
		{
			RotateTowardsPlayer();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateFlashEffect()
	{
		// If Flash Effect Active
		if (m_FlashEffectStruct.bFlashEffectActive )
		{
			// Update TimeTrackers
			m_FlashEffectStruct.TTFlashEffectLengthTimer.Update();
			m_FlashEffectStruct.TTFlashEffectSwitchTimer.Update();

			// If Flash Effect Should Transition To Next Colour
			if( m_FlashEffectStruct.TTFlashEffectSwitchTimer.TimeUp() )
			{
				m_FlashEffectStruct.TTFlashEffectSwitchTimer.Reset();
				m_FlashEffectStruct.bDisplayingFlash = !m_FlashEffectStruct.bDisplayingFlash;

				if( m_FlashEffectStruct.bDisplayingFlash )
				{
					SetMaterialColours(m_FlashEffectStruct.cFlashEffectColour);
				}
				else
				{
					RevertMaterialColours();
				}
			}

			// If Flash Effect is Over
			if( m_FlashEffectStruct.TTFlashEffectLengthTimer.TimeUp() )
			{
				TurnOffFlashEffect();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Turn On Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void TurnOnFlashEffect()
	{
		m_FlashEffectStruct.bFlashEffectActive = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Turn Off Flash Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void TurnOffFlashEffect()
	{
		if( m_FlashEffectStruct.bFlashEffectActive )
		{
			m_FlashEffectStruct.bFlashEffectActive	= false;
			m_FlashEffectStruct.bDisplayingFlash	= false;

			m_FlashEffectStruct.TTFlashEffectLengthTimer.Reset();
			m_FlashEffectStruct.TTFlashEffectSwitchTimer.Reset();
			RevertMaterialColours();
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Rotate Towards Player
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void RotateTowardsPlayer()
    {
		Quaternion LookRotation = Quaternion.LookRotation( GetPlayerPosition() - GetWorldPosition(), Vector3.up );
		
		SetRotation( Quaternion.Slerp(GetWorldRotation(), LookRotation, m_fRotationSpeed * Time.deltaTime) );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Die
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void Die()
	{
		m_iCurrentHealth = 0;
		TurnOffFlashEffect();
		RunDeathCommand();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Playing Animation?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsPlayingAnimation(string AnimationName, bool CanBeTransitioning = false)
	{
		return IsPlayingAnimation( Animator.StringToHash( AnimationName ), CanBeTransitioning );
	}

	public bool IsPlayingAnimation(int AnimationHashID, bool CanBeTransitioning = false)
	{
		bool Transitioning = (CanBeTransitioning) ? false : IsAnimationTransitioning();
		return ( !Transitioning && GetAnimatorComponent().GetCurrentAnimatorStateInfo(0).nameHash == AnimationHashID );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Not Playing Animation?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsNotPlayingAnimation(string AnimationName, bool CanBeTransitioning = false)
	{
		return IsNotPlayingAnimation( Animator.StringToHash( AnimationName ), CanBeTransitioning );
	}

	public bool IsNotPlayingAnimation(int AnimationHashID, bool CanBeTransitioning = false)
	{
		bool Transitioning = (CanBeTransitioning) ? false : IsAnimationTransitioning();
		return ( !Transitioning && GetAnimatorComponent().GetCurrentAnimatorStateInfo(0).nameHash != AnimationHashID );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Animation Is Transitioning?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsAnimationTransitioning()
	{
		return ( GetAnimatorComponent().IsInTransition(0) );
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set AI ID
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void SetID( int ID, bool ForceAllow = false )
    {
        // Only Change ID if not Already Assigned, Or only if Forced
        if( (m_ID == -1) || ForceAllow )
        {
            m_ID = ID;
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Revert Material Colours
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RevertMaterialColours()
	{
		foreach (MaterialHolder MH in m_FlashEffectStruct.lMaterialsList)
		{
			MH.MaterialRenderer.material.color = MH.MaterialOriginalColour;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Material Colour
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetMaterialColours( Color Colour )
	{
		foreach( MaterialHolder MH in m_FlashEffectStruct.lMaterialsList )
		{
			MH.MaterialRenderer.material.color = Colour;
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Set Position
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetLocalPosition(Vector3 Position)
	{
		GetLocalTransform().localPosition = Position;
	}

    public void SetWorldPosition( Vector3 Position )
    {
		GetLocalTransform().position = Position;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Set Rotation
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetRotation(Vector3 Rotation)
	{
		Quaternion NewRotation = new Quaternion(Rotation.x, Rotation.y, Rotation.z, 1.0f);
		GetLocalTransform().localRotation = NewRotation;
	}

	public void SetRotation(Quaternion Rotation)
    {
		GetLocalTransform().localRotation = Rotation;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set 'Look At' Rotation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetLookAtRotation(Vector3 Target)
	{
		GetLocalTransform().LookAt(Target);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Enemy Type
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual UnitType GetEnemyType()
	{
		return 0;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get AI ID
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int GetID()
	{
		return m_ID;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is An Enemy?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Get_IsEnemy()
	{
		return true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Active
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Get_IsActive()
	{
		return gameObject.activeSelf;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Alive?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Get_IsAlive()
	{
		return (m_iCurrentHealth > 0);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Dead?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Get_IsDead()
	{
		return !Get_IsAlive();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Local Transform
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Transform GetLocalTransform()
	{
		return transform;
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Get Position
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetLocalPosition()
	{
		return GetLocalTransform().localPosition;
	}

	public Vector3 GetWorldPosition()
	{
		return GetLocalTransform().position;
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Get Rotation
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Quaternion GetLocalRotation()
	{
		return GetLocalTransform().localRotation;
	}

	public Quaternion GetWorldRotation()
	{
		return GetLocalTransform().rotation;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Scale
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetScale()
	{
		return GetLocalTransform().localScale;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Up Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetUpVector()
	{
		return GetLocalTransform().up;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Down Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetDownVector()
	{
		return -GetUpVector();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Forward Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetForwardVector()
	{
		return GetLocalTransform().forward;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Backward Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetBackwardVector()
	{
		return -GetForwardVector();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Right Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetRightVector()
	{
		return GetLocalTransform().right;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Left Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetLeftVector()
	{
		return -GetRightVector();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Self Animator
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected Animator GetAnimatorComponent()
	{
		return GetComponent<Animator>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Chance To Hit Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected int GetChanceToHitPlayer()
	{
		SetupPlayerHitChance();
		return (101 - m_iChanceToHitPlayer);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Player Object
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected GameObject GetPlayerObject()
    {
        return GameObjectTrackingManager.m_goPlayer;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Player Info Script
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected PlayerControls GetPlayerInfoScript()
    {
        return GetPlayerObject().GetComponent< PlayerControls >();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Player Position (This Method Only Exists to Keep Neater Code)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected Vector3 GetPlayerPosition()
    {
        return GetPlayerObject().transform.position;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Player Velocity
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected Vector3 GetPlayerVelocityVector()
    {
        return (GetPlayerObject().transform.forward * GetPlayerInfoScript().m_fSpeed);
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Player Intercept Position
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected Vector3 GetPlayerInterceptPosition(float ProjectileSpeed = 1200.0f)
    {
		return GameObjectTrackingManager.CalculateInterceptCourse( GetPlayerPosition(), GetPlayerVelocityVector(), GetWorldPosition(), ProjectileSpeed );
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Target Position Near or Directly at Player 
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected virtual Vector3 GetPlayerTargetVector(float ProjectileSpeed = 1200.0f)
    {
        Vector3 v3TargetPosition	= GetPlayerInterceptPosition(ProjectileSpeed);
		int iPlayerHitChance		= GetChanceToHitPlayer();

		if (iPlayerHitChance == 1) // Always Hit
        {
            return v3TargetPosition;
        }

        // Get Either UpVector or RightVector Offset
        Vector3 v3DirectionalOffset = (Random.Range(0, 2) == 0) ? GetRightVector() : GetUpVector();

		// Swap Direction?
        if ((Random.Range(0, 2) == 0))
        {
            v3DirectionalOffset = -v3DirectionalOffset;
        }
        
        // Apply Offset with Random Miss Range
        v3TargetPosition += (v3DirectionalOffset * Random.Range(0, iPlayerHitChance));
        return v3TargetPosition;
    }


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnCollisionEnter(Collision collision)
	{
		if( Get_IsAlive() )
		{
			switch (collision.transform.tag)
			{
				case StaticKeywordsScript.PlayerColliderTag:
				{
					OnContactWithPlayer();
					break;
				}

				case StaticKeywordsScript.PlayerTag:
				{
					OnContactWithPlayer();
					break;
				}

				case StaticKeywordsScript.PlayerBulletsTag:
				{
					OnContactWithBullet(collision.gameObject);
					break;
				}

				default:
				{
					break;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Contact With Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnContactWithPlayer()
	{
		GetPlayerInfoScript().TakeDamage( m_fCollisionDamage );
		GetPlayerInfoScript().m_uiPlayerScore -= ((int)GetPlayerInfoScript().m_uiPlayerScore - m_iBaseScore) < 0 ? GetPlayerInfoScript().m_uiPlayerScore : (uint)m_iBaseScore;
		Die();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Contact With Bullet
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnContactWithBullet(GameObject Bullet)
	{
        m_iCurrentHealth -= (int)Bullet.GetComponent<BulletScript>().GetDamage();
		TurnOnFlashEffect();
		CheckHealth();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Health
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void CheckHealth()
	{
        if ( Get_IsDead() )
        {
			GetPlayerInfoScript().m_uiPlayerScore += (uint)m_iBaseScore;
			Die();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Has Finished Death Animation?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual bool HasFinishedDeathAnimation()
	{
		return true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Run Death Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void RunDeathCommand()
	{
		DestroySelf();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Destroy Enemy       (Public Version)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void KillMe(float Time = 0.15f)
	{
        DestroySelf(Time);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Destroy Self     (Protected Version)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DestroySelf()
	{
		DestroySelf(0.0f);
	}

    protected virtual void DestroySelf( float Time )
    {
        DestroyObject(gameObject, Time);
    }
}
