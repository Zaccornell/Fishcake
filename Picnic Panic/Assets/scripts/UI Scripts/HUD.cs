using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image[] m_lives;
    public Text[] m_playerDisplays;
    public Image[] m_playerColor;
    public Player[] m_players;
    public Image[] m_dashReady;
    public Actor m_king;
    public Text m_gameTimer;
    public Text m_roundCountDown;
    public Image m_roundCountImage;
    public Spawner m_spanwer;
    public GameObject m_endGame;
    public Button m_restartButton;
    public AudioSource m_audioSource;
    public AudioClip m_gameMusic;
    public float m_timer;



    private int m_playerLives = 5;
    private int m_minutes;
    private float m_fadeOutTimer;
   

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
        foreach (Image child in m_playerColor)
        {
            child.enabled = false;
        }
        foreach (Image item in m_dashReady)
        {
            item.enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_fadeOutTimer -= Time.deltaTime;
        m_timer += Time.deltaTime;
        m_minutes = Mathf.RoundToInt(m_timer) / 60; // dividing it from the time 
        m_gameTimer.text = m_minutes.ToString("00") + ":" + (m_timer - m_minutes * 60).ToString("00"); // checking to see if its more than a minute

        if (m_fadeOutTimer <= 0)
        {
            m_roundCountImage.rectTransform.sizeDelta = new Vector2(110, 110);
            m_roundCountDown.enabled = false;
            m_roundCountImage.enabled = false;
        }
        else
        {
            Color fadeIn = m_roundCountDown.color;
            fadeIn.a = m_fadeOutTimer;
            m_roundCountDown.color = fadeIn;
            Color imageFade = m_roundCountImage.color;
            imageFade.a = m_fadeOutTimer;
            m_roundCountImage.color = imageFade;
        }
        if (Mathf.RoundToInt(m_spanwer.RoundTimer) <= 6 && Mathf.RoundToInt(m_spanwer.RoundTimer) > 0)
        {
            m_roundCountImage.enabled = true;
            m_roundCountDown.enabled = true;
           
           Color fadeIn = m_roundCountDown.color;
           fadeIn.a = 6 - m_spanwer.RoundTimer;
           m_roundCountDown.color = fadeIn;
            Color imageFade = m_roundCountImage.color;
            imageFade.a = 6 - m_spanwer.RoundTimer;
            m_roundCountImage.color = imageFade;
            if (Mathf.RoundToInt(m_spanwer.RoundTimer) > 5)
            {
                m_roundCountDown.text = "5";
            }
            else
            {
                m_roundCountDown.text = m_spanwer.RoundTimer.ToString("0");
            }

        }
        else if(Mathf.RoundToInt(m_spanwer.RoundTimer) <= 0)
        {
            m_roundCountImage.rectTransform.sizeDelta = new Vector2(500, 100);
            m_fadeOutTimer = 1;
            m_roundCountImage.enabled = true;
            m_roundCountDown.enabled = true;
            m_roundCountDown.text = "Round "+ (m_spanwer.CurrentRound + 2);
        }



        foreach (Player current in m_players)
        {
            m_playerDisplays[current.m_playerNumber - 1].text = current.Health.ToString();
            m_playerDisplays[current.m_playerNumber - 1].enabled = true;
            m_playerColor[current.m_playerNumber - 1].enabled = true;

        }

        for (int i = 0; i < m_players.Length; i++)
        {
            if (m_players[i].DashReady)
            {
                m_dashReady[i].enabled = true;
            }
            else
            {
                m_dashReady[i].enabled = false;
            }

        }

      
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.loop = true;
            m_audioSource.clip = m_gameMusic;
            m_audioSource.Play();
        }
        
      
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
            m_lives[m_playerLives - 1].gameObject.SetActive(false);            

            m_playerLives--;
            return true;
        }
        else
        {
            return false;
        }
    }

}
