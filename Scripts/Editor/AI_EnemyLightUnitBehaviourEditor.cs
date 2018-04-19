//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Enemy Light Unit Behaviour Editor
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script simply creates a custom unity inspector editor for the
//#		AI_EnemyLightUnitBehaviour script. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AI_EnemyLightUnitBehaviour))]
public class AI_EnemyLightUnitBehaviourEditor : Editor
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnInspectorGUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeInspector();
		AI_EnemyLightUnitBehaviour pTarget = target as AI_EnemyLightUnitBehaviour;

		pTarget.m_HissSound = EditorGUILayout.ObjectField("Hiss Audio Clip: ", pTarget.m_HissSound, typeof(AudioClip), true) as AudioClip;
		pTarget.m_iHealth = EditorGUILayout.IntField("Health:", pTarget.m_iHealth);
		pTarget.m_iBaseScore = EditorGUILayout.IntField("Awarded Score When Killed:", pTarget.m_iBaseScore);
		pTarget.m_fOutputDamage = EditorGUILayout.FloatField("Attack Damage:", pTarget.m_fOutputDamage);
		if (pTarget.m_ePathChoosing != AI_EnemyLightUnitBehaviour.PathChoosing.FOLLOW_PLAYER)
		{
			pTarget.m_fCollisionDamage = EditorGUILayout.FloatField("Collision Damage:", pTarget.m_fCollisionDamage);
			pTarget.m_fMovementSpeed = EditorGUILayout.FloatField("Movement Speed:", pTarget.m_fMovementSpeed);
		}
		pTarget.m_fBulletSpeed = EditorGUILayout.FloatField("Acid Speed:", pTarget.m_fBulletSpeed);
		pTarget.m_fBulletCooldownTime = EditorGUILayout.FloatField("Acid Cooldown Time:", pTarget.m_fBulletCooldownTime);
		pTarget.m_goBullet = EditorGUILayout.ObjectField("Acid Prefab:", pTarget.m_goBullet, typeof(GameObject), true) as GameObject;
		pTarget.m_goUnitHead = EditorGUILayout.ObjectField("Unit's Head Object:", pTarget.m_goUnitHead, typeof(GameObject), true) as GameObject;

		EditorGUIUtility.LookLikeControls();
		pTarget.m_ePathChoosing = (AI_EnemyLightUnitBehaviour.PathChoosing)EditorGUILayout.EnumPopup("Path Choosing:", pTarget.m_ePathChoosing);
		EditorGUIUtility.LookLikeInspector();

		EditorGUI.indentLevel += 1;
		{
			// If Following Player, Show Additional Params
			if (pTarget.m_ePathChoosing == AI_EnemyLightUnitBehaviour.PathChoosing.FOLLOW_PLAYER)
			{
				EditorGUILayout.LabelField("Random Time In Front of Player", EditorStyles.boldLabel);
				EditorGUI.indentLevel += 1;
				{
					pTarget.m_fFollowPlayerTimeBegin = EditorGUILayout.FloatField("Range Begin:", pTarget.m_fFollowPlayerTimeBegin);
					pTarget.m_fFollowPlayerTimeEnd = EditorGUILayout.FloatField("Range End:", pTarget.m_fFollowPlayerTimeEnd);
				}
				EditorGUI.indentLevel -= 1;

				EditorGUILayout.LabelField("Random Unit Speed", EditorStyles.boldLabel);
				EditorGUI.indentLevel += 1;
				{
					pTarget.m_fPlayerFollowSpeedRangeBegin = EditorGUILayout.FloatField("Range Begin:", pTarget.m_fPlayerFollowSpeedRangeBegin);
					pTarget.m_fPlayerFollowSpeedRangeEnd = EditorGUILayout.FloatField("Range End:", pTarget.m_fPlayerFollowSpeedRangeEnd);
				}
				EditorGUI.indentLevel -= 1;
			}


			else
			{
				// New Option
				pTarget.m_fDirectionMovementOffset = EditorGUILayout.FloatField("Direction Movement Offset", pTarget.m_fDirectionMovementOffset);

				// If Moving via BEZIER_CURVES
				if (pTarget.m_ePathChoosing == AI_EnemyLightUnitBehaviour.PathChoosing.BEZIER_CURVES)
				{
					pTarget.m_iSmoothnessRating = EditorGUILayout.IntField("Smoothness Rating", pTarget.m_iSmoothnessRating);
				}

				EditorGUIUtility.LookLikeControls();
				// Modify Array	::	Code By: Darclaw	~  http://answers.unity3d.com/questions/26207/how-can-i-recreate-the-array-inspector-element-for.html#answer-220601
				SerializedProperty tps = serializedObject.FindProperty("m_PathPoints");
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(tps, true);
				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
				}
				EditorGUIUtility.LookLikeInspector();
			}
		}
		EditorGUI.indentLevel -= 1;



		if (GUI.changed)
		{
			EditorUtility.SetDirty(pTarget);
		}
	}
}