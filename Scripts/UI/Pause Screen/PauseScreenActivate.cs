using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseScreenActivate : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum PauseState
	{
		JUST_ACTIVATED,
		ACTIVE,
		JUST_DEACTIVATED,
		INACTIVE,
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject			m_VignetteObject;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private PauseState			m_ePauseState	= PauseState.INACTIVE;

	private static Vector3		m_vPauseScreenHiddenPosition	= new Vector3(0.0f, 67.45f, 40000.0f);
	private static Vector3		m_vPauseScreenVisiblePosition	= new Vector3(0.0f, 67.45f, 2000.0f);
	private static float		m_fPauseScreenMovementSpeed		= 1.5f;
	private MovementBasedOnTime m_MBOTPauseScreenHiddenToVisible;
	private MovementBasedOnTime m_MBOTPauseScreenVisibleToHidden;

	private static Vector3		m_vPlayerHiddenPosition			= new Vector3(0.0f, 0.0f, -4.0f);
	private static Vector3		m_vPlayerVisiblePosition		= new Vector3(0.0f, 0.0f, 1.363258f);
	private static float		m_fPlayerMovementSpeed			= 1.5f;
	private MovementBasedOnTime m_MBOTPlayerHiddenToVisible;
	private MovementBasedOnTime m_MBOTPlayerVisibleToHidden;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start () 
	{
		m_MBOTPauseScreenHiddenToVisible    = new MovementBasedOnTime(m_vPauseScreenHiddenPosition, m_vPauseScreenVisiblePosition, m_fPauseScreenMovementSpeed, false, true);
		m_MBOTPauseScreenVisibleToHidden    = new MovementBasedOnTime(m_vPauseScreenVisiblePosition, m_vPauseScreenHiddenPosition, m_fPauseScreenMovementSpeed, false, true);
		m_MBOTPlayerHiddenToVisible         = new MovementBasedOnTime(m_vPlayerHiddenPosition, m_vPlayerVisiblePosition, m_fPlayerMovementSpeed, false, true);
		m_MBOTPlayerVisibleToHidden         = new MovementBasedOnTime(m_vPlayerVisiblePosition, m_vPlayerHiddenPosition, m_fPlayerMovementSpeed, false, true);

		DeactivateUI();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update () 
	{
		if( !GameHandler.IsDialogueBoxPaused() )
			{
			if( IsTransitioning() )
			{
				UpdatePauseTransition();
			}

			else if (PressedPause())
			{
				ChangeState();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Pause Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdatePauseTransition()
	{
		GameObject oPauseScreen		= GetComponent< PauseScreenObjectsHolder >().m_PauseScreenPanel.Panel;
		GameObject oPlayer			= GetComponent< PauseScreenObjectsHolder >().m_PlayerHolder.PlayerParent;

		if (m_ePauseState == PauseState.JUST_ACTIVATED)
		{
			m_MBOTPauseScreenHiddenToVisible.Update();
			m_MBOTPlayerHiddenToVisible.Update();

			oPauseScreen.transform.localPosition	= m_MBOTPauseScreenHiddenToVisible.GetCurrentPosition();
			oPlayer.transform.localPosition			= m_MBOTPlayerHiddenToVisible.GetCurrentPosition();
			m_VignetteObject.GetComponent<UISprite>().alpha = m_MBOTPauseScreenHiddenToVisible.GetTimeInstance().GetCompletionPercentage();
			if (m_MBOTPauseScreenHiddenToVisible.HasReachedDestination() && m_MBOTPlayerHiddenToVisible.HasReachedDestination())
			{
				m_MBOTPauseScreenHiddenToVisible.Reset();
				m_MBOTPlayerHiddenToVisible.Reset();
				ChangeState();
			}
		}

		else
		{
			m_MBOTPauseScreenVisibleToHidden.Update();
			m_MBOTPlayerVisibleToHidden.Update();

			oPauseScreen.transform.localPosition = m_MBOTPauseScreenVisibleToHidden.GetCurrentPosition();
			oPlayer.transform.localPosition = m_MBOTPlayerVisibleToHidden.GetCurrentPosition();
			m_VignetteObject.GetComponent<UISprite>().alpha = 1.0f - m_MBOTPauseScreenVisibleToHidden.GetTimeInstance().GetCompletionPercentage();
			if (m_MBOTPauseScreenVisibleToHidden.HasReachedDestination() && m_MBOTPlayerVisibleToHidden.HasReachedDestination())
			{
				m_MBOTPauseScreenVisibleToHidden.Reset();
				m_MBOTPlayerVisibleToHidden.Reset();
				ChangeState();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeState()
	{
		switch (m_ePauseState)
		{
			case PauseState.INACTIVE:
			{
				m_ePauseState = PauseState.JUST_ACTIVATED;
				GameHandler.SetPausedGame(true);
				ActivateUI();
				break;
			}

			case PauseState.ACTIVE:
			{
				m_ePauseState = PauseState.JUST_DEACTIVATED;
				break;
			}

			case PauseState.JUST_ACTIVATED:
			{
				m_ePauseState = PauseState.ACTIVE;
				break;
			}

			case PauseState.JUST_DEACTIVATED:
			{
				m_ePauseState = PauseState.INACTIVE;
				DeactivateUI();
				GameHandler.SetPausedGame(false);
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate UI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateUI()
	{
		Time.timeScale = 0.0f;
		GetComponent<PauseScreenObjectsHolder>().m_PauseSceenUI.SetActive(true);
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.Panel.SetActive(true);
		GetComponent<PauseScreenObjectsHolder>().m_PlayerHolder.PlayerParent.SetActive(true);
		gameObject.SetActive(true);

		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.Panel.transform.localPosition		= m_vPauseScreenHiddenPosition;
		GetComponent<PauseScreenObjectsHolder>().m_PlayerHolder.PlayerParent.transform.localPosition	= m_vPlayerHiddenPosition;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Deactivate UI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DeactivateUI()
	{
		Time.timeScale = 1.0f;
		GetComponent<PauseScreenObjectsHolder>().m_PauseSceenUI.SetActive(false);
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.Panel.SetActive(false);
		GetComponent<PauseScreenObjectsHolder>().m_PlayerHolder.PlayerParent.SetActive(false);
		GetComponent<PauseScreenObjectsHolder>().m_MainMenuConfirmationPanel.Panel.SetActive(false);
		GetComponent<PauseScreenObjectsHolder>().m_RestartConfirmationPanel.Panel.SetActive(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Has Pressed Pause?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool PressedPause()
	{
		return Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P) || XboxInputHandler.KeyTriggered(XboxInputHandler.Controls.Start);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Transitioning Tp/From Pause Screen?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsTransitioning()
	{
		return (m_ePauseState == PauseState.JUST_ACTIVATED) || (m_ePauseState == PauseState.JUST_DEACTIVATED);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentState(PauseState ePauseState)
	{
		m_ePauseState = ePauseState;
	}
}
