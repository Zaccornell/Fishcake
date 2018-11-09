using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject[] m_lives;
    public Text[] m_playerDisplays;
    public Player[] m_players;
    public Actor m_king;
    public Text m_kingDisplay;
    public Text m_displayTimer;
    public Spawner m_spanwer;
    public GameObject m_endGame;
    public Button m_restartButton;
    public AudioSource m_audioSource;
    public AudioClip m_gameMusic;
    public float m_timer;


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
		foreach (Text current in m_playerDisplays)
        {
            current.enabled = false;
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer += Time.deltaTime;
      
        foreach (Player current in m_players)
        {
            m_playerDisplays[current.m_playerNumber - 1].text = current.Health.ToString();
            m_playerDisplays[current.m_playerNumber - 1].enabled = true;
        }

        m_kingDisplay.text = m_king.Health.ToString();
      
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.loop = true;
            m_audioSource.clip = m_gameMusic;
            m_audioSource.Play();
        }
        
        m_displayTimer.text = (Mathf.Ceil(m_spanwer.RoundTimer)).ToString(); // displays timer onto the HUD
        m_enemyCounter.text = (m_spanwer.EnemyTotal).ToString();// displays the amount of enemies left to spawn and spawned
       
        int playersAlive = 0; // setting a verible to keep count of living players
        foreach (Player amount in m_players)
        {
            if (amount.Alive) // seeing if the amount of players is alive or not
            {
                playersAlive++; // plusing 1 to the varible to amount alive 
            }
        }
        if (m_king.Health <= 0 || playersAlive <= 0) // cheeking to see if any players or king is alive 
        {  
            if (m_king.Health <= 0)
            {
                m_endGame.GetComponent<EndGame>().Cause = Cause.PieKing;
            }
            else
            {
                m_endGame.GetComponent<EndGame>().Cause = Cause.Player;
            }

            m_endGame.SetActive(true); // activating endscreen
            m_restartButton.Select(); // setting a button to start event
        }

    }

    /*
     * Use one of the players' shared 
     */
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
