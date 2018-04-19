using UnityEngine;
using System.Collections;

public class CheckPointsScript : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnCollisionEnter(Collision collision)
	{
		if (CollidedWithPlayer(collision.transform.tag))
		{
			collision.gameObject.GetComponent<PlayerCheckPointsSystem>().SetupCheckPoint();
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
	//	* New Methods: Get Gizmo Colours
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static Color GetCubeColour()
	{
		return new Color(0.9019f, 0.8325f, 0.0941f, 0.5f);			// Yellowish Colour
	}
}
