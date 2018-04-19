//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Load Level Script
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script handles the load screen when loading a scene.
//#		This includes shutting doors, loading scene, then reopening them for
//#		the load effect.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class LevelLoadScript : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum LoadState
	{
		CloseDoors,
		RevealText,
		LoadLevel,
		OpenDoors,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject[]			m_ObjectsToPreserve;
	public GameObject[]			m_ObjectsToDestroyDuringLoad;
	public GameObject[]			m_ObjectsToDestroyAfterLoad;
	public string				m_LevelLoadName = "FinishedLevel";
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool				m_LoadLevel;
	private LoadState			m_eLoadState = LoadState.CloseDoors;
	private LoadingDoorsScript	m_DoorsInstance;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Start()
    {
		m_DoorsInstance = GetComponent<LoadingDoorsScript>();

		foreach (GameObject GO in m_ObjectsToPreserve)
		{
			DontDestroyOnLoad(GO);
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		if (m_LoadLevel)
		{
			switch(m_eLoadState)
			{
				case LoadState.CloseDoors:
				{
                    if( !m_DoorsInstance.Active() )
                    {
                        m_DoorsInstance.Close();
                    }

                    else if( m_DoorsInstance.Activated() )
                    {
                        m_eLoadState = LoadState.LoadLevel;
						m_DoorsInstance.Reset();
                    }
                    break;
				}

				case LoadState.RevealText:
				{
					if (!m_DoorsInstance.Active())
					{
						m_DoorsInstance.RevealText();
					}

					else if (m_DoorsInstance.Activated())
					{
						foreach (GameObject GO in m_ObjectsToDestroyDuringLoad)
						{
							DestroyObject(GO);
						}

						m_eLoadState = LoadState.LoadLevel;
						m_DoorsInstance.Reset();
					}
					break;
				}

                case LoadState.LoadLevel:
                {
                    GameHandler.LoadLevel(m_LevelLoadName);
                    m_eLoadState = LoadState.OpenDoors;
                    break;
                }

                case LoadState.OpenDoors:
                {
                    if( !m_DoorsInstance.Active() )
                    {
                        m_DoorsInstance.Open();
                    }
                    else if (m_DoorsInstance.Activated())
                    {
						foreach (GameObject GO in m_ObjectsToDestroyAfterLoad)
						{
							DestroyObject(GO);
						}
                    }
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
	//	* New Method: Load Level
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void LoadLevel()
	{
		m_LoadLevel = true;
	}
}
