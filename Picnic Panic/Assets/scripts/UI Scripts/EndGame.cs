using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

/*
 * Author: John Plant / Bradyn Corkill
 * Date: 2018/11/29
 *
 * Enum that specifies what causes the game to end
 */
public enum Cause
{
    PieKing,
    Player
}

public class EndGame : MonoBehaviour
{
    public GameObject[] m_children;
    public GameObject[] m_objects;
    public Spawner m_spawner;
    public HUD m_hud;
    public PlayerSelect m_playerSelect;
    public Player[] m_players;
    public Text m_roundValue;
    public Text m_endCauseValue;
    public Text m_gameTimer;
    public Sprite[] m_weaponSprites;
    public Image[] m_weaponImages;
    public GameObject[] m_playerStats;
    public Text[] m_player1Values;
    public Text[] m_player2Values;
    public Text[] m_player3Values;
    public Text[] m_player4Values;
    public float m_length;
    public Image m_backGround;
    public AudioClip m_gameOver;
    public AudioSource m_audioSource;
    public Button m_mainMenu;

    private float m_timer;
    private int m_minutes;
    private bool m_check;

    private Cause m_cause;

    public Cause Cause
    {
        set { m_cause = value; }
    }

    // Use this for initialization
    void Start ()
    {
        m_timer = m_length;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;

        Color c = m_backGround.color; //creating a veriable for back ground color

        c.a = 1 - m_timer / m_length; // setting the the color alpha to slowly fade in 

        m_backGround.color = c; // seeting the color to slowly fade in

        // if the timer for the fade has ended and the statement has not been checked before
        if (m_timer <= 0 && !m_check)
        {
            m_mainMenu.Select();
            // if the gameover sound exists
            if (m_gameOver != null)
            {
                m_audioSource.loop = true;
                m_audioSource.clip = m_gameOver;
                m_audioSource.Play();
            }
            // set all of the children to active
            foreach (GameObject current in m_children)
            {
                current.SetActive(true);
            }
            m_spawner.enabled = false; // disable the spawner

            // display the the current round text
            if (m_spawner.CurrentRound == 1)
            {
                m_roundValue.text = "You survived for " + m_spawner.CurrentRound.ToString() + " round"; // remove the s when there is only 1 round
            }
            else
            {
                m_roundValue.text = "You survived for " + m_spawner.CurrentRound.ToString() + " rounds";
            }

            // display the end cause text
            if (m_cause == Cause.PieKing)
            {
                m_endCauseValue.text = "Your Pie King has fallen";
            }
            else
            {
                m_endCauseValue.text = "You all died";
            }
            

            // checking to see if the timer is greater than 60 seconds 
            if (m_hud.m_timer >= 60)
            {
                m_minutes =(int)m_hud.m_timer / 60; // dividing it from the time 
                //m_hud.m_timer -= m_minutes * 60; // resets the timer for minutes and secounds 
                if (m_minutes > 1)
                {
                    m_gameTimer.text = "You survived for " + m_minutes.ToString() + " minutes and " + (m_hud.m_timer - m_minutes * 60).ToString("0") + " seconds"; // checking to see if its more than a minute
                }
                else
                {
                    m_gameTimer.text = "You survived for " + m_minutes.ToString() + " minute and " + (m_hud.m_timer - m_minutes * 60).ToString("0") + " seconds";  // only outputing when it is 1 minute 
                }
            }
            else
            {
                m_gameTimer.text = "You survived for " + m_hud.m_timer.ToString("0") + " seconds"; // if it doesnt go over a minute 

            }

            // change the weapon images for each player to the weapon they selected
            for (int i = 0; i < 4; i++)
            {
                m_weaponImages[i].sprite = m_weaponSprites[m_playerSelect.SelectedWeapons[i]];
            }

            // Activate the player stats for each player in the game
            foreach (Player player in m_players)
            {
                // Set the values from each player
                switch (player.m_playerNumber)
                {
                    case 1:
                        m_playerStats[0].SetActive(true);
                        m_player1Values[0].text = player.Kills.ToString();
                        m_player1Values[1].text = player.Deaths.ToString();
                        m_player1Values[2].text = player.AntKills.ToString();
                        m_player1Values[3].text = player.RoachKills.ToString();
                        m_player1Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 2:
                        m_playerStats[1].SetActive(true);
                        m_player2Values[0].text = player.Kills.ToString();
                        m_player2Values[1].text = player.Deaths.ToString();
                        m_player2Values[2].text = player.AntKills.ToString();
                        m_player2Values[3].text = player.RoachKills.ToString();
                        m_player2Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 3:
                        m_playerStats[2].SetActive(true);
                        m_player3Values[0].text = player.Kills.ToString();
                        m_player3Values[1].text = player.Deaths.ToString();
                        m_player3Values[2].text = player.AntKills.ToString();
                        m_player3Values[3].text = player.RoachKills.ToString();
                        m_player3Values[4].text = player.HealsUsed.ToString();
                        break;

                    case 4:
                        m_playerStats[3].SetActive(true);
                        m_player4Values[0].text = player.Kills.ToString();
                        m_player4Values[1].text = player.Deaths.ToString();
                        m_player4Values[2].text = player.AntKills.ToString();
                        m_player4Values[3].text = player.RoachKills.ToString();
                        m_player4Values[4].text = player.HealsUsed.ToString();
                        break;
                }
            }

            // Set all the enemies in the spawner to not alive
            foreach (MovingActor enemy in m_spawner.Enemies)
            {
                enemy.Alive = false;
            }

            m_check = true;
        }
	}

    /*
     * Disables the required objects when the endgame is enabled by the HUD
     */
    private void OnEnable()
    {
        // Disable the players' scripts and animators
        foreach (Player player in m_spawner.m_players)
        {
            player.enabled = false;
            player.Animator.enabled = false;
        }
        // if the cause was not the player
        if (m_cause != Cause.Player)
        {
            // for each of the enemies in spawner
            foreach (MovingActor current in m_spawner.Enemies)
            {
                // disable the enemy's script and animator
                if (current != null)
                {
                    current.enabled = false;
                    if (current.Animator != null)
                        current.Animator.enabled = false;
                }
            }
            m_spawner.enabled = false;
        }
        else
        {
            m_spawner.NoDelay = true; // enable the NoDelay of the spawner
        }
        foreach (GameObject child in m_objects) // going though and selecting every child inside of the array
        {
            child.SetActive(false); // turning the object off inside the array
        }

        // Disable vibration
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        GamePad.SetVibration(PlayerIndex.Two, 0, 0);
        GamePad.SetVibration(PlayerIndex.Three, 0, 0);
        GamePad.SetVibration(PlayerIndex.Four, 0, 0);        
    }

    /*
     * Un-pauses the game and loads a random scene
     */
    public void RestartButton()
    {
        Time.timeScale = 1;
        foreach (GameObject child in m_objects) // going though and selecting every child inside of the array
        {
            child.SetActive(true); // turning the object off inside the array
        }
        SceneManager.LoadScene(Random.Range(1, SceneManager.sceneCountInBuildSettings - 2));
    }

    /*
     * Un-pauses the game and loads the main menu scene
     */
    public void MainMenuButton()
    {
        Time.timeScale = 1;
        foreach (GameObject child in m_objects) // going though and selecting every child inside of the array
        {
            child.SetActive(true); // turning the object off inside the array
        }
        SceneManager.LoadScene(0); // loading the screen up for mainmenu
    }

    /*
     * Quits the application
     */
    public void QuitButton()
    {
        Application.Quit();
    }
}
