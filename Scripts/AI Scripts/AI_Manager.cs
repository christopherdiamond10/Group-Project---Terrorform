//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Manager
//#             Version: 1.0
//#             Author: Christopher Diamond
//#             Date: June 14, 2013
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script holds onto all AI's and allows for changes to be made to them 
//#    Dynamically. This script can be used to get and things related to the 
//#    'AI_Base' Class.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//				Class: AI_Holder
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// This Class is Used as if it were a Struct in C++, only 
// due to Struct restrictions in C# am I using this as a 
// clas instead. This class simply holds each AI Element in
// The Manager List.
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
public class AI_Holder
{
    public AI_Base Instance;
    public bool IsAlive;

    public AI_Holder()
    {
        Instance = null;
        IsAlive = false;
    }

    public AI_Holder(AI_Base a_Instance, bool a_IsAlive)
    {
        Instance = a_Instance;
        IsAlive = a_IsAlive;
    }
};


//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//				Class: AI_Manager
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
public class AI_Manager : MonoBehaviour 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private List< AI_Holder > m_lAIInstanceList = new List<AI_Holder>();



    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	** Redefined Method: Awake      { Constructor }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Awake()
    {
		Reset();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*~ Redefined Method: On Destroy    { Destructor }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDestroy()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public void Reset()
	{
		ListClear();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Start()
    {
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Update
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Update()
    {
    }




    
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Is Argument Valid?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private bool IsValidArgument( int Element )
    {
        return ((ListSize() > Element) && (Element > -1));
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Convert Element To Empty Object
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private void ConvertToEmptyElement( int Element )
    {
        if (IsValidArgument(Element))
        {
			AI_Holder Holder	= ListElementAt(Element);
            Holder.Instance		= null;
            Holder.IsAlive		= false;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Destroy Element
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void DestroyElement( int Element )
    {
        ConvertToEmptyElement(Element);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Destroy All Active AI's
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public void DestroyAllActiveAIs()
	{
		List<AI_Base> Enemies = GetAllActiveUnits();
		foreach (AI_Base Enemy in Enemies)
		{
			Enemy.KillMe(0.0f);
		}
	}

    static public void DestroyAllActiveAIs(bool DestroyExplosive, bool DestroyTurret, bool DestroyBrainBug, bool DestroyLightUnit, bool DestroyHeavyUnit)
    {
		List<AI_Base> Enemies = GetAllActiveUnits();
		foreach (AI_Base Enemy in Enemies)
        {
            switch( Enemy.GetEnemyType() )
            {
                case AI_Base.UnitType.EXPLOSIVE:    
				{ 
					if (DestroyExplosive && (Enemy as AI_ExplosiveUnit).GetCurrentStance() != AI_ExplosiveUnit.Stance.ATTACHED) 
					{
						Enemy.KillMe(0.0f); 
					} 
					break; 
				}

                case AI_Base.UnitType.TURRET:       { if (DestroyTurret)    { Enemy.KillMe(0.0f); } break; }
                case AI_Base.UnitType.BRAINBUG:     { if (DestroyBrainBug)  { Enemy.KillMe(0.0f); } break; }
                case AI_Base.UnitType.LIGHT_UNIT:   { if (DestroyLightUnit) { Enemy.KillMe(0.0f); } break; }
                default:                            { if (DestroyHeavyUnit) { Enemy.KillMe(0.0f); } break; }
            }
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Add Item to List
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void AddItemToList(AI_Holder Item)
    {
        ListAdd(Item);
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Add Empty Item to List
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private void AddEmptyItemToList()
    {
        AddItemToList(new AI_Holder());
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Swap Elements
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private void SwapElements(int ElementOne, int ElementTwo)
    {
        if( IsValidArgument(ElementOne) && IsValidArgument(ElementTwo) )
        {
			AI_Holder HolderElementOne	= ListElementAt(ElementOne);
			AI_Holder HolderElementTwo	= ListElementAt(ElementTwo);

            // Get First Element's Info
			AI_Base ElementOne_Instance = HolderElementOne.Instance;
            bool ElementOne_IsAlive     = HolderElementOne.IsAlive;

            // Give ElementTwo's Info to ElementOne
            HolderElementOne.Instance	= HolderElementTwo.Instance;
            HolderElementOne.IsAlive	= HolderElementTwo.IsAlive;

            // Give ElementOne's Previous Info to ElementTwo
            HolderElementTwo.Instance	= ElementOne_Instance;
            HolderElementTwo.IsAlive	= ElementOne_IsAlive;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Max Out List Size         (Only Works if Value is actually bigger than list size)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private void MaxOutListSize( int Size )
    {
        while( ListSize() < Size )
        {
            AddEmptyItemToList();
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Setup New AI
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void SetupNewAI( AI_Base AI_Instance )
    {
        int ID                  = GetNewAIID();                     // Get New ID from ListSizeCount
        AI_Holder NewElement    = new AI_Holder(AI_Instance, true); // Create New Element for List

        AddItemToList( NewElement );                                // Push Element into List
        SetAI_ID( ID, ID );                                         // Setup AI's ID
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set AI Instance
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void SetAIInstance(int AI_ID, AI_Base Instance)
    {
		if (IsValidArgument(AI_ID))
		{
			ListElementAt(AI_ID).Instance = Instance;
		}
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set AI Instance
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void SetAI_ID(int AICurrentID, int AINewID, bool AlsoChangeAIPositionInList = false)
    {
        if (IsValidArgument(AICurrentID))
        {
            // Set AI ID
			ListElementAt(AICurrentID).Instance.SetID(AINewID);
            
            // Change Position in List
            if( AlsoChangeAIPositionInList )
            {
                int iNewListSize = AINewID + 1;     // Get Appropriate Max Size for List 
                MaxOutListSize( iNewListSize );     // Ensure The List is at that Size
                SwapElements(AINewID, AICurrentID); // Swap the two Elements
            }
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set AI Alive
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public void SetAIAlive(int AI_ID, bool IsAlive)
    {
		if (IsValidArgument(AI_ID))
		{
			ListElementAt(AI_ID).IsAlive = IsAlive;
		}
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get New AI ID
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public int GetNewAIID()
    {
        return ListSize();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get AI Instance
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public AI_Base GetAIInstance( int AI_ID )
    {
        return IsValidArgument(AI_ID) ? ListAIInstanceAt(AI_ID) : null;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Is AI Alive?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static public bool Get_IsAIAlive( int AI_ID )
    {
        return IsValidArgument(AI_ID) ? ListAIIsAliveAt(AI_ID) : false;
    }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get All Active Units
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public List<AI_Base> GetAllActiveUnits()
	{
		List<AI_Base> lActiveUnits = new List<AI_Base>();

		foreach (AI_Holder Enemy in m_lAIInstanceList)
		{
			if (Enemy.IsAlive && Enemy.Instance.Get_IsActive())
			{
				lActiveUnits.Add(Enemy.Instance);
			}
		}
		return lActiveUnits;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Size of List
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static private int ListSize()
	{
		return m_lAIInstanceList.Count;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Clear List
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static private void ListClear()
	{
		m_lAIInstanceList.Clear();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Add Item To List
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private void ListAdd(AI_Holder Item)
    {
        m_lAIInstanceList.Add(Item);
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Size of List
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private AI_Holder ListElementAt(int Element)
    {
        return m_lAIInstanceList[Element];
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Specific 'AI Instance'
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private AI_Base ListAIInstanceAt(int Element)
    {
        return m_lAIInstanceList[Element].Instance;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Specific 'Is AI Alive'?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    static private bool ListAIIsAliveAt(int Element)
    {
        return m_lAIInstanceList[Element].IsAlive;
    }
}
