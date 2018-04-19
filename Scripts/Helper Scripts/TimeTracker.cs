//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Time Tracker
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 1, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script contains a time tracking class which you can use as a timer.
//	  The idea is that you can this script as a way to shorten any code that 
//	  requires a wait timer in actual seconds.
//
//-------------------------------------------------------------------------------
//	Instructions:
//
//	~	Simply create a new instance of this class and call the update function 
//		as needed.
//
//  ~   If you allow this class to add itself to the DynamicUpdateList, you do not 
//      need to call 'Update()', because it will be done automatically.
//
//	~	Check whether or not Time is Up with the 'TimeUp()' function.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TimeTracker
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fCurrentTime = 0.0f;
	public float m_fWaitTimer	= 5.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_ID			        = -1;
	private bool m_bTimeUp		        = false;
    private bool m_bUpdateWhenPaused    = false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Constructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TimeTracker(float TotalWaitTime, bool AddSelfToAutoUpdateList = false, bool UpdateEvenWhenGameIsPaused = false)
	{
		m_fWaitTimer        = TotalWaitTime;
        m_bUpdateWhenPaused = UpdateEvenWhenGameIsPaused;

		if( AddSelfToAutoUpdateList )
        {
			m_ID = DynamicUpdateManager.AddTimeTracker(this);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*~ Deconstructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~TimeTracker()
	{
		DynamicUpdateManager.RemoveTimeTracker( m_ID );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Update Timer
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update()
	{
		if( !TimeUp() )
		{
			m_fCurrentTime += GetDeltaTime();

			if( CheckIfTimeIsUp() )
			{
				m_fCurrentTime = m_fWaitTimer;
				m_bTimeUp = true;
			}
		}
	}

	public void Update(float TimeMultiplier)
	{
		if( !TimeUp() )
		{
			m_fCurrentTime += GetDeltaTime() * TimeMultiplier;

			if( CheckIfTimeIsUp() )
			{
				m_fCurrentTime = m_fWaitTimer;
				m_bTimeUp = true;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check if Time Is Up
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool CheckIfTimeIsUp()
	{
		return (m_fCurrentTime > m_fWaitTimer);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Time Up?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool TimeUp()
	{
		return m_bTimeUp;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Completion Percentage
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float GetCompletionPercentage()
	{
		return (m_fCurrentTime / m_fWaitTimer);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Delta Time
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private float GetDeltaTime()
    {
        return (GameHandler.IsGamePaused() && m_bUpdateWhenPaused) ? DynamicUpdateManager.GetDeltaTime() : Time.deltaTime;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Timer
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		m_fCurrentTime = 0.0f;
		m_bTimeUp = false;
	}
}
