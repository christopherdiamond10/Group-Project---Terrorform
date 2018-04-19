using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthBarScript: MonoBehaviour 
{

	public GameObject[] HealthObjects = new GameObject[10];
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float health = GetComponent<PlayerControls>().m_fHealth;
		
		
		if( health < 10)
		{
			//HealthObjects[1].GetComponent<Visuals>()
		}
		else if( health < 20)
		{
			
		}
		else if( health < 30)
		{
			
		}
		else if( health < 40)
		{
			
		}
		else if( health < 50)
		{
			
		}
		else if( health < 60)
		{
			
		}
		else if( health < 70)
		{
			
		}
		else if( health < 80)
		{
			
		}
		else if( health < 90)
		{
			
		}
		else if( health < 100)
		{
			
		}
		
	}
}
