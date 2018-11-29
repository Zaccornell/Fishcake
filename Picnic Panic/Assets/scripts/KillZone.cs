using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Bradyn Corkill 
 * Date: 2018/11/29
 */
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
        // if the object is a player
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().FallDamage(999); // call the falldamage function on the player to kill them even in god mode
        }
        // if the object is an enemy
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<MovingActor>().TakeDamage(999, null); // call the take damage function of the enemy
        }
        
    }
}
