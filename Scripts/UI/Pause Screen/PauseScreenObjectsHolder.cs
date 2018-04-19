using UnityEngine;
using System.Collections;

public class PauseScreenObjectsHolder : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Serializable]
	public class ButtonObject
	{
		public GameObject Parent;
		public GameObject Label;
		public GameObject Background;
	};

	[System.Serializable]
	public class PauseScreenPanel
	{
		public GameObject		Panel;
		public GameObject		BackgroundColour;
		public GameObject		BackgroundSprite;
		public GameObject		PausedLabel;
		public ButtonObject		ResumeButton;
		public ButtonObject		RestartButton;
		public GameObject		TopHorizontalLine;
		public GameObject		QualityLevelLabel;
		public GameObject		QualityLevelLeftButton;
		public GameObject		QualityLevelTypeText;
		public GameObject		QualityLevelRightButton;
		public GameObject		InvertYLabel;
		public GameObject		InvertYLeftButton;
		public GameObject		InvertYTypeLabel;
		public GameObject		InvertYRightButton;
		public GameObject		PlayBGMLabel;
		public GameObject		PlayBGMLeftButton;
		public GameObject		PlayBGMTypeLabel;
		public GameObject		PlayBGMRightButton;
		public GameObject		BottomHorizontalLine;
		public ButtonObject		MainMenuButton;
	};

	[System.Serializable]
	public class ConfirmationPanel
	{
		public GameObject	Panel;
		public ButtonObject AffirmativeButton;
		public ButtonObject NegativeButton;
		public GameObject	BackgroundColour;
		public GameObject	BackgroundSprite;
		public GameObject	TextLabel;
		public GameObject	WarningLabel;
	};


	[System.Serializable]
	public class PlayerHolder
	{
		public GameObject PlayerParent;
		public GameObject Aimer;
		public GameObject Player;
	};
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject			m_PauseScreenParent;
	public GameObject			m_PauseSceenUI;
	public PauseScreenPanel		m_PauseScreenPanel;
	public ConfirmationPanel	m_RestartConfirmationPanel;
	public ConfirmationPanel	m_MainMenuConfirmationPanel;
	public PlayerHolder			m_PlayerHolder;
	public GameObject			m_Camera;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get All Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject[] GetAllObjects()
	{
		return new GameObject[]
		{
			m_PauseScreenParent,
			m_PauseSceenUI,
			m_PauseScreenPanel.BackgroundColour,
			m_PauseScreenPanel.BackgroundSprite,
			m_PauseScreenPanel.BottomHorizontalLine,
			m_PauseScreenPanel.InvertYLabel,
			m_PauseScreenPanel.InvertYLeftButton,
			m_PauseScreenPanel.InvertYRightButton,
			m_PauseScreenPanel.InvertYTypeLabel,
			m_PauseScreenPanel.MainMenuButton.Background,
			m_PauseScreenPanel.MainMenuButton.Label,
			m_PauseScreenPanel.MainMenuButton.Parent,
			m_PauseScreenPanel.Panel,
			m_PauseScreenPanel.PausedLabel,
			m_PauseScreenPanel.QualityLevelLabel,
			m_PauseScreenPanel.QualityLevelLeftButton,
			m_PauseScreenPanel.QualityLevelRightButton,
			m_PauseScreenPanel.QualityLevelTypeText,
			m_PauseScreenPanel.RestartButton.Background,
			m_PauseScreenPanel.RestartButton.Label,
			m_PauseScreenPanel.RestartButton.Parent,
			m_PauseScreenPanel.ResumeButton.Background,
			m_PauseScreenPanel.ResumeButton.Label,
			m_PauseScreenPanel.TopHorizontalLine,
			m_RestartConfirmationPanel.AffirmativeButton.Background,
			m_RestartConfirmationPanel.AffirmativeButton.Label,
			m_RestartConfirmationPanel.AffirmativeButton.Parent,
			m_RestartConfirmationPanel.BackgroundColour,
			m_RestartConfirmationPanel.BackgroundSprite,
			m_RestartConfirmationPanel.NegativeButton.Background,
			m_RestartConfirmationPanel.NegativeButton.Label,
			m_RestartConfirmationPanel.NegativeButton.Parent,
			m_RestartConfirmationPanel.Panel,
			m_RestartConfirmationPanel.TextLabel,
			m_RestartConfirmationPanel.WarningLabel,
			m_MainMenuConfirmationPanel.AffirmativeButton.Background,
			m_MainMenuConfirmationPanel.AffirmativeButton.Label,
			m_MainMenuConfirmationPanel.AffirmativeButton.Parent,
			m_MainMenuConfirmationPanel.BackgroundColour,
			m_MainMenuConfirmationPanel.BackgroundSprite,
			m_MainMenuConfirmationPanel.NegativeButton.Background,
			m_MainMenuConfirmationPanel.NegativeButton.Label,
			m_MainMenuConfirmationPanel.NegativeButton.Parent,
			m_MainMenuConfirmationPanel.Panel,
			m_MainMenuConfirmationPanel.TextLabel,
			m_MainMenuConfirmationPanel.WarningLabel,
			m_PlayerHolder.Aimer,
			m_PlayerHolder.Player,
			m_PlayerHolder.PlayerParent,
			m_Camera,
		};
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is a Pause Screen GameObject?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsPauseScreenObject(GameObject TestObj)
	{
		GameObject[] PauseObjects = GetAllObjects();
		foreach (GameObject Obj in PauseObjects)
		{
			if( Obj == TestObj )
				return true;
		}
		return false;
	}
}
