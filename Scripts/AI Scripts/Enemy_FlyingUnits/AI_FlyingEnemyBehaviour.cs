//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Flying Enemy Behaviour
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: June 15, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script Contains the Base Behaviour of Flying Enemies, including 'light', 
//#	  'medium' & 'heavy' units.
//#	  The different unit types excel in different areas such as speed, 
//#	  damage production & damage reduction.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AI_FlyingEnemyBehaviour : AI_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum FlightDirection
	{
		UP,
		UP_LEFT,
		UP_RIGHT,
		FORWARD,
		FORWARD_UP,
		FORWARD_UP_LEFT,
		FORWARD_UP_RIGHT,
		FORWARD_DOWN,
		FORWARD_DOWN_LEFT,
		FORWARD_DOWN_RIGHT,
		DOWN,
		DOWN_LEFT,
		DOWN_RIGHT,
		LEFT,
		RIGHT,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fMovementSpeed			= 50.0f;					// My Movement Speed
	public float m_fBulletSpeed				= 1200.0f;					// How Quick Do My Bullets Fire?
	public float m_fBulletCooldownTime		= 5.0f;						// How Long do I need to Wait Before I can Fire Again?
	public float m_fDirectionMovementOffset = 800.0f;					// How Far do I Move Left & Right?

	public int			m_iSmoothnessRating	= 50;						// How Smooth Is My Path?
	public FlightDirection[] m_PathPoints;								// Public Path Selection

    public GameObject m_goBullet;										// The Prefab for this Unit to Fire towards Player.
	public GameObject m_goUnitHead;										// The Head of the AI Unit. The Bullet Fires from Here
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*. Protected Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected TimeTracker						m_TTFireCooldownTimer;	// The TimeTracker for Keeping Track of when to Fire at Target.
	protected ArrayElementTracker<Vector3>		m_FlightPath;			// Flight Path if not following Player
	protected Vector3							m_vDropPosition;		// This is Where the Ground is When/If I am Killed and Gravity needs to do away with me.
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Start()
    {
        base.Start();
		SetupInstanceVariables();
		SetupAttackCooldownTime();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupInstanceVariables()
	{
		m_fMovementSpeed			= (m_fMovementSpeed				<= 0.0f)								?   50.0f	 :   m_fMovementSpeed;
		m_fBulletSpeed				= (m_fBulletSpeed				<= 0.0f)								?   1200.0f	 :   m_fBulletSpeed;
		m_fBulletCooldownTime		= (m_fBulletCooldownTime		<= 0.0f)								?   5.0f	 :   m_fBulletCooldownTime;
		m_fDirectionMovementOffset	= (m_fDirectionMovementOffset	<= 0.0f)								?   800.0f   :   m_fDirectionMovementOffset;
		m_iSmoothnessRating			= ((m_iSmoothnessRating			<= 0) || (m_iSmoothnessRating > 100))	?	50		 :	 m_iSmoothnessRating;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Attack Cooldown Time
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupAttackCooldownTime()
	{
		m_TTFireCooldownTimer					= new TimeTracker(m_fBulletCooldownTime, false);

		// Get Enemy Cooldown Timers Offsetted so they don't all shoot at the exact same time
		int iMaxTime							= (int)(m_fBulletCooldownTime * 100.0f);
		float fNewTime							= Random.Range(0, iMaxTime) * 0.01f;
		m_TTFireCooldownTimer.m_fCurrentTime	= fNewTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Rotate Towards Player 
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void RotateTowardsPlayer()
	{
		SetLookAtRotation( GetPlayerPosition() );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void UpdateMovement()
	{
		// If Finished Path, DestroySelf
		if (m_FlightPath.AtEndOfArray())
		{
			RunEndOfFlightPathCommand();
		}
		else
		{
			MoveTowardsTarget();

			if (IsCloseEnoughToTargetPosition())
			{
				// Move on to the Next Path Node
				m_FlightPath.IncrementElement();
			}
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fire Towards Player
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected virtual void FireTowardsPlayer()
    {
		// Create Projectile
        GameObject Projectile						= Instantiate(m_goBullet, m_goUnitHead.transform.position, Quaternion.identity) as GameObject;
		BasicBulletMovementScript ProjectileScript	= Projectile.GetComponent< BasicBulletMovementScript >();

		// Face it Towards the Target Vector
		Projectile.transform.LookAt( GetPlayerPosition() );

		// Setup Projectile Information
		ProjectileScript.SetImpactDamage( m_fOutputDamage );
		ProjectileScript.SetMovementSpeed( m_fBulletSpeed );
		ProjectileScript.SetExpectedTravelDistance( m_fBulletSpeed * 2.0f );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Run End Of Flight Path Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void RunEndOfFlightPathCommand()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move Towards Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void MoveTowardsTarget()
	{
		Vector3 vNewPosition = (GetLocalPosition() + ((GetTargetPosition() - GetLocalPosition()).normalized) * GetDeltaMovementSpeed());
		SetLocalPosition(vNewPosition);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Am I Close Enough To My Target Position?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool IsCloseEnoughToTargetPosition()
	{
		return (Vector3.Distance(GetLocalPosition(), GetTargetPosition()) < GetDeltaMovementSpeed());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Target Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual Vector3 GetTargetPosition()
	{
		return m_FlightPath.GetCurrentElement();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Delta Movement Speed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual float GetDeltaMovementSpeed()
	{
		return (m_fMovementSpeed * Time.deltaTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Direction Vector
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected Vector3 GetDirectionPathPoint(FlightDirection eDirection)
	{
		switch (eDirection)
		{
			case FlightDirection.UP:					{	return  GetUpVector();												}
			case FlightDirection.UP_LEFT:				{	return	GetUpVector()		+ GetLeftVector();						}
			case FlightDirection.UP_RIGHT:				{	return	GetUpVector()		+ GetRightVector();						}
			case FlightDirection.FORWARD:				{	return	GetForwardVector();											}
			case FlightDirection.FORWARD_UP:			{	return	GetForwardVector()	+ GetUpVector();						}
			case FlightDirection.FORWARD_UP_LEFT:		{	return	GetForwardVector()	+ GetUpVector()		+ GetLeftVector();	}
			case FlightDirection.FORWARD_UP_RIGHT:		{	return	GetForwardVector()	+ GetUpVector()		+ GetRightVector(); }
			case FlightDirection.FORWARD_DOWN:			{	return	GetForwardVector()	+ GetDownVector();						}
			case FlightDirection.FORWARD_DOWN_LEFT:		{	return	GetForwardVector()	+ GetDownVector()	+ GetLeftVector();	}
			case FlightDirection.FORWARD_DOWN_RIGHT:	{	return	GetForwardVector()	+ GetDownVector()	+ GetRightVector(); }
			case FlightDirection.DOWN:					{	return	GetDownVector();											}
			case FlightDirection.DOWN_LEFT:				{	return	GetDownVector()		+ GetLeftVector();						}
			case FlightDirection.DOWN_RIGHT:			{	return	GetDownVector()		+ GetRightVector();						}
			case FlightDirection.LEFT:					{	return	GetLeftVector();											}
			case FlightDirection.RIGHT:					{	return  GetRightVector();											}
			default:									{	return  Vector3.zero;												}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Straight Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void CreateStraightPath()
	{
		// Create Nodes
		List<Vector3> lPointsList = new List<Vector3>();
		Vector3 vCurrentPathPosition = GetWorldPosition();

		for (int i = 0; i < m_PathPoints.Length; ++i)
		{
			vCurrentPathPosition += GetDirectionPathPoint(m_PathPoints[i]) * m_fDirectionMovementOffset;
			lPointsList.Add(vCurrentPathPosition);
		}

		m_FlightPath = new ArrayElementTracker<Vector3>(lPointsList.Count);
		int iCurrentArrayElement = 0;
		foreach (Vector3 Node in lPointsList)
		{
			m_FlightPath[iCurrentArrayElement] = Node;
			iCurrentArrayElement += 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Bezier Curves Path
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void CreateBezierCurvesPath()
	{
		// Create Nodes
		List<Vector3> lPointsList = new List<Vector3>();
		Vector3 vCurrentPathPosition = GetWorldPosition();

		for (int i = 0; i < m_PathPoints.Length; ++i)
		{
			vCurrentPathPosition += GetDirectionPathPoint(m_PathPoints[i]) * m_fDirectionMovementOffset;
			lPointsList.Add(vCurrentPathPosition);
		}

		int iProperSmothnessRating = (101 - ((m_iSmoothnessRating > 0 && m_iSmoothnessRating < 101) ? m_iSmoothnessRating : 50));
		float fSmoothnessPercentile = ((float)iProperSmothnessRating * 0.01f);												// Smoothness Percent
		lPointsList.Insert(0, GetWorldPosition());																			// First Point is SelfPosition 
		List<Vector3> lBezierPointsList = GetBezierPoints(fSmoothnessPercentile, lPointsList);								// Get Bezier Curving

		m_FlightPath = new ArrayElementTracker<Vector3>(lBezierPointsList.Count);
		int iCurrentArrayElement = 0;
		foreach (Vector3 Node in lBezierPointsList)
		{
			m_FlightPath[iCurrentArrayElement] = Node;
			iCurrentArrayElement += 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get Flight Path Colour
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected Color GetFlightPathColour()
	{
		return new Color(0.658f, 0.235f, 0.560f);			// Purplish Colour
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Bezier Curving
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float cx = 3 * (p1.x - p0.x);
        float cy = 3 * (p1.y - p0.y);
		float cz = 3 * (p1.z - p0.z);

        float bx = 3 * (p2.x - p1.x) - cx;
        float by = 3 * (p2.y - p1.y) - cy;
		float bz = 3 * (p2.z - p1.z) - cz;

        float ax = p3.x - p0.x - cx - bx;
        float ay = p3.y - p0.y - cy - by;
		float az = p3.z - p0.z - cz - bz;

        float Cube = t * t * t;
        float Square = t * t;

        float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.x;
        float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.y;
		float resZ = (az * Cube) + (bz * Square) + (cz * t) + p0.z;

        return new Vector3(resX, resY, resZ);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Bezier Curving
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static List<Vector3> GetBezierPoints(float Smoothness, List<Vector3> lPointsList)
	{
		List<Vector3> lNewBezierPointsList = new List<Vector3>();

		const int iNecessaryPoints = 4;
		for (int i = 0; i < lPointsList.Count; ++i)
		{
			if ((lPointsList.Count - i) > iNecessaryPoints)
			{
				Vector3 P0 = lPointsList[i];
				Vector3 P1 = lPointsList[i + 1];
				Vector3 P2 = lPointsList[i + 2];
				Vector3 P3 = lPointsList[i + 3];
				for (float t = 0.0f; t < 1.0f; t += Smoothness)
				{
					lNewBezierPointsList.Add(GetBezierPoint(t, P0, P1, P2, P3));
				}

				i += 2; // Skip Already Calculated Points
			}
			else
			{
				lNewBezierPointsList.Add( lPointsList[i] );
			}
		}

		return lNewBezierPointsList;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Bezier Curving
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static Vector3 GetBezierPoint(float t, List<Vector3> lPointsList)
	{
		if (lPointsList.Count == 1)
		{
			return lPointsList[0];
		}

		float[] aCurvePointsX = new float[lPointsList.Count - 1];
		float[] aCurvePointsY = new float[lPointsList.Count - 1];
		float[] aCurvePointsZ = new float[lPointsList.Count - 1];

		Vector3 vThisPoint = lPointsList[1];
		Vector3 vLastPoint = lPointsList[0];

		for( int i = 0; (i+1) < lPointsList.Count; ++i )
		{
			aCurvePointsX[i] = lPointsList.Count * (vThisPoint.x - vLastPoint.x);
			aCurvePointsY[i] = lPointsList.Count * (vThisPoint.y - vLastPoint.y);
			aCurvePointsZ[i] = lPointsList.Count * (vThisPoint.z - vLastPoint.z);

			if( (i-1) > -1)
			{
				aCurvePointsX[i] -= aCurvePointsX[(i - 1)];
				aCurvePointsY[i] -= aCurvePointsY[(i - 1)];
				aCurvePointsZ[i] -= aCurvePointsZ[(i - 1)];
			}

			vLastPoint = vThisPoint;
			vThisPoint = lPointsList[i + 1];
		}

		float resX = 0.0f, resY = 0.0f, resZ = 0.0f;
		for (int j = (aCurvePointsX.Length - 1); j != 0; --j)
		{
			float fPower = Mathf.Pow(t, j);
			resX += aCurvePointsX[j] * fPower;
			resY += aCurvePointsY[j] * fPower;
			resZ += aCurvePointsZ[j] * fPower;
		}
		resX += lPointsList[0].x;
		resY += lPointsList[0].y;
		resZ += lPointsList[0].z;

		return new Vector3(resX, resY, resZ);
	}
}