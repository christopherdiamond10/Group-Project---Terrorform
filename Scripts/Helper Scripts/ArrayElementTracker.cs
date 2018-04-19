//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Array Element Tracker
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script can be used as a wrapper for an array/list to easily keep track
//#    of which element(s) are currently being indexed.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ArrayElementTracker<T>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public T[] m_Array;
	public int m_iCurrentElement = 0;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	**	Constructors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ArrayElementTracker()
	{

	}

	public ArrayElementTracker(int Size)
	{
		m_Array = new T[Size];
	}

	public ArrayElementTracker(T[] Args)
	{
		m_Array = Args;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Increment Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void IncrementElement(int Count)
	{
		if ( ValidElement(m_iCurrentElement + Count) )
		{
			m_iCurrentElement += Count;
		}
	}

	public void IncrementElement()
	{
		IncrementElement( 1 );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Decrement Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DecrementElement(int Count)
	{
		IncrementElement( -Count );
	}

	public void DecrementElement()
	{
		IncrementElement( -1 );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetElement(int ElementID)
	{
		if( ValidElement(ElementID) )
		{
			m_iCurrentElement = ElementID;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Valid Element?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool ValidElement(int ElementID)
	{
		return ( (ElementID > -1) && (ElementID <= GetSize()) );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Element	   (Operator Overload)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public T this[int i]
	{
		get {	return m_Array[i];	}
		set {	m_Array[i] = value;	}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public T GetCurrentElement()
	{
		return m_Array[m_iCurrentElement];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Next Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public T GetNextElement(bool AdvanceCurrentElement = true)
	{
		if (AdvanceCurrentElement)
		{
			IncrementElement();
			return GetCurrentElement();
		}

		int NextElementID = m_iCurrentElement + 1;

		if( ValidElement( NextElementID ) )
		{
			return m_Array[NextElementID];
		}

		return m_Array[m_iCurrentElement];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Previous Element
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public T GetPreviousElement(bool RetreatCurrentElement = true)
	{
		if (RetreatCurrentElement)
		{
			DecrementElement();
			return GetCurrentElement();
		}

		int PreviousElementID = m_iCurrentElement - 1;

		if (ValidElement(PreviousElementID))
		{
			return m_Array[PreviousElementID];
		}

		return m_Array[m_iCurrentElement];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Size
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int GetSize()
	{
		return m_Array.Length;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: At The End Of The Array?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool AtEndOfArray()
	{
		return (m_iCurrentElement == GetSize()); 
	}
}
