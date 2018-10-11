using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject[] m_lives;
    public Text[] m_displayHealth;
    public Actor[] m_actorHealth;
    public Text m_displayTimer;
    public Spawner m_spanwer;

    public Text m_enemyCounter;

    private int m_playerLives = 5;

    public int PlayerLives
    {
        get { return m_playerLives; }
        set { m_playerLives = value; }
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < m_actorHealth.Length; i++)
        {
            m_displayHealth[i].text = m_actorHealth[i].Health.ToString();
        }
        m_displayTimer.text = (Mathf.Ceil(m_spanwer.RoundTimer)).ToString(); // displays timer onto the HUD
        m_enemyCounter.text = (m_spanwer.EnemyTotal).ToString();// displays the amount of enemies left to spawn and spawned

    }

    public bool UseLife()
    {
        if (m_playerLives > 0)
        {
            m_lives[m_playerLives - 1].SetActive(false);
            m_playerLives--;
            return true;
        }
        else
        {
            return false;
        }
    }

}
