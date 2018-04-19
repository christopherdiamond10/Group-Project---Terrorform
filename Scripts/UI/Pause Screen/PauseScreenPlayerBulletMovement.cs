using UnityEngine;
using System.Collections;

public class PauseScreenPlayerBulletMovement : MonoBehaviour
{

	public float m_fSpeed;
	public float m_fDamage;

	public ParticleSystem m_Sparks;

	public PauseScreenPlayerControls owningPlayer;

	public PauseScreenAimer targetAim;
	public LayerMask rayCastDetectionLayers;
	private Vector3 AimDir;
	private Vector3 ForwardDir;

	private bool RayTarget;

	private float deathTimer;

	private TimeTracker m_TTSelfDestruct;

	// Use this for initialization
	void Start()
	{
		m_TTSelfDestruct = new TimeTracker(1.0f, false, true);
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = (transform.position + (transform.forward * m_fSpeed * DynamicUpdateManager.GetDeltaTime()));

		m_TTSelfDestruct.Update();
		if( m_TTSelfDestruct.TimeUp() )
		{
			DestroyImmediate(gameObject);
		}
	}

	public float GetDamage()
	{
		return m_fDamage;
	}

	public void DestroyBullet()
	{

		this.GetComponent<Renderer>().enabled = false;
		m_Sparks.Play();
		Destroy(gameObject, 0.15f);
	}

	public void DestroyFromTime()
	{
		Destroy(gameObject);
	}
}

