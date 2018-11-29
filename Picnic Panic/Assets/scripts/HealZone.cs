using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class HealZone : MonoBehaviour
{
    public int m_healAmount;
    public float m_duration;

    private List<GameObject> m_players;
    private float m_timer;
	// Use this for initialization
	void Start ()
    {
        m_players = new List<GameObject>();
        m_timer = m_duration;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;
        
        // Destory the gameobject after the timer runs out
        if (m_timer <= 0)
        {
            Destroy(gameObject);
        }
	}

    /*
     * Handles healing players that enter the heal zone's trigger
     */
    private void OnTriggerEnter(Collider other)
    { 
        // if the object that entered the collider is a player
        if (other.gameObject.tag == "Player")
        {
            // if the player hasn't already been healed
            if (!m_players.Contains(other.gameObject))
            {                
                m_players.Add(other.gameObject); // store the player
                other.gameObject.GetComponent<Player>().RestoreHealth(m_healAmount); // heal the player
            }
        }
    

    }
}
