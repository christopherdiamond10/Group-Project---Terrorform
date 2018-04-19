//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Xbox Controller Input Handler
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script handles the input received from an Xbox controller.
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class XboxInputHandler : MonoBehaviour 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*{} Class Declarations
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public enum Controls
    {
        A = 0,
        B,               
        X,               
        Y,               
        LB,              
        RB,             
        LeftClick,       
        RightClick,      
        LeftTrigger,     
        RightTrigger,    

        Back,   
        Start,           

        DPad_Left,       
        DPad_Right,      
        DPad_Up,         
        DPad_Down,       
    };
    

    public enum AnalogStick
    {
        Left,
        Right,
    };


    public enum Direction
    {
        Horizontal,
        Vertical,
    };


    private struct KeyInfo
    {
        public bool  KeyTriggered;
        public bool  KeyDown;
        public float CurrentHoldTime;

        public void ResetInput()
        {
            KeyTriggered = false;
            KeyDown = false;
        }

        public void StartInput()
        {
            KeyTriggered = true;
            KeyDown = true;
            CurrentHoldTime = 0.0f;
        }

        public void TurnOffTrigger()
        {
            KeyTriggered = false;
        }
    };
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static KeyInfo[]    m_aKeyInputArray        = new KeyInfo[16];

    private const string m_sXboxA                = "Xbox_A";
    private const string m_sXboxB                = "Xbox_B";
    private const string m_sXboxX                = "Xbox_X";
    private const string m_sXboxY                = "Xbox_Y";
    private const string m_sXboxLB               = "Xbox_LB";
    private const string m_sXboxRB               = "Xbox_RB";
    private const string m_sXboxLC               = "Xbox_LeftClick";
    private const string m_sXboxRC               = "Xbox_RightClick";
    private const string m_sXboxTriggers         = "Xbox_Triggers";
    private const string m_sXboxBack             = "Xbox_Back";
    private const string m_sXboxStart            = "Xbox_Start";
    private const string m_sDPadHorizontal       = "Xbox_DPadHorizontal";
    private const string m_sDPadVertical         = "Xbox_DPadVertical";
    private const string m_sLeftStickHorizontal  = "Xbox_LeftStickHorizontal";
    private const string m_sLeftStickVertical    = "Xbox_LeftStickVertical";
    private const string m_sRightStickHorizontal = "Xbox_RightStickHorizontal";
    private const string m_sRightStickVertical   = "Xbox_RightStickVertical";
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Start()
    {
		ResetInput();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Redefined Method: Update
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void Update()
    {
		UpdateInput();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void ResetInput()
	{
		for( int i = 0; i < m_aKeyInputArray.Length; ++i )
        {
            m_aKeyInputArray[i].ResetInput();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Input
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void UpdateInput()
	{
		for( int i = 0; i < m_aKeyInputArray.Length; ++i )
        {
            if( IsKeyDown((Controls)i) )
            {
                if( m_aKeyInputArray[i].KeyDown )
                {
                    m_aKeyInputArray[i].TurnOffTrigger();
                }
                else
                {
                    m_aKeyInputArray[i].StartInput();
                }
            }
            else
            {
                m_aKeyInputArray[i].ResetInput();
            }
        }
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Is Key Down?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static bool IsKeyDown(Controls eKey)
    {
        switch( eKey )
        {
            case Controls.A:            return Input.GetAxisRaw(m_sXboxA)           >  0.5f;
            case Controls.B:            return Input.GetAxisRaw(m_sXboxB)           >  0.5f;
            case Controls.X:            return Input.GetAxisRaw(m_sXboxX)           >  0.5f;
            case Controls.Y:            return Input.GetAxisRaw(m_sXboxY)           >  0.5f;
            case Controls.LB:           return Input.GetAxisRaw(m_sXboxLB)          >  0.5f;
            case Controls.RB:           return Input.GetAxisRaw(m_sXboxRB)          >  0.5f;
            case Controls.LeftClick:    return Input.GetAxisRaw(m_sXboxLC)          >  0.5f;
            case Controls.RightClick:   return Input.GetAxisRaw(m_sXboxRC)          >  0.5f;
            case Controls.LeftTrigger:  return Input.GetAxisRaw(m_sXboxTriggers)    >  0.5f;
            case Controls.RightTrigger: return Input.GetAxisRaw(m_sXboxTriggers)    < -0.5f;
            case Controls.Back:         return Input.GetAxisRaw(m_sXboxBack)        >  0.5f;
            case Controls.Start:        return Input.GetAxisRaw(m_sXboxStart)       >  0.5f;
            case Controls.DPad_Left:    return Input.GetAxisRaw(m_sDPadHorizontal)  < -0.5f;
            case Controls.DPad_Right:   return Input.GetAxisRaw(m_sDPadHorizontal)  >  0.5f;
            case Controls.DPad_Up:      return Input.GetAxisRaw(m_sDPadVertical)    <  0.5f;
            default:                    return Input.GetAxisRaw(m_sDPadVertical)    > -0.5f;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Is Key Triggered?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static bool KeyTriggered(Controls eKey)
    {
        return m_aKeyInputArray[ (int)eKey ].KeyTriggered;
    }

    public static bool KeyTriggered(Controls[] eKeys)
    {
        foreach (Controls Key in eKeys)
        {
            if (KeyTriggered(Key))
                return true;
        }
        return false;
    }

    public static bool KeysTriggered(Controls[] eKeys)
    {
        foreach (Controls Key in eKeys)
        {
            if (!KeyTriggered(Key))
                return false;
        }
        return true;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Is Key Pressed?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static bool KeyPressed(Controls eKey)
    {
        return m_aKeyInputArray[ (int)eKey ].KeyDown;
    }

    public static bool KeyPressed(Controls[] eKeys)
    {
        foreach (Controls Key in eKeys)
        {
            if (KeyPressed(Key))
                return true;
        }
        return false;
    }

    public static bool KeysPressed(Controls[] eKeys)
    {
        foreach (Controls Key in eKeys)
        {
            if (!KeyPressed(Key))
                return false;
        }
        return true;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Is Key Released?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static bool KeyReleased(Controls eKey)
    {
        return !KeyPressed(eKey);
    }

    public static bool KeyReleased(Controls[] eKeys)
    {
        return !KeyPressed(eKeys);
    }

    public static bool KeysReleased(Controls[] eKeys)
    {
        foreach (Controls Key in eKeys)
        {
            if (!KeyReleased(Key))
                return false;
        }
        return true;
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Analog Stick Axis
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static float GetAnalogAxis(AnalogStick eStick, Direction eDir)
    {
        if (eStick == AnalogStick.Left)
        {
            if (eDir == Direction.Horizontal)
            {
                return Input.GetAxisRaw(m_sLeftStickHorizontal);
            }
            else
            {
                return Input.GetAxisRaw(m_sLeftStickVertical);
            }
        }

        else
        {
            if (eDir == Direction.Horizontal)
            {
                return Input.GetAxisRaw(m_sRightStickHorizontal);
            }
            else
            {
                return Input.GetAxisRaw(m_sRightStickVertical);
            }
        }
    }
}
