//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             DialogueBox Text Parser Editor Editor
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script simply creates a custom unity inspector editor for the
//#		DialogueboxTextParser script. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DialogueboxTextParser))]
public class DialogueboxTextParserEditor : Editor 
{
    private DialogueBox.SpokesPersonState m_eNextSpeaker = DialogueBox.SpokesPersonState.PLAYER;
    DialogueboxTextParser Target { get { return target as DialogueboxTextParser; } }

    Color m_Colour;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnInspectorGUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        EditorGUILayout.Space();
        Target.m_DialogueBoxScript = EditorGUILayout.ObjectField("Dialogue Box Script: ", Target.m_DialogueBoxScript, typeof(DialogueBox), true) as DialogueBox;

        EditorGUILayout.Space();
        Target.m_DialogueBoxScrollSpeed = EditorGUILayout.FloatField("Dialogue Box Scroll Speed: ", Target.m_DialogueBoxScrollSpeed);

        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.Space();
        Target.m_Speaker = (DialogueBox.SpokesPersonState)EditorGUILayout.EnumPopup("Starting Speaker: ", Target.m_Speaker);

        EditorGUILayout.Space();
        GUILayout.Label("Text");
        Target.m_sNewText = EditorGUILayout.TextArea(Target.m_sNewText, GUILayout.MaxHeight(150));


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("End Current Text Block", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        {
            EditorGUILayout.TextField("Command: ", "\\endl");
        }
        EditorGUI.indentLevel -= 1;


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Text Colour", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        {
            m_Colour = EditorGUILayout.ColorField("Colour: ", m_Colour);
            EditorGUILayout.TextField("Colour Command: ", GetColourCommand());
            EditorGUILayout.TextField("Revert Command: ", "[-]");
        }
        EditorGUI.indentLevel -= 1;
        
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Change Speaker", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        {
            m_eNextSpeaker = (DialogueBox.SpokesPersonState)EditorGUILayout.EnumPopup("New Speaker: ", m_eNextSpeaker);
            EditorGUILayout.TextField("Command: ", GetNextSpeakerCommand());
        }
        EditorGUI.indentLevel -= 1;


        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pause Game", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        {
            EditorGUILayout.TextField("Command: ", "\\<PAUSE>");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conditions:", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;
            {
                // Modify Array	::	Code By: Darclaw	~  http://answers.unity3d.com/questions/26207/how-can-i-recreate-the-array-inspector-element-for.html#answer-220601
                SerializedProperty PauseKeys = serializedObject.FindProperty("m_PauseKeys");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(PauseKeys, true);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.Space();
                // Modify Array	::	Code By: Darclaw	~  http://answers.unity3d.com/questions/26207/how-can-i-recreate-the-array-inspector-element-for.html#answer-220601
                SerializedProperty PauseAxis = serializedObject.FindProperty("m_PauseAxis");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(PauseAxis, true);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.indentLevel -= 1;




        if (GUI.changed)
        {
            EditorUtility.SetDirty(Target);
            this.Repaint();
        }
    }

    int GetCurrentTextCursorIndex()
    {
        return ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).pos;
    }

    void AddNewSpeaker()
    {
        Debug.Log(GetCurrentTextCursorIndex());
        Target.m_sNewText = Target.m_sNewText.Insert(GetCurrentTextCursorIndex(), GetNextSpeakerCommand());
        GUIUtility.keyboardControl = 0;
        GUIUtility.hotControl = 0;
    }

    string GetNextSpeakerCommand()
    {
        switch (m_eNextSpeaker)
        {
            case DialogueBox.SpokesPersonState.PLAYER:      return "\\SP<PLAYER>\n"     ;
            case DialogueBox.SpokesPersonState.GENERAL:     return "\\SP<GENERAL>\n"    ;
            case DialogueBox.SpokesPersonState.SARGE:       return "\\SP<SARGE>\n"      ;
            case DialogueBox.SpokesPersonState.MALE_NPC:    return "\\SP<MALE_NPC>\n"   ;
            default:                                        return "\\SP<FEMALE_NPC>\n" ;
        }
    }

    string GetColourCommand()
    {
        // Convert from Float colour values to int colour values, then convert to byte. Finally convert to Hex Value
        return
            "["                                                                               +
                System.Convert.ToByte(((int)(m_Colour.r * 255.0f)).ToString()).ToString("x2") +
                System.Convert.ToByte(((int)(m_Colour.g * 255.0f)).ToString()).ToString("x2") +
                System.Convert.ToByte(((int)(m_Colour.b * 255.0f)).ToString()).ToString("x2") +
            "]";
    }
}
