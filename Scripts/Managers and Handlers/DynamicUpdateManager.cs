using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicUpdateManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{}	Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public struct ElementHolder<T>
	{
		public int			ID;
		public T			Instance;
	};


	public class TemplateList<T>
	{
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*+ Public Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public List<ElementHolder<T>> ClassList = new List<ElementHolder<T>>();
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	*- Private Instance Variables
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private List<int> lElementRemove = new List<int>();
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* New Method: Add Element
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public int AddElement(T a_ClassInstance)
		{
			if( a_ClassInstance != null )
			{
				ElementHolder<T> Holder = new ElementHolder<T>();
				Holder.Instance			= a_ClassInstance;
				Holder.ID				= this.ClassList.Count;

				this.ClassList.Add( Holder );
				return Holder.ID;
			}
			return -1;
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* New Method: Remove Element ID
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public void RemoveElementID(int ID)
		{
			lElementRemove.Add(ID);
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* New Method: Remove Element
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private void RemoveElement(int ID)
		{
			if( ID >= 0 )
			{
				int Index = 0;
				bool Found = false;

				for( ; Index < this.ClassList.Count; ++Index )
				{
					if (this.ClassList[Index].ID == ID)
					{
						Found = true;
						break;
					}
				}

				if (Found)
				{
					this.ClassList.RemoveAt(Index);
				}
			}
		}
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		//	* New Method: Update
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public void Update()
		{
			foreach( int ID in lElementRemove )
			{
				RemoveElement(ID);
			}
			lElementRemove.Clear();
		}
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static private float							 m_fRealTime				= 0.0f;
	static private float							 m_fRealTimeDeltaTime		= 0.0f;
	static private TemplateList<TimeTracker>		 m_lTimeTrackerList			= new TemplateList<TimeTracker>();
	static private TemplateList<MovementBasedOnTime> m_lMovementBasedOnTimeList	= new TemplateList<MovementBasedOnTime>();
	static private TemplateList<FadeEffect>			 m_lFadeEffectList			= new TemplateList<FadeEffect>();

	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Start() 
	{
		m_fRealTime			 = Time.realtimeSinceStartup;
		m_fRealTimeDeltaTime = 0.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update() 
	{
		// Update DeltaTime
		UpdateRealTimeDeltaTime();

		// Update List Class					// Update Each Instance
		m_lTimeTrackerList.Update();			foreach (ElementHolder<TimeTracker>			TT	 in m_lTimeTrackerList.ClassList)			{	TT.Instance.Update();	}	// Update TimeTrackers
		m_lMovementBasedOnTimeList.Update();	foreach (ElementHolder<MovementBasedOnTime> MBOT in m_lMovementBasedOnTimeList.ClassList )	{	MBOT.Instance.Update();	}	// Update MovementBasedOnTime's
		m_lFadeEffectList.Update();				foreach (ElementHolder<FadeEffect>			FE	 in m_lFadeEffectList.ClassList )			{	FE.Instance.Update();	}	// Update Fade Effects
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void Reset()
	{
		m_lTimeTrackerList.ClassList.Clear();			
		m_lMovementBasedOnTimeList.ClassList.Clear();
		m_lFadeEffectList.ClassList.Clear();			
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update RealTime DeltaTime
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateRealTimeDeltaTime()
	{
		m_fRealTimeDeltaTime = (Time.realtimeSinceStartup - m_fRealTime);
		m_fRealTime = Time.realtimeSinceStartup;

		if (m_fRealTimeDeltaTime > 0.05f)
		{
			m_fRealTimeDeltaTime = 0.05f;
		}
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add TimeTracker
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public int AddTimeTracker(TimeTracker TT)
	{
		return m_lTimeTrackerList.AddElement(TT);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add MovementBasedOnTime
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public int AddMovementBasedOnTime(MovementBasedOnTime MBOT)
	{
		return m_lMovementBasedOnTimeList.AddElement(MBOT);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add FadeEffect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public int AddFadeEffect(FadeEffect FE)
	{
		return m_lFadeEffectList.AddElement(FE);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove TimeTracker
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public void RemoveTimeTracker(int ID)
	{
		m_lTimeTrackerList.RemoveElementID(ID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove MovementBasedOnTime
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public void RemoveMovementBasedOnTime(int ID)
	{
		m_lMovementBasedOnTimeList.RemoveElementID(ID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove FadeEffect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public void RemoveFadeEffect(int ID)
	{
		m_lFadeEffectList.RemoveElementID(ID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get DeltaTime
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public float GetDeltaTime()
	{
		return m_fRealTimeDeltaTime;
	}
}
