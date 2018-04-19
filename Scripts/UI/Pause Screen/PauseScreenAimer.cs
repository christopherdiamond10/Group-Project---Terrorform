using UnityEngine;
using System.Collections;

public class PauseScreenAimer : MonoBehaviour
{
	private bool m_bBounce;

	public float m_fAimRate;

	private float m_fReAim;

	public float m_fTrackStray;

	private float fHorizontal;
	private float fVertical;

	public float m_PlayerOffset = 300;

	public float StrafeSpeed { get; set; }
	public float AltSpeed { get; set; }

	private Vector3 v3Velocity;

	private Transform StartPos;

	public float MoveAdjust { get; set; }

	private bool m_bBrainBugged;

	public Vector2 m_v3MinBounds;
	public Vector2 m_v3MaxBounds;

	public bool m_bRollLeft = false;
	public bool m_bRollRight = false;


	//visual Second Aimer - Bernard
	public GameObject VisAimer;
	public GameObject Player;


	float m_fTiltMod;
	float m_fMoveValue;
	// Use this for initialization
	void Start()
	{
		m_fAimRate = (m_fAimRate != 0) ? m_fAimRate : 80.0f;
		m_bBounce = false;
		transform.localPosition = new Vector3(0, 20, m_PlayerOffset);

		StartPos = transform;

		m_fTrackStray = (m_fTrackStray != 0) ? m_fTrackStray : 100f;

		m_v3MinBounds = new Vector2((-Screen.width * 0.45f), -Screen.height * 0.44f);
		m_v3MaxBounds = new Vector2((Screen.width * 0.45f), Screen.height * 0.44f);
	}

	// Update is called once per frame
	void Update()
	{
		GetInput();

		MoveAimer();

		UpdateVisualAimer();
	}

	void UpdateVisualAimer()
	{					//AIMER					//Player
		Vector3 line = transform.position - Player.transform.position;
		line.Normalize();
		Vector3 VisAimerPoint = line * -100;

		VisAimerPoint = VisAimerPoint + transform.position;

		VisAimer.transform.position = Vector3.Lerp(VisAimer.transform.position, VisAimerPoint, DynamicUpdateManager.GetDeltaTime() * 100);
	}

	public void SetBounce(bool bounce, Vector3 bounceDir)
	{
		m_bBounce = bounce;
		m_fReAim = 0;
	}

	void MoveAimer()
	{
		if (m_bBounce)
		{
			m_fReAim += DynamicUpdateManager.GetDeltaTime();
		}
		if (m_bBounce && m_fReAim < 1f)
		{
			//transform.position += (v3BounceDir * ((m_fAimRate) * DynamicUpdateManager.GetDeltaTime()));
		}
		else if (m_bRollLeft || m_bRollRight)
		{
			if (m_bRollLeft)
			{
				v3Velocity.x += -3;
			}
			if (m_bRollRight)
			{
				v3Velocity.x += 3;
			}
			CheckBounds();
			transform.localPosition += v3Velocity * (m_fAimRate * DynamicUpdateManager.GetDeltaTime());
		}
		else
		{
			CheckBounds();
			transform.localPosition += v3Velocity * (m_fAimRate * DynamicUpdateManager.GetDeltaTime());
		}
		if (m_fReAim > 0.2f)
		{
			m_bBounce = false;
		}
	}

	public Vector3 GetVelocity()
	{
		return v3Velocity;
	}

	public float GetTiltMod()
	{
		return m_fTiltMod;
	}

	void GetInput()
	{
		fVertical = Input.GetAxis("LeftAnalogVertical") + (AltSpeed * Input.GetAxis("LeftAnalogVertical"));
		fHorizontal = Input.GetAxis("LeftAnalogHorizontal") + (StrafeSpeed * Input.GetAxis("LeftAnalogHorizontal"));

		m_fTiltMod = 0;

		/////////////
		if (GameHandler.IsInvertedYAxis())
		{
			if (m_bBrainBugged)
			{
				CheckInvertedXAxisInput();
				CheckYAxisInput();
			}
			else
			{
				CheckXAxisInput();
				CheckInvertedYAxisInput();
			}
		}

		else
		{
			if (m_bBrainBugged)
			{
				CheckInvertedXAxisInput();
				CheckInvertedYAxisInput();
			}
			else
			{
				CheckXAxisInput();
				CheckYAxisInput();
			}
		}
		/////////////

		if (fHorizontal < 0)
		{
			m_fTiltMod = 1;
		}
		if (fHorizontal > 0)
		{
			m_fTiltMod = -1;
		}

		m_fMoveValue = Mathf.Abs((fHorizontal + fVertical) * 0.5f);

		v3Velocity = ((transform.localRotation * Vector3.right) * fHorizontal) + ((transform.localRotation * Vector3.forward) * -fVertical); //new Vector3(transform.righy-fHorizontal + MoveAdjust, -fVertical,0);
	}

	private void CheckXAxisInput()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			fHorizontal = -1f - StrafeSpeed;
		}

		else if (Input.GetKey(KeyCode.RightArrow))
		{
			fHorizontal = 1f + StrafeSpeed;
		}
	}

	private void CheckYAxisInput()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			fVertical = -1f - AltSpeed;
		}

		else if (Input.GetKey(KeyCode.DownArrow))
		{
			fVertical = 1f + AltSpeed;
		}
	}

	private void CheckInvertedXAxisInput()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			fHorizontal = 1f + StrafeSpeed;
		}

		else if (Input.GetKey(KeyCode.RightArrow))
		{

			fHorizontal = -1f - StrafeSpeed;
		}
	}

	private void CheckInvertedYAxisInput()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			fVertical = 1f + AltSpeed;
		}

		else if (Input.GetKey(KeyCode.DownArrow))
		{
			fVertical = -1f - AltSpeed;
		}
	}

	public Transform GetStart()
	{
		return StartPos;
	}

	public float GetMoveValue()
	{
		return m_fMoveValue;
	}

	private void CheckBounds()
	{
		Vector3 newPos = transform.localPosition + (v3Velocity * (m_fAimRate * DynamicUpdateManager.GetDeltaTime()));

		if (newPos.x < m_v3MinBounds.x || newPos.x > m_v3MaxBounds.x)
		{
			v3Velocity.x = 0;
		}

		if (newPos.y < m_v3MinBounds.y || newPos.y > m_v3MaxBounds.y)
		{
			v3Velocity.y = 0;
		}

		v3Velocity.z = 0;
	}

	public bool WithinBounds()
	{
		return true;
	}


	public void SetBugged(bool a_bBugged)
	{
		m_bBrainBugged = a_bBugged;
	}
}
