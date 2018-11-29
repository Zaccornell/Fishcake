using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Author: John Plant / Bradyn Corkill
 * Date: 2018/11/29
 */
public class HUD : MonoBehaviour
{
    public Image[] m_lives;
    public Text[] m_playerDisplays;
    public Image[] m_playerColor;
    public Sprite[] m_weaponSprites;
    public Player[] m_players;
    public Image[] m_dashReady;
    public Actor m_king;
    public Text m_gameTimer;
    public Text m_roundCountDown;
    public Image m_roundCountImage;
    public Spawner m_spanwer;
    public PlayerSelect m_playerSelect;
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
		
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Timers
        m_fadeOutTimer -= Time.deltaTime;
        m_timer += Time.deltaTime;
        //

        m_minutes = Mathf.RoundToInt(m_timer) / 60; // dividing it from the time 
        m_gameTimer.text = m_minutes.ToString("00") + ":" + (m_timer - m_minutes * 60).ToString("00"); // checking to see if its more than a minute

        // When the fade out has ended
        if (m_fadeOutTimer <= 0)
        {
            m_roundCountImage.rectTransform.sizeDelta = new Vector2(110, 110); // Revert the size of the image back to the original 
            // disable the countdown ui components
            m_roundCountDown.enabled = false;
            m_roundCountImage.enabled = false;
        }
        else
        {
            // replace the current alpha with the fade out timer as the new alpha
            Color fadeOut = m_roundCountDown.color;
            fadeOut.a = m_fadeOutTimer;
            m_roundCountDown.color = fadeOut;

            // replace the current alpha with the fade out timer as the new alpha
            Color imageFade = m_roundCountImage.color;
            imageFade.a = m_fadeOutTimer;
            m_roundCountImage.color = imageFade;
        }

        // when the timer is between 0-6
        if (Mathf.RoundToInt(m_spanwer.RoundTimer) <= 6 && Mathf.RoundToInt(m_spanwer.RoundTimer) > 0)
        {
            // enable the countdown ui components
            m_roundCountImage.enabled = true;
            m_roundCountDown.enabled = true;

            // fade in between 6-5
            // replace the current alpha with the fade in timer as the new alpha
            Color fadeIn = m_roundCountDown.color;
            fadeIn.a = 6 - m_spanwer.RoundTimer;
            m_roundCountDown.color = fadeIn;

            // replace the current alpha with the fade in timer as the new alpha
            Color imageFade = m_roundCountImage.color;
            imageFade.a = 6 - m_spanwer.RoundTimer;
            m_roundCountImage.color = imageFade;

            // between 6-5 seconds the timer still displays 5
            if (Mathf.RoundToInt(m_spanwer.RoundTimer) > 5)
            {
                m_roundCountDown.text = "5";
            }
            else
            {
                m_roundCountDown.text = m_spanwer.RoundTimer.ToString("0"); // display the timer
            }

        }
        // when the timer is reaches 0
        else if(Mathf.RoundToInt(m_spanwer.RoundTimer) <= 0)
        {
            m_roundCountImage.rectTransform.sizeDelta = new Vector2(500, 100); // expand the background image to fit the new text
            m_fadeOutTimer = 1; // start the fade out timer
            m_roundCountImage.enabled = true;
            m_roundCountDown.enabled = true;
            m_roundCountDown.text = "Round "+ (m_spanwer.CurrentRound + 2); // display the next round
        }

        foreach (Player current in m_players)
        {
            m_playerDisplays[current.m_playerNumber - 1].text = current.Health.ToString(); // display the health for each player
        }

       
        for (int i = 0; i < m_players.Length; i++)
        {
            // if the player's dash is ready 
            if (m_players[i].DashReady)
            {
                m_dashReady[i].enabled = true; // enable the dash ready image 
            }
            else
            {
                m_dashReady[i].enabled = false; // disable the dash ready image
            }
        }
      
        // if the audio source is not playing anything
        if (!m_audioSource.isPlaying)
        {
            // give the audio source the game music and play it
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
     * Use one of the players' shared lives
     * Returns true if there was a life left
     */
    public bool UseLife()
    {
        // if there are lives left 
        if (m_playerLives > 0)
        {            
            // disable the object
            m_lives[m_playerLives - 1].gameObject.SetActive(false);            

            m_playerLives--;
            return true; // return true if there was lives
        }
        else
        {
            return false; // return false if there were not any lives
        }
    }

    /*
     * Enable the hud elements for each player
     */
    public void AssignPlayers()
    {
        // disable all of the player dependant hud elements
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
        //

        // For each player that exists
        for (int i = 0; i < m_players.Length; i++)
        {
            // enable their hud elements
            m_playerDisplays[m_players[i].m_playerNumber - 1].enabled = true;
            m_playerColor[m_players[i].m_playerNumber - 1].enabled = true;
            m_dashReady[m_players[i].m_playerNumber - 1].enabled = true;
        }
        // for health backgrounds
        for (int i = 0; i < 4; i++)
        {
            // if the image is enabled
            if (m_playerColor[i].enabled)
            {
                m_playerColor[i].sprite = m_weaponSprites[m_playerSelect.SelectedWeapons[i]]; // change the sprite to the specific weapon the player selected
            }
        }
    }

}
