//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//#             AI Explosive Unit Editor
//#             Version: 1.0
//#             Author: Christopher Diamond
//#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#  Description:
//#
//#    This Script simply creates a custom unity inspector editor for the
//#		AI_ExplosiveUnit script. 
//#
//#=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AI_ExplosiveUnit))]
public class AI_ExplosiveUnitEditor : Editor
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnInspectorGUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeInspector();
		AI_ExplosiveUnit pTarget = target as AI_ExplosiveUnit;

		pTarget.m_AttachHissSound = EditorGUILayout.ObjectField("Hiss Audio Clip: ", pTarget.m_AttachHissSound, typeof(AudioClip), true) as AudioClip;
		pTarget.m_iHealth = EditorGUILayout.IntField("Health:", pTarget.m_iHealth);
		pTarget.m_iBaseScore = EditorGUILayout.IntField("Awarded Score When Killed:", pTarget.m_iBaseScore);
		pTarget.m_fOutputDamage = EditorGUILayout.FloatField("Explosion Damage:", pTarget.m_fOutputDamage);
		pTarget.m_fCollisionDamage = EditorGUILayout.FloatField("Collision Damage:", pTarget.m_fCollisionDamage);
		pTarget.m_fRotationSpeed = EditorGUILayout.FloatField("Rotation Speed:", pTarget.m_fRotationSpeed);
		//pTarget.m_iChanceToHitPlayer	= EditorGUILayout.IntField(		"Chance To hit Player",			pTarget.m_iChanceToHitPlayer);
		pTarget.m_goExplosionPrefab = EditorGUILayout.ObjectField("Explosion Prefab:", pTarget.m_goExplosionPrefab, typeof(GameObject), true) as GameObject;
		pTarget.m_fJumpVelocity = EditorGUILayout.FloatField("Jump Velocity:", pTarget.m_fJumpVelocity);

		EditorGUIUtility.LookLikeControls();
		pTarget.m_eAttackState = (AI_ExplosiveUnit.AttackState)EditorGUILayout.EnumPopup("Attack State:", pTarget.m_eAttackState);
		EditorGUIUtility.LookLikeInspector();

		if (GUI.changed)
		{
			EditorUtility.SetDirty(pTarget);
		}
	}
}
