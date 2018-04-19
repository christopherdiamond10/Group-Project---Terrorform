using UnityEngine;
using System.Collections;

public class PauseScreenButtonCollision : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum ButtonType
	{
		RESUME,
		RESTART,
		QUALITY_LEVEL_LEFT,
		QUALITY_LEVEL_RIGHT,
		INVERT_Y_LEFT,
		INVERT_Y_RIGHT,
		PLAY_BGM_LEFT,
		PLAY_BGM_RIGHT,
		MAIN_MENU,

		RESTART_AFFIRMATIVE_BUTTON,
		RESTART_NEGATIVE_BUTTON,

		MAIN_MENU_CONFIRMATION_BUTTON,
		MAIN_MENU_NEGATIVE_BUTTON,

		NONE,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ButtonType m_eButtonType = ButtonType.RESUME;
	public GameObject m_PauseMenuScriptsObject;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TimeTracker m_TTButtonEffectTimer;              // Stops the Button from being hit more than once after just being hit.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_TTButtonEffectTimer = new TimeTracker(0.1f, false, true);
		m_TTButtonEffectTimer.m_fCurrentTime = 2.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		RaycastHit hit;
		if (GetComponent<Rigidbody>().SweepTest(transform.forward, out hit, 20))
		{
			if (m_TTButtonEffectTimer.TimeUp())
			{
				m_PauseMenuScriptsObject.GetComponent<PauseScreenButtonEffects>().ActivateButtonEffect(m_eButtonType);
				m_TTButtonEffectTimer.Reset();
			}
			Destroy(hit.transform.gameObject);
		}

		m_TTButtonEffectTimer.Update();
	}
}
