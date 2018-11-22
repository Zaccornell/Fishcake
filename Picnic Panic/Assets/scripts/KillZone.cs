using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    /*
     * Handles killing actors that enter the trigger
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().FallDamage(999);
        }
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<MovingActor>().TakeDamage(999, null);
        }
        
    }
}
