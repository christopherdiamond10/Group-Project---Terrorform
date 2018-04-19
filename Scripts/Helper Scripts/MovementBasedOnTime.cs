using UnityEngine;
using System.Collections;

public class MovementBasedOnTime
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3		m_vStartPosition;
	private Vector3		m_vTargetPosition;
	private Vector3		m_vCurrentPosition;
	private TimeTracker m_TTMovementTime;
	private int			m_ID = -1;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Constructors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MovementBasedOnTime()
	{
		m_TTMovementTime = new TimeTracker(0.0f, false);
	}

	public MovementBasedOnTime(Vector3 StartPosition, Vector3 EndPosition, float Seconds, bool AddSelfToList = false, bool UpdateEvenIfGameIsPaused = false)
	{
        SetupMovement(StartPosition, EndPosition, Seconds, UpdateEvenIfGameIsPaused);

		if( AddSelfToList )
		{
			m_ID = DynamicUpdateManager.AddMovementBasedOnTime( this );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*~ Deconstructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~MovementBasedOnTime()
	{
		DynamicUpdateManager.RemoveMovementBasedOnTime( m_ID );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void SetupMovement(Vector3 StartPosition, Vector3 EndPosition, float Seconds, bool UpdateEvenIfGameIsPaused = false)
	{
		m_vStartPosition	= StartPosition;
		m_vTargetPosition	= EndPosition;
		m_TTMovementTime	= new TimeTracker(Seconds, false, UpdateEvenIfGameIsPaused);

		Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update()
	{
		GetTimeInstance().Update();

		m_vCurrentPosition = m_vStartPosition + ((m_vTargetPosition - m_vStartPosition) * GetTimeInstance().GetCompletionPercentage());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetCurrentPosition()
	{
		return m_vCurrentPosition;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Has Reached Destination
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool HasReachedDestination()
	{
		return GetTimeInstance().TimeUp();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get TimeTracker Instance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TimeTracker GetTimeInstance()
	{
		return m_TTMovementTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		GetTimeInstance().Reset();
		m_vCurrentPosition = m_vStartPosition;
	}
}
