using UnityEngine;
using System.Collections;

public class ScorePopupScript : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public					MovementBasedOnTime		m_MovementClass;
	public		static		Vector3					sm_vStartPosition		= new Vector3(-37.9f, -14.0f, 0.0f);
	public		static		Vector3					sm_vEndPosition			= new Vector3(-37.9f, -8.0f,  0.0f);
    public      static      Vector3                 sm_vScale               = new Vector3(2.0f, 2.0f, 1.0f);
	public		static		float					sm_fScoreMovementTimer	= 3.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Start () 
	{
		m_MovementClass = new MovementBasedOnTime(sm_vStartPosition, sm_vEndPosition, sm_fScoreMovementTimer);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update() 
	{
		m_MovementClass.Update();

		// Destroy Self if Finished
		if( m_MovementClass.HasReachedDestination() )
		{
			DestroyImmediate(gameObject);
		}

		else
		{
			// Set Position
			transform.localPosition = m_MovementClass.GetCurrentPosition();

			// Change Alpha for Score
			Color LabelColour = GetComponent< UILabel >().color;
			LabelColour.a = (1.0f - m_MovementClass.GetTimeInstance().GetCompletionPercentage());
			GetComponent< UILabel >().color = LabelColour;
		}
	}
}
