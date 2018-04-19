using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectSpawner : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject[] m_ObjectsToSpawn;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Awake			 (Constructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		CreateObjectsSpawner();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDestroy	   (Deconstructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDestroy()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Objects Spawner
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateObjectsSpawner()
    {
		if( m_ObjectsToSpawn != null )
		{
			for( int i = 0; i < m_ObjectsToSpawn.Length; ++i )
			{
                if( m_ObjectsToSpawn[i] != null )
                {
				    m_ObjectsToSpawn[i].SetActive( false );
                }
			}
		}

		foreach( Transform Child in transform )
		{
			Child.gameObject.SetActive( false );
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Spawn Game Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SpawnGameObjects()
	{
		List<GameObject> lObjs = GetObjects();
		
		foreach( GameObject Obj in lObjs )
		{
        	Obj.gameObject.SetActive(true);
			Obj.transform.parent = transform.parent;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Spawn Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<GameObject> GetObjects()
	{
		List<GameObject> lObjs = new List<GameObject>();
		
		if( m_ObjectsToSpawn != null )
        {
		    for( int i = 0; i < m_ObjectsToSpawn.Length; ++i )
            {
                if( m_ObjectsToSpawn[i] != null )
                {
			        lObjs.Add(m_ObjectsToSpawn[i]);
                }
            }
        }

		foreach (Transform Child in transform)
		{
			lObjs.Add(Child.gameObject);
		}
		
		return lObjs;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Destroy Self
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DestroySelf()
	{
		DestroyObject(gameObject);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: On Collision
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnCollisionEnter(Collision collision)
	{
		if (CollidedWithPlayer(collision.transform.tag))
		{
			SpawnGameObjects();
			DestroySelf();
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
		SphereCollider SCollider    = GetComponent< SphereCollider >();
		Gizmos.color                = GetCubeColour();
        Gizmos.matrix               = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.zero, new Vector3(SCollider.radius * 2, SCollider.radius * 2, 1));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDrawGizmosSelected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDrawGizmosSelected()
	{
		if (GameHandler.UnitySceneOnly())
		{
            Gizmos.color = GetSpawnLineColour();

            if( m_ObjectsToSpawn != null )
            {
			    for (int i = 0; i < m_ObjectsToSpawn.Length; ++i)
			    {
                    if( m_ObjectsToSpawn[i] != null )
                    {
				        Gizmos.DrawLine(transform.position, m_ObjectsToSpawn[i].transform.position);
                    }
			    }
            }

			foreach (Transform Child in transform)
			{
				Gizmos.DrawLine(transform.position, Child.position);
			}
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Methods: Get Gizmo Colours
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static Color GetCubeColour()
    {
        return new Color(0.180f, 0.6117f, 0.4117f, 0.5f);			// Greenish Colour
    }

    private static Color GetSpawnLineColour()
    {
        return new Color(0.235f, 0.235f, 0.882f);				    // Blue Colour
    }
}
