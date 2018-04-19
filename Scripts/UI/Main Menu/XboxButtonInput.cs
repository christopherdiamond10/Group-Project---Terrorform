using UnityEngine;
using System.Collections;

public class XboxButtonInput : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_PlayButton;
	public GameObject m_CrewButton;
	public GameObject m_HighScoresButton;
	public GameObject m_ExitButton;

	private GameObject m_SelectedObject;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		UICamera.selectedObject = m_CrewButton;
		XboxInputHandler.ResetInput();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		XboxInputHandler.UpdateInput();

        if (XboxInputHandler.KeyTriggered(XboxInputHandler.Controls.DPad_Up))
        {
			UICamera.selectedObject = GetUpperButton();
        }
		else if (XboxInputHandler.KeyTriggered(XboxInputHandler.Controls.DPad_Down))
        {
			UICamera.selectedObject = GetBelowButton();
        }

		// If B is Pressed, NGUI Button Selection gets disabled. This Stops that. 
		if(XboxInputHandler.KeyPressed(XboxInputHandler.Controls.B))
		{
			UICamera.selectedObject = m_SelectedObject;
		}
		m_SelectedObject = UICamera.selectedObject;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Upper Button
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject GetUpperButton()
	{
		if		(UICamera.selectedObject == m_PlayButton)		{ return m_ExitButton;				}
		else if (UICamera.selectedObject == m_CrewButton)		{ return m_PlayButton;				}
		else if (UICamera.selectedObject == m_HighScoresButton) { return m_CrewButton;				}
		else if (UICamera.selectedObject == m_ExitButton)		{ return m_HighScoresButton;		}
		else													{ return UICamera.selectedObject;	}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Below Button
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject GetBelowButton()
	{
		if		(UICamera.selectedObject == m_PlayButton)		{ return m_CrewButton;				}
		else if (UICamera.selectedObject == m_CrewButton)		{ return m_HighScoresButton;		}
		else if (UICamera.selectedObject == m_HighScoresButton) { return m_ExitButton;				}
		else if (UICamera.selectedObject == m_ExitButton)		{ return m_PlayButton;				}
		else													{ return UICamera.selectedObject;	}
	}
}
