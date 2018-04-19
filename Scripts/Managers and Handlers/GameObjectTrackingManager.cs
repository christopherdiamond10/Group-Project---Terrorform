using UnityEngine;
using System.Collections;

public class GameObjectTrackingManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	// Static GameObjects for other scripts to access
	public static GameObject m_goPlayer;


	// Public GameObjects to have values inputted via the Unity Inspector
	public GameObject m_Player;

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		m_goPlayer = m_Player;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Object World Transform
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static Transform GetObjectWorldTransform( GameObject obj )
	{
		Transform Root = obj.transform;
        while (Root.parent != null)
		{
            Root = Root.parent;
		}
        return Root;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Object World Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static Vector3 GetObjectWorldPosition(GameObject obj)
	{
		return GetObjectWorldTransform(obj).position;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Object World Rotation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static Quaternion GetObjectWorldRotation(GameObject obj)
	{
		return GetObjectWorldTransform(obj).rotation;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Object Future Position (Frames)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static Vector3 GetObjectFuturePosition(GameObject obj, float objSpeed, uint Frames)
	{
        return (obj.transform.position + (obj.transform.forward * objSpeed * Time.deltaTime * Frames));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Object Future Position (Seconds)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static Vector3 GetObjectFuturePosition(GameObject obj, float objSpeed, float Seconds)
	{
        return (obj.transform.position + (obj.transform.forward * objSpeed * Seconds));
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Calculate Intercept Course
    // ** Code By "Bunny83" :   http://answers.unity3d.com/questions/296949/how-to-calculate-a-position-to-fire-at.html
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;

        float iSpeed2       = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2       = aTargetSpeed.sqrMagnitude;
        float fDot1         = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2   = targetDir.sqrMagnitude;
        float d             = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);

        if (d < 0.1f) // negative == no possible course because the interceptor isn't fast enough
        {
            return Vector3.zero;
        }

        float sqrt  = Mathf.Sqrt(d);
        float S1    = (-fDot1 - sqrt) / targetDist2;
        float S2    = (-fDot1 + sqrt) / targetDist2;

        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
            {
                return Vector3.zero;
            }
            else
            {
                return (S2) * targetDir + aTargetSpeed;
            }
        }
        else if (S2 < 0.0001f)
        {
            return (S1) * targetDir + aTargetSpeed;
        }
        else if (S1 < S2)
        {
            return (S2) * targetDir + aTargetSpeed;
        }
        else
        {
            return (S1) * targetDir + aTargetSpeed;
        }
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Find Closest Point of Approach
    // ** Code By "Bunny83" :   http://answers.unity3d.com/questions/296949/how-to-calculate-a-position-to-fire-at.html
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
    {
        Vector3 PVec    = aPos1 - aPos2;
        Vector3 SVec    = aSpeed1 - aSpeed2;
        float d         = SVec.sqrMagnitude;

        // if d is 0 then the distance between Pos1 and Pos2 is never changing
        // so there is no point of closest approach... return 0
        // 0 means the closest approach is now!
        if (d >= -0.0001f && d <= 0.0002f)
        {
            return 0.0f;
        }

        return (-Vector3.Dot(PVec, SVec) / d);
    }
}
