using UnityEngine;
using System.Collections;

public class PauseScreenButtonEffects : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum ArrowDirection
	{
		LEFT,
		RIGHT,
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private PauseScreenButtonCollision.ButtonType m_eButtonType = PauseScreenButtonCollision.ButtonType.NONE;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Start()
	{
		SetQualityTypeText();
		SetInvertYTypeText();
		SetPlayBGMLabel();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update()
	{
		if (m_eButtonType != PauseScreenButtonCollision.ButtonType.NONE)
		{
			BeginButtonEffect();
			m_eButtonType = PauseScreenButtonCollision.ButtonType.NONE;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void BeginButtonEffect()
	{
		switch (m_eButtonType)
		{
			case PauseScreenButtonCollision.ButtonType.RESUME:
			{
				ActivateResumeButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.RESTART:
			{
				ActivateRestartButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.QUALITY_LEVEL_LEFT:
			{
				ActivateQualityLevelButtonEffect(ArrowDirection.LEFT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.QUALITY_LEVEL_RIGHT:
			{
				ActivateQualityLevelButtonEffect(ArrowDirection.RIGHT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.INVERT_Y_LEFT:
			{
				ActivateInvertYAxisButtonEffect(ArrowDirection.LEFT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.INVERT_Y_RIGHT:
			{
				ActivateInvertYAxisButtonEffect(ArrowDirection.RIGHT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.PLAY_BGM_LEFT:
			{
				ActivatePlayBGMButtonEffect(ArrowDirection.LEFT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.PLAY_BGM_RIGHT:
			{
				ActivatePlayBGMButtonEffect(ArrowDirection.RIGHT);
				break;
			}

			case PauseScreenButtonCollision.ButtonType.MAIN_MENU:
			{
				ActivateMainMenuButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.RESTART_AFFIRMATIVE_BUTTON:
			{
				ActivateRestartConfirmationButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.RESTART_NEGATIVE_BUTTON:
			{
				ActivateRestartNegativeButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.MAIN_MENU_CONFIRMATION_BUTTON:
			{
				ActivateMainMenuConfirmationButtonEffect();
				break;
			}

			case PauseScreenButtonCollision.ButtonType.MAIN_MENU_NEGATIVE_BUTTON:
			{
				ActivateMainMenuNegativeButtonEffect();
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ActivateButtonEffect(PauseScreenButtonCollision.ButtonType eButtonType)
	{
		m_eButtonType = eButtonType;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set "Quality Type" Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetQualityTypeText()
	{
		GetComponent< PauseScreenObjectsHolder >().m_PauseScreenPanel.QualityLevelTypeText.GetComponent< UILabel >().text =	(GameHandler.GetGraphicsQualityLevel() == 0) ?	"Fastest"	:
																															(GameHandler.GetGraphicsQualityLevel() == 1) ?	"Faster"	:
																															(GameHandler.GetGraphicsQualityLevel() == 2) ?	"Simple"	:
																															(GameHandler.GetGraphicsQualityLevel() == 3) ?	"Beautiful" : 
																																											"Fantastic"	;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set "Inverted Y Bool" Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetInvertYTypeText()
	{
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.InvertYTypeLabel.GetComponent<UILabel>().text = (GameHandler.IsInvertedYAxis() ? "On" : "Off");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set "Play BGM Bool" Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetPlayBGMLabel()
	{
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.PlayBGMTypeLabel.GetComponent<UILabel>().text = (SoundPlayerManager.IsMuted() ? "Off" : "On");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Resume Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateResumeButtonEffect()
	{
		GetComponent< PauseScreenActivate >().SetCurrentState( PauseScreenActivate.PauseState.JUST_DEACTIVATED );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Restart Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateRestartButtonEffect()
	{
		GetComponent< PauseScreenObjectsHolder >().m_PauseScreenPanel.Panel.SetActive( false );
		GetComponent< PauseScreenObjectsHolder >().m_RestartConfirmationPanel.Panel.SetActive( true );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Quality Level Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateQualityLevelButtonEffect(ArrowDirection eDirection)
	{
		if( eDirection == ArrowDirection.LEFT )
		{
			if( QualitySettings.GetQualityLevel() > 0 )
			{
				QualitySettings.SetQualityLevel( QualitySettings.GetQualityLevel() - 1 );
				SetQualityTypeText();
			}
		}

		else
		{
			if( QualitySettings.GetQualityLevel() < 4 )
			{
				QualitySettings.SetQualityLevel( QualitySettings.GetQualityLevel() + 1 );
				SetQualityTypeText();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Invert Y Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateInvertYAxisButtonEffect(ArrowDirection eDirection)
	{
		GameHandler.SetInvertedYAxis( !GameHandler.IsInvertedYAxis() );
		SetInvertYTypeText();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Play BGM Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivatePlayBGMButtonEffect(ArrowDirection eDirection)
	{
		SoundPlayerManager.SetMuted( !SoundPlayerManager.IsMuted() );
		SetPlayBGMLabel();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Main Menu Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateMainMenuButtonEffect()
	{
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.Panel.SetActive(false);
		GetComponent<PauseScreenObjectsHolder>().m_MainMenuConfirmationPanel.Panel.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Restart Confirmation Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateRestartConfirmationButtonEffect()
	{
		GameHandler.SetPausedGame(false);
		GameHandler.LoadLevel( GameHandler.GetCurrentLevelIDAsInt() );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Restart Cancel Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateRestartNegativeButtonEffect()
	{
		GetComponent< PauseScreenObjectsHolder >().m_PauseScreenPanel.Panel.SetActive( true );
		GetComponent< PauseScreenObjectsHolder >().m_RestartConfirmationPanel.Panel.SetActive( false );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate MainMenu Confirmation Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateMainMenuConfirmationButtonEffect()
	{
		GameHandler.SetPausedGame(false);
		GameHandler.LoadLevel("MainMenu");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate MainMenu Cancel Button Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateMainMenuNegativeButtonEffect()
	{
		GetComponent<PauseScreenObjectsHolder>().m_PauseScreenPanel.Panel.SetActive(true);
		GetComponent<PauseScreenObjectsHolder>().m_MainMenuConfirmationPanel.Panel.SetActive(false);
	}
}
