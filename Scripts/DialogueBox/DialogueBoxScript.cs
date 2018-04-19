//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Dialogue Box Script
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: June 14, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//		This Script controls the Dialogue Boxes. The script will handle the 
//		Textures as well as the texts that goes into making dialogue possible.
//		Wordwrapping is also used in this process but is handled by a helper script.
//
//-------------------------------------------------------------------------------
//	Instructions:
//
//	~	This script will need to used as a prefab to be properly set up with Textures.
//
//	~	You may insert Text via the Inspector, but it would probably be wiser to 
//		do so by instantiating the prefab object of this and then calling:
//			SetDialogueText( Text ) 
//		
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DialogueBoxScript : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{}	Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TextDisplayMode
	{
		SLOW,       // 0.5          - 0.5
        NORMAL,     // 1.0          
		FAST,       // 3.0          + 2.0
		FASTER,     // 6.0          + 3.0
        VERYFAST,   // 10.0         + 4.0
        SUPERFAST,  // 15.0         + 5.0
        ULTRAFAST,  // 21.0         + 6.0
		IMMEDIATE,
	};

	private class DialogueBoxTextControl
	{
		public int			iCurrentElement = 0;
		public List<string> lPerBoxDialogue = new List<string>();

		public string GetCurrentText() 
		{
			return (lPerBoxDialogue.Count > iCurrentElement) ? lPerBoxDialogue[iCurrentElement] : ""; 
		}

		public char GetTextElement(int Element)
		{
			return (GetCurrentText().Length > Element) ? GetCurrentText()[Element] : ' ';
		}

		public void ProceedToNextBox()
		{
			++iCurrentElement;
		}

		public bool CompletedText()
		{
			return (iCurrentElement >= lPerBoxDialogue.Count);
		}
	};
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public Texture2D        m_DialogueBoxTexture;										// DialogueBox Background Texture
    public Texture2D        m_FaceTexture;												// Texture for the Face

	public Font				m_FontSheet;												// The FontSheet to be used
    public Color            m_FontColour                    = Color.white;              // Colour of the Font
	public int				m_iFontSize						= 18;						// The Size of the Font
	public FontStyle		m_FontStyle						= FontStyle.Normal;			// The Style of the Font
    public TextDisplayMode  m_eDisplayMode					= TextDisplayMode.FAST;     // Text Display Mode. SLOW = DefaultSpeed, MEDIUM = DefaultSpeed * 2, FAST = DefaultSpeed * 3, IMMEDIATE = Display all text at once.

    public int              m_iFacesetCount					= 1;                        // How Many Faces (in horizontal axis) are in this Spritesheet?   
    public float            m_fFaceAnimationSpeed			= 0.3f;						// The Speed of the Face Animation as it's Speaking (in seconds)

    public float            m_fTextDisplayDefaultSpeed		= 0.0001f;					// Default Speed of Text (also in seconds)
	public float			m_fCompletedDialogueWaitTimer	= 5.0f;						// How long (in seconds) until the DialogueBox disappears or moves on to the next entry AFTER it has completely displayed all text?
	
	public string			m_sDialogue;												// The Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private GameObject				m_GUIText;											// The Game Object for the Text
    private GameObject				m_DialogueBox;										// The Game Object for the DialogueBox Background Texture
    private GameObject				m_Face;												// The Game Object for the Face Display Texture

	private DialogueBoxTextControl  m_DBTextControl;									// The Controller for the Display Text 
	private TimeTracker				m_TimeToDisplayNextTextCharacter;					// The TimeTracker until the next Character in the Textbox can be displayed
	private TimeTracker				m_TimeToDisplayNextTextBox;							// The TimeTracker until the next textbox can be displayed after full showing all currently available text.

    private int						m_iCurrentStringElement;							// Current Char in String the GUIText is on.        (used to advance text if not displayed in IMMEDIATE mode)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
        CreateDialogueBox();
        CreateFaceTexture();
        CreateGUIText();
		CreateTextTimeTracker();
		CreateDialogueBoxTimeTracker();
		CreateTextController();

		SetVisibility( false );
		SetDialogueText( m_sDialogue, m_eDisplayMode );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		if (GetDialogueCurrentText() != m_DBTextControl.GetCurrentText())
		{
			if (m_eDisplayMode == TextDisplayMode.IMMEDIATE)
			{
				GetDialogueGUIText().text = m_DBTextControl.GetCurrentText();
			}
			else
			{
				UpdateTextDisplay();
			}
		}
		else
		{
			UpdateDialogueBox();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Text Display
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateTextDisplay()
	{
		UpdateTextDisplayTimer();										// Update Timer
		if( CanShowNextCharacter() )									// Can Next Character be shown now?
		{
			GetDialogueGUIText().text += GetCurrentStringElement();		// Increase Displayed Text
			IncrementStringElement();									// Go to next Character
			ResetTimers();												// Reset Time
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update DialogueBox
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDialogueBox()
	{
		UpdateDialogueDisplayTimer();
		if( CanShowNextDialogueBox() )
		{
			m_DBTextControl.ProceedToNextBox();
			GetDialogueGUIText().text = "";
			if( m_DBTextControl.CompletedText() )
			{
				DestroySelf();
			}
			ResetTimers();
			m_iCurrentStringElement = 0;
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Update Text Display Timer
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void UpdateTextDisplayTimer()
    {
        m_TimeToDisplayNextTextCharacter.Update( GetDisplayModeAsFloat() );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Update Dialogue Display Timer
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void UpdateDialogueDisplayTimer()
    {
		m_TimeToDisplayNextTextBox.Update();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Reset
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void Reset()
    {
		m_DBTextControl.lPerBoxDialogue.Clear();
        m_iCurrentStringElement		= 0;

		ResetTimers();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Create Dialogue Box
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void CreateDialogueBox()
    {
        if (m_DialogueBoxTexture != null)
        {
            m_DialogueBox									= new GameObject();
            m_DialogueBox.name								= "DialogueBoxTexture";
			m_DialogueBox.AddComponent< GUITexture >();
			GetDialogueBoxInstance().texture				= m_DialogueBoxTexture;
			GetDialogueBoxInstance().pixelInset				= CreateRect(Screen.width, (int)(Screen.height * 0.2f)); 
			GetDialogueBoxInstance().transform.position		= new Vector3(0.0f, 0.0f, 2.0f);
			GetDialogueBoxInstance().transform.localScale	= new Vector3(0.0f, 0.0f, 0.0f);

			Color Colour									= GetDialogueBoxInstance().color;
			Colour.a										= 0.7f;
			GetDialogueBoxInstance().color					= Colour;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Create Face Texture
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void CreateFaceTexture()
    {
		if (m_FaceTexture != null)
        {
			m_Face										= new GameObject();
            m_Face.name									= "DialogueFaceTexture";
            m_Face.transform.parent						= ((m_DialogueBox != null) ? m_DialogueBox.transform : null);
			m_Face.AddComponent<GUITexture>();
			GetFaceGUIInstance().texture				= m_FaceTexture;

            if (GetDialogueBoxInstance() != null)
            {
                float X									= (GetDialogueBoxContentsBeginOffset().x);
                float Y									= (GetDialogueBoxInstance().pixelInset.yMin + GetDialogueBoxInstance().pixelInset.height * 0.05f);
                float W									= (GetDialogueBoxInstance().pixelInset.xMax - GetDialogueBoxInstance().pixelInset.xMin) * 0.1f;
                float H									= (GetDialogueBoxInstance().pixelInset.yMax - GetDialogueBoxInstance().pixelInset.yMin) * 0.8f;
                GetFaceGUIInstance().pixelInset			= new Rect(X, Y, W, H);
            }
            else
            {
                GetFaceGUIInstance().pixelInset			= CreateRect((int)(Screen.width * 0.2f), (int)(Screen.height * 0.2f)); 
            }

			GetFaceGUIInstance().transform.position		= new Vector3(0.0f, 0.0f, 3.0f);
			GetFaceGUIInstance().transform.localScale	= new Vector3(0.0f, 0.0f, 0.0f);

            Color Colour								= GetFaceGUIInstance().color;
            Colour.a									= 0.7f;
            GetFaceGUIInstance().color					= Colour;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Create GUI Text
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void CreateGUIText()
    {
		m_GUIText							= new GameObject();
        m_GUIText.name						= "DialogueBoxText";
        m_GUIText.transform.position		= new Vector3(0.0f, 0.0f, 4.0f);
        m_GUIText.transform.parent			= ((m_DialogueBox != null) ? m_DialogueBox.transform : (m_Face != null) ? m_Face.transform : null);
		m_GUIText.AddComponent<GUIText>();
		GetDialogueGUIText().anchor			= TextAnchor.UpperLeft;
		GetDialogueGUIText().alignment		= TextAlignment.Left;
        GetDialogueGUIText().fontSize		= GetFontSizeScale( m_iFontSize );
		GetDialogueGUIText().font			= m_FontSheet;
        GetDialogueGUIText().color          = m_FontColour;
		GetDialogueGUIText().fontStyle		= m_FontStyle;
        GetDialogueGUIText().pixelOffset	= GetTextPixelOffset();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Text TimeTracker
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateTextTimeTracker()
	{
		m_TimeToDisplayNextTextCharacter = new TimeTracker(m_fTextDisplayDefaultSpeed, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create DialogueBox TimeTracker
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateDialogueBoxTimeTracker()
	{
		m_TimeToDisplayNextTextBox = new TimeTracker(m_fCompletedDialogueWaitTimer, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Text Controller
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateTextController()
	{
		m_DBTextControl = new DialogueBoxTextControl();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Font Size Scale
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private int GetFontSizeScale(int FontSize)
    {
        return FontSize; // Having Issues Figuring this out. Supposed to Upscale Text with Screen Resolution
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Contents Offset
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Vector2 GetDialogueBoxContentsBeginOffset()
    {
		if (GetDialogueBoxInstance() != null)
		{
			return new Vector2(
								 (GetDialogueBoxInstance().pixelInset.xMin + (GetDialogueBoxInstance().pixelInset.width * 0.08f)),
								 (GetDialogueBoxInstance().pixelInset.yMax - GetDialogueBoxInstance().pixelInset.yMin)
							  );
		}
		return new Vector2(0.0f, 0.0f);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Contents Offset (offset with face)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector2 GetFaceTextureTextBeginOffset()
	{
		if (GetFaceGUIInstance() != null)
		{
			Vector2 vContentsBegin = GetDialogueBoxContentsBeginOffset();
			return new Vector2(
								 (vContentsBegin.x + (GetFaceGUIInstance().pixelInset.width * 1.1f)),
								 (GetFaceGUIInstance().pixelInset.yMax)
							  );
		}
		return new Vector2(0.0f, 0.0f);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Text Pixel Offset
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private Vector2 GetTextPixelOffset()
    {
        Vector2 vPixelOffset;

        if (GetFaceGUIInstance() != null)
        {
			vPixelOffset = GetFaceTextureTextBeginOffset();
        }
        else if ( GetDialogueBoxInstance() != null )
        {
            vPixelOffset = GetDialogueBoxContentsBeginOffset();
        }
        else
        {
            vPixelOffset = new Vector2((int)(Screen.width * 0.2f), (int)(Screen.height * 0.2f));
        }
        return vPixelOffset;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Current String Element
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private char GetCurrentStringElement()
    {
        return m_DBTextControl.GetTextElement(m_iCurrentStringElement);
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Increment Current String Element
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void IncrementStringElement()
    {
       ++m_iCurrentStringElement;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Background Texture of Dialogue Box
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GUITexture GetDialogueBoxInstance()
	{
		return (m_DialogueBox != null) ? m_DialogueBox.GetComponent<GUITexture>() : null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Face Texture
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GUITexture GetFaceGUIInstance()
	{
		return (m_Face != null) ? m_Face.GetComponent<GUITexture>() : null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get GUI Text Instance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GUIText GetDialogueGUIText()
	{
        return m_GUIText.GetComponent< GUIText >();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Dialogue Current Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public string GetDialogueCurrentText()
	{
		return GetDialogueGUIText().text;	
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Display Mode (as Float)
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float GetDisplayModeAsFloat()
    {
        switch (m_eDisplayMode)
        {
            case TextDisplayMode.SLOW:      return 0.5f;
            case TextDisplayMode.NORMAL:    return 1.0f;
            case TextDisplayMode.FAST:      return 3.0f;
            case TextDisplayMode.FASTER:    return 6.0f;
            case TextDisplayMode.VERYFAST:  return 10.0f;
            case TextDisplayMode.SUPERFAST: return 15.0f;
            case TextDisplayMode.ULTRAFAST: return 21.0f;
            default:                        return 100000.0f;
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Timers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ResetTimers()
	{
		m_TimeToDisplayNextTextCharacter.Reset();
		m_TimeToDisplayNextTextBox.Reset();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Show Next Character?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool CanShowNextCharacter()
    {
        return m_TimeToDisplayNextTextCharacter.TimeUp();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Next DialogueBox?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool CanShowNextDialogueBox()
	{
		return m_TimeToDisplayNextTextBox.TimeUp();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Visibility
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetVisibility( bool Show )
	{
        if (GetDialogueBoxInstance() != null) { GetDialogueBoxInstance().enabled = Show; }
        if (GetFaceGUIInstance()     != null) { GetFaceGUIInstance().enabled     = Show; }
		GetDialogueGUIText().enabled = Show;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Rect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Rect CreateRect( int Width, int Height )
	{
		int X = (int)(Width * 0.1f);
		int Y = (int)(Height * 0.1f);
		int W = (Width - (X * 2));
		int H = (Height - (Y * 2));

		return new Rect(X, Y, W, H);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Dialogue
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetDialogueText( string Text, TextDisplayMode eDisplayMode = TextDisplayMode.SUPERFAST)
	{
		if( Text != "" )
		{
			m_DBTextControl.lPerBoxDialogue = GetWordWrappingString(Text);
			GetDialogueGUIText().text		= "";
			m_eDisplayMode					= eDisplayMode;

			SetVisibility( true );
			ResetTimers();
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get WordWrapping String
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private List<string> GetWordWrappingString(string Text)
    {
		int StartX	= (int)GetDialogueGUIText().pixelOffset.x;
		int StartY	= (m_DialogueBox != null) ? ((int)(GetDialogueBoxInstance().pixelInset.yMax)            - (int)GetDialogueGUIText().pixelOffset.y)	            : (int)GetDialogueGUIText().pixelOffset.y;
		int EndX    = (m_DialogueBox != null) ?  (int)(GetDialogueBoxInstance().pixelInset.xMin             + (GetDialogueBoxInstance().pixelInset.width * 0.8f))   : Screen.width  - (int)GetDialogueGUIText().pixelOffset.x;
		int EndY	= (m_DialogueBox != null) ? ((int)(GetDialogueBoxInstance().pixelInset.height * 0.9f)   - (int)GetDialogueBoxInstance().pixelInset.yMin)	    : Screen.height - (int)GetDialogueGUIText().pixelOffset.y;

		return WordWrapping.GetWordWrapping(Text, StartX, EndX, StartY, EndY, GetDialogueGUIText().font, GetDialogueGUIText().fontSize, GetDialogueGUIText().fontStyle);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Destroy Self
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DestroySelf()
	{
		if( m_GUIText != null )
			DestroyObject(m_GUIText);
		
		if( m_DialogueBox != null )
			DestroyObject(m_DialogueBox);	

		if( m_Face != null )
			DestroyObject(m_Face);	
		
		DestroyObject( gameObject );
	}
}
