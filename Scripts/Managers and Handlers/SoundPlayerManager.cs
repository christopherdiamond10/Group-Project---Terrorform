using UnityEngine;
using System.Collections;

public class FadeEffect
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AudioSource	m_ASource;
	private TimeTracker	m_TTTimeValue;
	private float		m_fStartVolume;
	private float		m_fEndVolume;
	private bool		m_bFadeIn;
	private int			m_ID = -1;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Constructor
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public FadeEffect(AudioSource ASource, float FadeTime, float StartVolume, float FinishVolume)
	{
		if( ASource == null )
		{
			return;
		}

		m_ASource		= ASource;
		m_fStartVolume	= StartVolume;
		m_fEndVolume	= FinishVolume;
		m_bFadeIn		= (StartVolume < FinishVolume);
		m_TTTimeValue	= new TimeTracker(FadeTime, false, true);
		m_ID			= DynamicUpdateManager.AddFadeEffect(this);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Update()
	{
		m_TTTimeValue.Update();
		if( m_TTTimeValue.TimeUp() )
		{
			m_ASource.volume = m_fEndVolume;
			DynamicUpdateManager.RemoveFadeEffect(m_ID);
		}


		
		if( m_bFadeIn )
		{
			m_ASource.volume = ((m_fEndVolume - m_fStartVolume) * m_TTTimeValue.GetCompletionPercentage());
		}
		else
		{
			m_ASource.volume = (m_fStartVolume - (m_fStartVolume * m_TTTimeValue.GetCompletionPercentage()));
		}
	}
};

public class SoundPlayerManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public			AudioClip		m_AudioClip;
    public          bool            m_bPlayOnAwake = true;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static	AudioSource		m_AudioSource;
	private static	bool			m_bIsMuted = false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		m_AudioSource = AddAudio(gameObject, m_AudioClip, true, !m_bIsMuted);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Redefined Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Audio
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void PlayAudio(AudioClip clip)
	{
		m_AudioSource.clip = clip;
		m_AudioSource.Play();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Mute Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void SetMuted( bool b )
	{
		m_bIsMuted = b;
		if( m_bIsMuted )
		{
			m_AudioSource.Stop();
		}
		else
		{
			m_AudioSource.Play();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Muted?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool IsMuted()
	{
		return m_bIsMuted;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Audio
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AudioSource AddAudio(GameObject Obj, AudioClip Clip, bool Loop = false, bool PlayOnAwake = false,  float Volume = 0.8f)
	{
		AudioSource Source = null;
		if (Clip != null)
		{
			Source = Obj.AddComponent<AudioSource>();
			Source.clip = Clip;

			if (PlayOnAwake)
			{
				Source.Play();
			}
			Source.loop = Loop;
			Source.volume = Volume;
		}

		return Source;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Audio Source
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AudioSource AddAudioSource(GameObject Obj)
	{
		return Obj.AddComponent<AudioSource>();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get BGM Audio Source
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static AudioSource GetBGMAudioSource()
	{
		return m_AudioSource;
	}
}
