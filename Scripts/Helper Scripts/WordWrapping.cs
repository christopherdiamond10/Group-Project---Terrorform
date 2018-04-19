//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Word Wrapping
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 1, 2013
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script contains the word wrapper (paragraph formatter) logic. This will 
//	  convert text into respective lines to fit a rectangle. Should the text exceed
//	  the rect height, the word wrapper will add the remaining text onto an external
//	  element and continue word-wrapping.
//	  After the text has been completely covered, the script will return the text as 
//	  either a whole string or a list, with each element signifying the text to be
//	  displayed per dialogue box entry.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordWrapping
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private string			sText;
	private List<string>	lDialoguePerBox;
    private char			cCurrentCharacter;
	private int				iCurrentTextStartElement;
    private int				iCurrentTextElement;
    private int				iCurrentPositionX;
    private int				iStartPositionX;
	private int				iStartPositionY;
    private int				iEndPositionX;
	private int				iEndPositionY;
    private GUIStyle		oStyle;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	** Constructor
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private WordWrapping(string Text, int StartPositionX, int EndPositionX, Font FontType, int FontSize, FontStyle FStyle)
    {
		SetupWordWrapper(Text, StartPositionX, EndPositionX, 0, 1000000, FontType, FontSize, FStyle);
    }

	private WordWrapping(string Text, int StartPositionX, int EndPositionX, int StartPositionY, int EndPositionY, Font FontType, int FontSize, FontStyle FStyle)
	{
		SetupWordWrapper(Text, StartPositionX, EndPositionX, StartPositionY, EndPositionY, FontType, FontSize, FStyle);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup WordWrapper
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupWordWrapper(string Text, int StartPositionX, int EndPositionX, int StartPositionY, int EndPositionY, Font FontType, int FontSize, FontStyle FStyle)
	{
		sText						= Text;
        iCurrentPositionX			= StartPositionX;
        iStartPositionX				= StartPositionX;
		iStartPositionY				= StartPositionY;
        iEndPositionX				= EndPositionX;
		iEndPositionY				= EndPositionY;
		iCurrentTextStartElement	= 0;
		iCurrentTextElement			= 0;
        cCurrentCharacter			= ' ';
        oStyle						= new GUIStyle();
        oStyle.font					= FontType;
        oStyle.fontSize				= FontSize;
		oStyle.fontStyle			= FStyle;
		lDialoguePerBox				= new List<string>();
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Increase Position
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void IncreasePosition()
    {
        iCurrentPositionX += GetCharWidth();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: At End of Line?
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool ReachedEndOfLine()
    {
        return (iCurrentPositionX >= iEndPositionX);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: At End of Window?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool ReachedEndOfWindow()
	{
		return ((iStartPositionY + GetCharHeight()) >= iEndPositionY);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Handle End Of Line
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void HandleEndOfLine()
    {
        int iCurrentElement = iCurrentTextElement;
		while (iCurrentTextElement > iCurrentTextStartElement)
        {
            --iCurrentTextElement;

            if (sText[iCurrentTextElement] == ' ')
            {
                sText = sText.Insert(iCurrentTextElement + 1, "\n");
                iCurrentPositionX    = iStartPositionX;
                return;
            }
        }
        iCurrentTextElement = iCurrentElement;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Char Width (WordWrapping)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int GetCharWidth()
	{
		return (int)oStyle.CalcSize(new GUIContent("" + cCurrentCharacter)).x;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Char Height (WordWrapping)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int GetCharHeight()
	{
		return (int)oStyle.CalcSize(new GUIContent(GetCurrentString(iCurrentTextStartElement))).y;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current String (WordWrapping)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetCurrentString(int StartingElement = 0)
	{
		string sCurrentText = "";
		for (int i = StartingElement; i < iCurrentTextElement; ++i)
		{
			sCurrentText += sText[i];
		}

		return sCurrentText;
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Start WordWrapping
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void StartWordWrapping()
    {
        for ( ; iCurrentTextElement < sText.Length; ++iCurrentTextElement)
        {
            cCurrentCharacter = sText[iCurrentTextElement];
			
            // If New Line Text, go to New Line
            if (sText[iCurrentTextElement] == '\n')
            {
                iCurrentPositionX = iStartPositionX;

				if (ReachedEndOfWindow())
				{
					lDialoguePerBox.Add(GetCurrentString(iCurrentTextStartElement));
					iCurrentTextStartElement = iCurrentTextElement + 1;
				}

                continue;
            }

            // Otherwise Increase Position. And if at end of line, create new line from start of most recent word
            IncreasePosition();
            if (ReachedEndOfLine())
            {
                HandleEndOfLine();
            }
        }

		lDialoguePerBox.Add(GetCurrentString(iCurrentTextStartElement));
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get WordWrapping
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	static public string GetWordWrapping(string Text, int StartPositionX, int EndPositionX, Font FontType, int FontSize, FontStyle FStyle = FontStyle.Normal)
	{
		WordWrapping WW = new WordWrapping(Text, StartPositionX, EndPositionX, FontType, FontSize, FStyle);
		WW.StartWordWrapping();

		string sWordWrappedText = "";
		foreach (string BoxText in WW.lDialoguePerBox)
		{
			sWordWrappedText += BoxText;
		}

		return sWordWrappedText;
	}

	static public List<string> GetWordWrapping(string Text, int StartPositionX, int EndPositionX, int StartPositionY, int EndPositionY, Font FontType, int FontSize, FontStyle FStyle = FontStyle.Normal)
	{
		WordWrapping WW = new WordWrapping(Text, StartPositionX, EndPositionX, StartPositionY, EndPositionY, FontType, FontSize, FStyle);
		WW.StartWordWrapping();
		return WW.lDialoguePerBox;
	}
};
