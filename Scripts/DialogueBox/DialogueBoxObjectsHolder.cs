//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             Dialogue Box Objects Holder
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script simply holds objects of interest to the DialogueBox Sript
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class DialogueBoxObjectsHolder : MonoBehaviour 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_DialogueBoxParent;
    public GameObject m_TransBackground;
    public GameObject m_TextBackground;
    public GameObject m_FaceBackground;
    public GameObject m_FaceSpriteObject;
	public GameObject m_SpeakerNameTextObject;
    public GameObject m_TextObject;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Dialogue Box GameObjects
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public GameObject[] GetObjects()
    {
		return new GameObject[] { m_TransBackground, m_TextBackground, m_FaceBackground, m_FaceSpriteObject, m_SpeakerNameTextObject, m_TextObject };
    }
}
