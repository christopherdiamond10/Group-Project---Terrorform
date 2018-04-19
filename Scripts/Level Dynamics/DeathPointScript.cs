using UnityEngine;
using System.Collections;

public class DeathPointScript : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnCollisionEnter(Collision collision)
	{
		if (CollidedWithPlayer(collision.transform.tag))
		{
			collision.gameObject.GetComponent<PlayerCheckPointsSystem>().InvokeCheckPoint();
			DestroyObject(gameObject);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Collided With Player?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool CollidedWithPlayer(string Tag)
	{
		return (Tag == StaticKeywordsScript.PlayerColliderTag || Tag == StaticKeywordsScript.PlayerTag);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmos()
	{
		SphereCollider SCollider = GetComponent<SphereCollider>();
		Gizmos.color = GetCubeColour();
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.zero, new Vector3(SCollider.radius * 2, SCollider.radius * 2, 1));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Gizmo Colours
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static Color GetCubeColour()
	{
		return new Color(0.4901f, 0.2745f, 0.4705f, 0.5f);			// Purplish Colour
	}
}
