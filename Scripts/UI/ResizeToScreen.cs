using UnityEngine;
using System.Collections;

public class ResizeToScreen : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+	Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject[]      m_ObjectsToResize;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-	Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static float m_fScreenTestWidth  = 1906.0f;
	private static float m_fScreenTestHeight = 1027.0f;	// Screen Dimensions I was using when I was making the UI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		// Scale Objects
		for( int i = 0; i < m_ObjectsToResize.Length; ++i )
		{
			ResizeObject( m_ObjectsToResize[i] );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Resize Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void ResizeObject(GameObject Obj)
	{
        Obj.transform.localPosition = ResizeVector3(Obj.transform.localPosition);
        Obj.transform.localScale    = ResizeVector3(Obj.transform.localScale);
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Scale Vector3
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public static Vector3 ResizeVector3(Vector3 V)
    {
        return new Vector3( V.x * WidthScaleFactor(), V.y * HeightScaleFactor(), V.z );
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Width Scale Factor
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static float WidthScaleFactor()
    {
        return ((float)Screen.width / m_fScreenTestWidth);
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Height Scale Factor
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static float HeightScaleFactor()
    {
        return ((float)Screen.height / m_fScreenTestHeight);
    }
}
