using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RemoveComponents))]
public class RemoveComponentsEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		RemoveComponents pTarget = target as RemoveComponents;

		SerializedProperty PauseKeys = serializedObject.FindProperty("Parents");
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(PauseKeys, true);
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(pTarget);
			this.Repaint();
		}

		if( GUILayout.Button( "Remove Animator Components" ) )
		{
			foreach (GameObject Obj in pTarget.Parents)
			{
				RemoveAnimatorComponents(Obj.transform);
			}
		}

		if( GUILayout.Button( "Remove Terrain Collider Components" ) )
		{
			foreach (GameObject Obj in pTarget.Parents)
			{
				RemoveTerrainColliderComponents(Obj.transform);
			}
		}

		if( GUILayout.Button("Remove Colliders") )
		{
			foreach (GameObject Obj in pTarget.Parents)
			{
				RemoveColliderComponents(Obj.transform);
			}
		}
	}

	void RemoveAnimatorComponents(Transform Parent)
	{
		foreach( Transform Child in Parent )
		{
			RemoveAnimatorComponents(Child);
		}
		
		if( Parent.GetComponent<Animator>() != null )
		{
			DestroyImmediate(Parent.GetComponent<Animator>());
		}
	}

	void RemoveTerrainColliderComponents(Transform Parent)
	{
		foreach (Transform Child in Parent)
		{
			RemoveTerrainColliderComponents(Child);
		}

		if (Parent.GetComponent<TerrainCollider>() != null)
		{
			DestroyImmediate(Parent.GetComponent<TerrainCollider>());
		}
	}

	void RemoveColliderComponents(Transform Parent)
	{
		foreach (Transform Child in Parent)
		{
			RemoveColliderComponents(Child);
		}

		if (Parent.GetComponent<Collider>() != null)
		{
			DestroyImmediate(Parent.GetComponent<Collider>());
		}
	}
}
