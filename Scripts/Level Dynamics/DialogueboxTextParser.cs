using UnityEngine;
using System.Collections;

public class DialogueboxTextParser : MonoBehaviour 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public DialogueBox                      m_DialogueBoxScript;
	public float							m_DialogueBoxScrollSpeed	= 20.0f;
    public DialogueBox.SpokesPersonState    m_Speaker					= DialogueBox.SpokesPersonState.GENERAL;
    public string                           m_sNewText;
	public KeyCode[]						m_PauseKeys;
    public XboxInputHandler.Controls[]      m_PauseAxis;
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
			m_DialogueBoxScript.SetText(m_sNewText, m_Speaker, m_DialogueBoxScrollSpeed);

			if (m_PauseKeys != null && m_PauseKeys.Length > 0 || m_PauseAxis != null && m_PauseKeys.Length > 0)
			{
				m_DialogueBoxScript.SetPauseConditions( m_PauseKeys, m_PauseAxis );
			}

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
        SphereCollider SCollider = GetComponent<SphereCollider>();
        Gizmos.color = GetCubeColour();
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, new Vector3(SCollider.radius * 2, SCollider.radius * 2, 1));
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Get Gizmo Colour
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static Color GetCubeColour()
    {
        return new Color(0.1215f, 0.2313f, 0.5254f, 0.5f);			// Dark Blue Colour
    }
}
