//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Dialogue Box
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script uses NGUI to create a dialoguebox which is capable of:
//#		* Displaying a Face Sprite and Animating it.
//#		* Drawing Text over time.
//#		* Scroll Box to Reveal/Hide the DialogueBox
//#		* Use Regular Expression for special commands.
//#
//#		This script handles all of those actions.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{}	Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum FaceState
	{
		MOUTH_CLOSED,
		MOUTH_OPENED,
	};

	public enum SpokesPersonState
	{
		PLAYER,
		MALE_NPC,
		FEMALE_NPC,
		SARGE,
		GENERAL,
	};

    public enum ScrollDirection
    {
        UP,
        DOWN,
		RIGHT,
    };

	public enum ScrollPhase
	{
		DROP,
		CENTRED,
		COMPLETE,
	};


	public class PauseTrigger
	{
		public bool			                Paused	= false;
		public KeyCode[]	                Keys	= null;
		public XboxInputHandler.Controls[]	JoyAxis = null;
	};

	[System.Serializable]
	public class SoundFiles
	{
		public AudioClip[]	FlyInSFX;
		public AudioClip[]	FlyOutSFX;
		public AudioClip	TextSFX;

		[HideInInspector] public AudioSource[]	FlyInSource;
		[HideInInspector] public AudioSource[]	FlyOutSource;
		[HideInInspector] public AudioSource	TextSource;
	}

    public struct ScrollInfo
    {
		public bool			   UpdateText;
        public bool            Scroll;
		public ScrollPhase	   ScrollLodgePhase;
        public ScrollDirection Dir;
    };
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+	Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public DialogueBoxObjectsHolder             m_DialogueBoxObjects;
	public UIObjectsHolder						m_UIObjects;
	public float						        m_fFaceAnimationSpeedInSeconds	= 0.3f;
	public float						        m_fTextSpeedInSeconds			= 0.001f;
	public float						        m_fEndOfTextWaitTimer			= 5.0f;
	public SpokesPersonState			        m_eSpeaker						= SpokesPersonState.PLAYER;

	public SoundFiles							m_SoundFiles;

	public static float   m_fScrollSpeed		 = 20.0f;
	public static Vector3 m_vVisiblePosition	 = new Vector3(0.0f,		0.0f,		0.0f);
	public static Vector3 m_vLiftLodgePosition	 = new Vector3(0.0f,		10.0f,		0.0f);
	public static Vector3 m_vDropLodgePosition	 = new Vector3(0.0f,	   -5.1f,		0.0f);
	public static Vector3 m_vHiddenDownPosition	 = new Vector3(0.0f,	   -193.7477f,	0.0f);
	public static Vector3 m_vHiddenRightPosition = new Vector3(1047.897f,	0.0f,		0.0f);
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-	Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ArrayElementTracker<string>         m_aTextArray;
	private int							        m_iCurrentCharElement;
	private FaceState					        m_eFaceState					= FaceState.MOUTH_OPENED;
	private	TimeTracker					        m_TTTextTimer;
	private TimeTracker					        m_TTFaceAnimationTimer;
	private TimeTracker					        m_TTEndOfTextWaitTimer;
    private ScrollInfo                          m_ScrollInfo;
	private PauseTrigger						m_PauseTrigger;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Start() 
	{
		m_TTTextTimer				 = new TimeTracker(m_fTextSpeedInSeconds, false);
		m_TTFaceAnimationTimer		 = new TimeTracker(m_fFaceAnimationSpeedInSeconds, false);
		m_TTEndOfTextWaitTimer		 = new TimeTracker(m_fEndOfTextWaitTimer, false);
		m_PauseTrigger				 = new PauseTrigger();


		SetText(GetLabelScript().text, m_eSpeaker);
        SetScrollPosition();
		ContinueToNextFaceAnimation();
		SetupSounds();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Sounds
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupSounds()
	{
		m_SoundFiles.FlyInSource = new AudioSource[m_SoundFiles.FlyInSFX.Length];
		for (int i = 0; i < m_SoundFiles.FlyInSFX.Length; ++i )
		{
			m_SoundFiles.FlyInSource[i] = SoundPlayerManager.AddAudio(gameObject, m_SoundFiles.FlyInSFX[i], false, false, 0.8f);
		}

		m_SoundFiles.FlyOutSource = new AudioSource[m_SoundFiles.FlyOutSFX.Length];
		for (int i = 0; i < m_SoundFiles.FlyOutSFX.Length; ++i)
		{
			m_SoundFiles.FlyOutSource[i] = SoundPlayerManager.AddAudio(gameObject, m_SoundFiles.FlyOutSFX[i], false, false, 0.8f);
		}

		m_SoundFiles.TextSource = SoundPlayerManager.AddAudio(gameObject, m_SoundFiles.TextSFX, false, false, 0.8f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redfined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Update() 
	{
		if( m_PauseTrigger.Paused )
		{
			UpdatePauseInput();
		}
		else if (m_ScrollInfo.Scroll)
		{
			UpdateScroll();
		}
		else if( m_ScrollInfo.ScrollLodgePhase != ScrollPhase.COMPLETE )
		{
			UpdateLodge();
		}
		else if( m_ScrollInfo.UpdateText )
		{
			UpdateText();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Pause Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdatePauseInput()
	{
		// Update Vignette Alpha
		m_UIObjects.VignetteObject.GetComponent<UISprite>().alpha = Mathf.Abs(Mathf.Cos(Time.realtimeSinceStartup * 0.8f));

		if( m_PauseTrigger.Keys != null )
		{
			foreach( KeyCode Key in m_PauseTrigger.Keys )
			{
				if( Input.GetKey(Key) )
				{
					m_PauseTrigger.Paused	= false;
					m_PauseTrigger.JoyAxis	= null;
					m_PauseTrigger.Keys		= null;
					Time.timeScale			= 1.0f;
					GameHandler.SetPausedGame(false);
					GameHandler.SetDialogueBoxPaused(false);
					m_UIObjects.VignetteObject.GetComponent<UISprite>().alpha = 0.0f;
					return;
				}
			}
		}

		if( m_PauseTrigger.JoyAxis != null )
		{
            foreach (XboxInputHandler.Controls Axis in m_PauseTrigger.JoyAxis)
			{
				if( XboxInputHandler.KeyTriggered(Axis) )
				{
					m_PauseTrigger.Paused	= false;
					m_PauseTrigger.JoyAxis	= null;
					m_PauseTrigger.Keys		= null;
					Time.timeScale			= 1.0f;
					GameHandler.SetPausedGame(false);
					GameHandler.SetDialogueBoxPaused(false);
					m_UIObjects.VignetteObject.GetComponent<UISprite>().alpha = 0.0f;
					return;
				}
			}
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Update Scroll
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void UpdateScroll()
    {
		float fScrollSpeed													= (m_fScrollSpeed * Time.deltaTime);
		m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition   += (GetScrollPosition() - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition).normalized * fScrollSpeed;
		
		// If At Position
		if (((GetScrollPosition() - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition).magnitude < (fScrollSpeed * 2.0f)))
		{
			m_ScrollInfo.Scroll = false;
			m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition = GetScrollPosition();

			// Begin LODGE Process, if Scrolling UPWARDS
			if( (GetCurrentScrollDirection() == ScrollDirection.UP) )
			{
				m_ScrollInfo.ScrollLodgePhase = ScrollPhase.DROP; 
			}
		}
	 }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Lodge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLodge()
	{
		float fScrollSpeed = (80.0f * Time.deltaTime);

		switch( m_ScrollInfo.ScrollLodgePhase )
		{
			case ScrollPhase.DROP:
			{
				Vector3 Velocity = (m_vDropLodgePosition - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition);
				m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition += Velocity.normalized * fScrollSpeed;
				if ((m_vDropLodgePosition - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition).magnitude < (fScrollSpeed * 2.0f))
				{
					m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition = m_vDropLodgePosition;
					m_ScrollInfo.ScrollLodgePhase = ScrollPhase.CENTRED;
				}
				break;
			}

			case ScrollPhase.CENTRED:
			{
				m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition += (m_vVisiblePosition - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition).normalized * fScrollSpeed;
				if ((m_vVisiblePosition - m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition).magnitude < (fScrollSpeed * 2.5f))
				{
					m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition = m_vVisiblePosition;
					m_ScrollInfo.ScrollLodgePhase = ScrollPhase.COMPLETE;
				}
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateText()
	{
        if (!m_aTextArray.AtEndOfArray())
        {
			if (m_SoundFiles.TextSource != null)
			{
				if (!m_SoundFiles.TextSource.isPlaying)
					m_SoundFiles.TextSource.Play();
			}

            // Update Text (H...e...l...l...o) and Face Animation, if there is still text
            if (m_iCurrentCharElement < GetCurrentWindowText().Length)
            {
                m_TTTextTimer.Update();
                m_TTFaceAnimationTimer.Update();

                if (m_TTTextTimer.TimeUp())
                {
                    GetLabelScript().text += GetNextCharacter();
                    m_iCurrentCharElement += 1;
                    m_TTTextTimer.Reset();
                }

                if (m_TTFaceAnimationTimer.TimeUp())
                {
                    ContinueToNextFaceAnimation();
                    m_TTFaceAnimationTimer.Reset();
                }
            }

            // Otherwise Wait Long Enough for the Text to continue
            else
            {
				if( m_SoundFiles.TextSource != null )
				{
					m_SoundFiles.TextSource.Stop();
				}

                // Close the Mouth of the NPC, (NoMoarTxt4U)
                if (m_eFaceState != FaceState.MOUTH_CLOSED)
                {
                    ContinueToNextFaceAnimation();
                }


                m_TTEndOfTextWaitTimer.Update();

                if (m_TTEndOfTextWaitTimer.TimeUp())
                {
                    m_TTEndOfTextWaitTimer.Reset();
                    m_aTextArray.IncrementElement();
                    ResetTextDisplay();
                }
            }
        }

		// At End of Dialogue
        else
		{
			m_ScrollInfo.Scroll		= true;
			m_ScrollInfo.UpdateText = false;
			m_ScrollInfo.Dir		= ScrollDirection.DOWN;

			if( m_SoundFiles.FlyOutSource != null && m_SoundFiles.FlyOutSource.Length > 0 )
			{
				m_SoundFiles.FlyOutSource[ Random.Range(0, m_SoundFiles.FlyOutSource.Length) ].Play();
			}
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Continue To Next Face Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ContinueToNextFaceAnimation()
	{
		if (m_eFaceState == FaceState.MOUTH_OPENED)
		{
			m_eFaceState						= FaceState.MOUTH_CLOSED;
			GetFaceSpriteScript().spriteName	= GetClosedMouthSpriteName();
		}
		else
		{
			m_eFaceState						= FaceState.MOUTH_OPENED;
			GetFaceSpriteScript().spriteName	= GetOpenedMouthSpriteName();
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set Scroll Position
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void SetScrollPosition()
    {
        m_ScrollInfo					= new ScrollInfo();
		m_ScrollInfo.UpdateText			= false;
        m_ScrollInfo.Scroll				= false;
		m_ScrollInfo.ScrollLodgePhase	= ScrollPhase.COMPLETE;
        m_ScrollInfo.Dir				= ScrollDirection.DOWN;

		m_DialogueBoxObjects.m_DialogueBoxParent.transform.localPosition = m_vHiddenDownPosition;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ResetTextDisplay()
	{
		GetLabelScript().text				= "";
		GetSpeakerNameScript().text			= GetSpeakerName();
		GetFaceSpriteScript().spriteName	= GetClosedMouthSpriteName();
		m_iCurrentCharElement				= 0;
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Set Text
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void SetText(string Text, SpokesPersonState Speaker = SpokesPersonState.GENERAL, float ScrollSpeed = 20.0f)
    {
		if( Text != "" )
		{
			m_ScrollInfo.UpdateText = true;
	        m_ScrollInfo.Scroll		= true;
	        m_ScrollInfo.Dir		= ScrollDirection.UP;
	
	        //Text					= ReplaceRGBCommandsWithHex(Text);
	        m_aTextArray			= new ArrayElementTracker<string>(ReplaceENDL(Text));
	
	        m_eSpeaker				= Speaker;
			m_fScrollSpeed			= ScrollSpeed;

			if (m_SoundFiles.FlyInSource != null && m_SoundFiles.FlyInSource.Length > 0)
			{
				m_SoundFiles.FlyInSource[Random.Range(0, m_SoundFiles.FlyInSource.Length)].Play();
			}
		}
		ResetTextDisplay();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Scroll Direction
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ScrollDirection GetCurrentScrollDirection()
	{
		return m_ScrollInfo.Dir;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Scroll Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 GetScrollPosition()
	{
		switch (GetCurrentScrollDirection())
		{
			case ScrollDirection.UP:	return m_vLiftLodgePosition;
			case ScrollDirection.DOWN:	return m_vHiddenDownPosition;
			default:					return m_vHiddenRightPosition;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current Window Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetCurrentWindowText()
	{
		return m_aTextArray.GetCurrentElement();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Next Character
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetNextCharacter()
	{
		// Check for Hex Command
		if ((GetCurrentWindowText().Length - m_iCurrentCharElement) > 2)
		{
			string HexCommandString = GetNextCharacters(3);
			if (IsHexCommand(HexCommandString))
			{
				m_iCurrentCharElement += HexCommandString.Length;
				return (HexCommandString + GetCurrentWindowText()[m_iCurrentCharElement]);
			}
		}

        // Check for Colour Hex Command
        if ((GetCurrentWindowText().Length - m_iCurrentCharElement) > 9)
        {
            string HexValuePattern = GetNextCharacters(9);
            if (IsRGBHexValue(HexValuePattern))
            {
                m_iCurrentCharElement += HexValuePattern.Length;
                return (HexValuePattern + GetCurrentWindowText()[m_iCurrentCharElement]);
            }
        }

		if ((GetCurrentWindowText().Length - m_iCurrentCharElement) > 8)
		{
			string StringPattern = GetNextCharacters(9);
			if( IsPauseCommand(StringPattern) )
			{
				m_iCurrentCharElement += StringPattern.Length;
				// If Pause Conditions Exist
				if( m_PauseTrigger.Keys != null || m_PauseTrigger.JoyAxis != null )
				{
					//GameHandler.SetPausedGame(true);
					//GameHandler.SetDialogueBoxPaused(true);
					//Time.timeScale			= 0.0f;
					//m_PauseTrigger.Paused	= true;
				}
			}
		}

		// Check for Speaker Command
		if ((GetCurrentWindowText().Length - m_iCurrentCharElement) > 17)
		{
			string NameRegexPattern		= GetNextCharacters(17);
			SpokesPersonState eSpeaker	= IsNewSpeakerCommand(NameRegexPattern);
			if (eSpeaker != m_eSpeaker)
			{
				m_eSpeaker = eSpeaker;
				GetSpeakerNameScript().text = GetSpeakerName();
			}
		}


        return (GetCurrentWindowText().Length > m_iCurrentCharElement) ? GetCurrentWindowText()[m_iCurrentCharElement].ToString() : "";
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get X Amount of Next Characters
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetNextCharacters(int Amount)
	{
        return GetCurrentWindowText().Substring(m_iCurrentCharElement, Amount);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove X Amount of Characters
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RemoveCharacters(int StartPoint, int Amount)
	{
		m_aTextArray[m_aTextArray.m_iCurrentElement] = m_aTextArray[m_aTextArray.m_iCurrentElement].Remove(StartPoint, Amount);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Label Script
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private UILabel GetLabelScript()
	{
		return GetComponent<UILabel>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Label Script
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private UILabel GetSpeakerNameScript()
	{
		return m_DialogueBoxObjects.m_SpeakerNameTextObject.GetComponent<UILabel>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Face Sprite Script
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private UISprite GetFaceSpriteScript()
	{
		return m_DialogueBoxObjects.m_FaceSpriteObject.GetComponent<UISprite>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Speaker Name
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetSpeakerName()
	{
		switch (m_eSpeaker)
		{
			case SpokesPersonState.PLAYER:		return " [FFAA00]Commander:[-]";
			case SpokesPersonState.MALE_NPC:	return " [FFAA00]Devdan:[-]";
			case SpokesPersonState.FEMALE_NPC:	return " [FFAA00]Sapphire:[-]";
			case SpokesPersonState.SARGE:		return " [FFAA00]Sargeant Nukem:[-]";
			default:							return " [FFAA00]Mission Control:[-]";
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Opened Mouth Sprite Name
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetOpenedMouthSpriteName()
	{
		switch (m_eSpeaker)
		{
			case SpokesPersonState.PLAYER:		return "PlayerOpen";
			case SpokesPersonState.MALE_NPC:	return "NPC1Open";
			case SpokesPersonState.FEMALE_NPC:	return "NPC2Open";
			case SpokesPersonState.SARGE:		return "SargeOpen";
			default:							return "GeneralOpen";
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Closed Mouth Sprite Name
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetClosedMouthSpriteName()
	{
		switch (m_eSpeaker)
		{
			case SpokesPersonState.PLAYER:		return "PlayerClosed 1";
			case SpokesPersonState.MALE_NPC:	return "NPC1Closed";
			case SpokesPersonState.FEMALE_NPC:	return "NPC2Closed";
			case SpokesPersonState.SARGE:		return "SargeClosed";
			default:							return "GeneralClosed";
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Replace RGB Commands With Hex
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string ReplaceRGBCommandsWithHex(string Input)
	{
		string Text			= Input;
		string Regexpr		= "\\\\c\\[\\s?(\\d+),\\s?(\\d+),\\s?(\\d+)\\]";
		Match RegexMatch	= Regex.Match(Text, Regexpr, RegexOptions.IgnoreCase);

		while (RegexMatch.Success)
		{
			string sHexValue = "[";
			for (int i = 1; i < 4; ++i)
			{
				CaptureCollection Collection = RegexMatch.Groups[i].Captures;
				for (int j = 0; j < Collection.Count; ++j)
				{
					sHexValue += System.Convert.ToByte(Collection[j].ToString()).ToString("x2").ToUpper();
				}
			}
			Text = Text.Replace(RegexMatch.Value, sHexValue + "]");
			RegexMatch = RegexMatch.NextMatch();
		}

		return Text;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Replace ENDL
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string[] ReplaceENDL(string Input)
	{
        return Regex.Split(Input, "\\\\endl(?:\\\n)*", RegexOptions.IgnoreCase);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Hex Command?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool IsHexCommand(string Text)
	{
		return Text.Contains("[-]");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is an RGB Hex Value?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool IsRGBHexValue(string Text)
	{
		return (Regex.Match(Text, "\\[[a-fA-F0-9]{6}\\]").Success);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Pause Command
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool IsPauseCommand(string Text)
	{
		return Text.ToUpper().Contains("\\<PAUSE>");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is New Speaker Command?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private SpokesPersonState IsNewSpeakerCommand(string Text)
	{
		string Regexpr					= "\\\\SP<(\\w+)>(?:\\\n)*";
		Match RegexMatch				= Regex.Match(Text, Regexpr, RegexOptions.IgnoreCase);
		string NameCapture				= "";
		if (RegexMatch.Groups.Count > 1)
		{
			NameCapture = RegexMatch.Groups[1].Captures[0].ToString().ToUpper();
            m_aTextArray[m_aTextArray.m_iCurrentElement] = m_aTextArray[m_aTextArray.m_iCurrentElement].Replace(RegexMatch.Value, "");
		}

		switch (NameCapture)
		{
			case "PLAYER"		: return SpokesPersonState.PLAYER;
			case "MALE_NPC"		: return SpokesPersonState.MALE_NPC;
			case "FEMALE_NPC"	: return SpokesPersonState.FEMALE_NPC;
			case "SARGE"		: return SpokesPersonState.SARGE;
			case "GENERAL"		: return SpokesPersonState.GENERAL;
			default				: return m_eSpeaker;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Pause Conditions
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void SetPauseConditions(KeyCode[] Keys, XboxInputHandler.Controls[] Axis)
	{
		m_PauseTrigger.Keys		= Keys;
		m_PauseTrigger.JoyAxis	= Axis;
	}
}
