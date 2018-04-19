using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectDestroyer : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public bool         m_DestroyAllActiveExplosiveUnits    = true;
    public bool         m_DestroyAllActiveTurretUnits       = true;
    public bool         m_DestroyAllActiveBrainBugUnits     = true;
    public bool         m_DestroyAllActiveLightUnits        = false;
    public bool         m_DestroyAllActiveHeavyUnits        = true;
	public GameObject[] m_GameObjectsToDespawn;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Awake			 (Constructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: OnDestroy	   (Deconstructor)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDestroy()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Destroy Game Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DestroyGameObjects()
	{
		List<GameObject> lObjs = GetObjects();
		foreach( GameObject Obj in lObjs )
		{
			Destroy(Obj);
		}

        // Destroy Active AI's
        AI_Manager.DestroyAllActiveAIs(m_DestroyAllActiveExplosiveUnits, m_DestroyAllActiveTurretUnits, m_DestroyAllActiveBrainBugUnits, m_DestroyAllActiveLightUnits, m_DestroyAllActiveHeavyUnits);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Spawn Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private List<GameObject> GetObjects()
	{
		List<GameObject> lObjs = new List<GameObject>();

		if (m_GameObjectsToDespawn != null)
		{
			for (int i = 0; i < m_GameObjectsToDespawn.Length; ++i)
			{
				if (m_GameObjectsToDespawn[i] != null)
				{
					lObjs.Add(m_GameObjectsToDespawn[i]);
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
			DestroyGameObjects();
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
            Gizmos.color = GetDespawnLineColour();

            if( m_GameObjectsToDespawn != null )
            {
                for (int i = 0; i < m_GameObjectsToDespawn.Length; ++i)
                {
                    if( m_GameObjectsToDespawn[i] != null )
                    {
                        Gizmos.DrawLine(transform.position, m_GameObjectsToDespawn[i].transform.position);
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
        return new Color(0.6196f, 0.098f, 0.1882f, 0.5f);			// Dark Red Colour
    }

    private static Color GetDespawnLineColour()
    {
        return new Color(0.698f, 0.4313f, 0.1647f);				    // Dark Orange Colour
    }
}
