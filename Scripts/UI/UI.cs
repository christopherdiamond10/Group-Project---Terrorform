using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	PlayerControls PlayerControlsInstance { get { return GameObjectTrackingManager.m_goPlayer.GetComponent<PlayerControls>(); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AudioClip	m_HealthAlarmSound;
    public GameObject[] m_HealthObjects = new GameObject[10];
	public GameObject	m_goScorePopupPrefab;

	public UILabel		m_uiScoreCount;
	public UILabel		m_uiHealthPercenatge;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AudioSource m_HealthAlarmSource;
	private uint		m_uiScore = 0;
	private float 		m_fHealthPercentage;
	private string		m_sColour = "";
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Start () 
    {
		m_HealthAlarmSource = SoundPlayerManager.AddAudio(gameObject, m_HealthAlarmSound, false, false, 1.0f);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Update () 
    {
		UpdateLabels();
        UpdateHealthBar();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Health Bar
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void UpdateHealthBar()
	{
		for (int i = 0; i < m_HealthObjects.Length; ++i) //i < the count of HealtObjects array size
		{
			if (!m_HealthObjects[i].GetComponent<Visuals>().m_bPlayed && m_fHealthPercentage <= (m_HealthObjects.Length * i))
			{
				if( m_HealthObjects[i].GetComponent<Visuals>().m_eHealthVisState != Visuals.eHealthVis.GONE )
					m_HealthObjects[i].GetComponent<Visuals>().m_eHealthVisState = Visuals.eHealthVis.PULSE;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Labels
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void UpdateLabels()
	{
        if (PlayerControlsInstance.m_uiPlayerScore != m_uiScore)
		{
            RunScorePopup();
            m_uiScore = PlayerControlsInstance.m_uiPlayerScore;
		    m_uiScoreCount.text = "Score:  " + m_uiScore.ToString();
		}

        if (m_fHealthPercentage != PlayerControlsInstance.m_fHealth && PlayerControlsInstance.m_fHealth > 0)
        {
			// Play Alarm if Low Health. Only Play Once
			if( PlayerControlsInstance.m_fHealth < 20f && m_fHealthPercentage >= 20f )
			{
				m_HealthAlarmSource.Play();
			}

			// Back to Full Health after Dying?
			if (PlayerControlsInstance.m_fHealth > 99.9f)
			{
				foreach (GameObject GO in m_HealthObjects)
				{
					GO.GetComponent<Visuals>().Reset();
					GO.GetComponent<Visuals>().m_eHealthVisState = Visuals.eHealthVis.FINE;
				}
			}

            m_uiHealthPercenatge.text = "Health: " + GetHealthColour() + ((int)m_fHealthPercentage).ToString() + "%";
        }
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Run "Score Popup"
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void RunScorePopup()
    {
        if( m_goScorePopupPrefab != null )
		{
            uint uiCurrentScore = PlayerControlsInstance.m_uiPlayerScore;

			if( uiCurrentScore < m_uiScore )
			{
				uint uiDifference = (m_uiScore - uiCurrentScore);
				GameObject Popup = Instantiate(m_goScorePopupPrefab) as GameObject;
				Popup.transform.parent = GetComponent<UIObjectsHolder>().PlayerInfoPanel.transform;
                Popup.transform.localScale = ScorePopupScript.sm_vScale;
				Popup.GetComponent< UILabel >().color = new Color(0.4274f, 0.0549f, 0.07540f);
				Popup.GetComponent< UILabel >().text = "-" + (uiDifference.ToString());
			}
			else
			{
				uint uiDifference = (uiCurrentScore - m_uiScore);
				GameObject Popup = Instantiate(m_goScorePopupPrefab) as GameObject;
				Popup.transform.parent = GetComponent<UIObjectsHolder>().PlayerInfoPanel.transform;
                Popup.transform.localScale = ScorePopupScript.sm_vScale;
				Popup.GetComponent< UILabel >().color = new Color(0.9843f, 1.0f, 0.0f);
				Popup.GetComponent< UILabel >().text = "+" + (uiDifference.ToString());
			}
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Health Colour
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetHealthColour()
	{
        m_fHealthPercentage = PlayerControlsInstance.m_fHealth;
		
		
		int iMaxHealth			= 100;
		bool bHalfHealth		= (m_fHealthPercentage < (iMaxHealth * 0.5f));
		iMaxHealth				= (int)(iMaxHealth * 0.5f);
		float fHealthPercentage = bHalfHealth ? m_fHealthPercentage : (m_fHealthPercentage * 0.5f);

		int[] AliveColour	= bHalfHealth ? new int[3] { 246, 255, 0  }  : new int[3] { 255, 255, 255 };
		int[] DeadColour	= bHalfHealth ? new int[3] { 255, 0,   0  }	 : new int[3] { 246, 255, 0   };

		float fAliveHealthRange = ((float)fHealthPercentage / iMaxHealth);
		float fDeadHealthRange  = ((float)(iMaxHealth - fHealthPercentage) / iMaxHealth);

		AliveColour[0] = (int)(AliveColour[0] * fAliveHealthRange);
		AliveColour[1] = (int)(AliveColour[1] * fAliveHealthRange);
		AliveColour[2] = (int)(AliveColour[2] * fAliveHealthRange);

		DeadColour[0] = (int)(DeadColour[0] * fDeadHealthRange);
		DeadColour[1] = (int)(DeadColour[1] * fDeadHealthRange);
		DeadColour[2] = (int)(DeadColour[2] * fDeadHealthRange);

		m_sColour = "[" + System.Convert.ToByte(AliveColour[0] + DeadColour[0]).ToString("x2") +		// Red
						  System.Convert.ToByte(AliveColour[1] + DeadColour[1]).ToString("x2") +		// Green
						  System.Convert.ToByte(AliveColour[2] + DeadColour[2]).ToString("x2") + "]";	// Blue
		return m_sColour;
	}
}
