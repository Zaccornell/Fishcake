using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/11/29
 */
public class PauseMenu : MonoBehaviour
{
    public Button m_resumeButton;
    public Button m_backButton;
    public GameObject[] m_optionMenu;
    public GameObject[] m_children;
    public Player[] m_players;
    public Spawner m_spawner;
    public PieKing m_king;
    public HUD m_hud;
    public ScreenShake m_screenShake;
    public CustomInputModule m_inputModule;
    public MonoBehaviour[] m_gameplayScripts;
    public AudioSource m_audioSource;
    public AudioClip m_lobbyMusic;

    private bool m_optionOpen;
    private bool m_toggledThisFrame;

    private bool m_active = false;
	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false; // sets the cursor to be invisible
        Cursor.lockState = CursorLockMode.Locked; // set the cursor to be lcoked int he middle
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleObjects();
        }
        if (m_active && XCI.GetButtonDown(XboxButton.Start, m_inputModule.m_controller))
        {
            ToggleObjects();

            m_inputModule.m_controller = XboxController.All;
        }
        for (int i = 1; i <= 4; i++)
        {
            if (!m_active)
            {
                if (XCI.GetButtonDown(XboxButton.Start, (XboxController)i))
                {
                    ToggleObjects();
                    
                    m_inputModule.m_controller = (XboxController)i;                                                             
                }
            }
        }
        
        // if the pause menu is active
        if (m_active)
        {
            // if the user presses the back button
            if (XCI.GetButtonDown(XboxButton.B, m_inputModule.m_controller))
            {
                // if the options menu is open
                if (m_optionOpen)
                {
                    BackButton(); // return to the pause menu
                }
                else
                {
                    ToggleObjects(); // deactivate the pause menu
                }
            }
        }

        if (m_active) // if Puase Menu is up will ...
        {
            Cursor.lockState = CursorLockMode.None; // unlock cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // keeped locked
        }

        m_toggledThisFrame = false;
	}

    /*
     * Handles resetting the level
     */
    public void ResetClick()
    {
        SceneManager.LoadScene(Random.Range(1, SceneManager.sceneCountInBuildSettings - 2));
        ToggleObjects();
    }

    /*
     * Handles quitting back to the main menu
     */
    public void QuitClick()
    {
        ToggleObjects();
        SceneManager.LoadScene(0);
    }

    /*
     * Starts the objects back up when resuming
     */
    public void ResumeClick()
    {
        ToggleObjects();
    }

    /*
     * Toggles all objects between active and inactive
     */
    private void ToggleObjects()
    {
        // stops the toggle objects from being called multiple times per frame
        if (!m_toggledThisFrame)
        {
            // if the pause menu is active
            if (m_active)
            {
                // play the hud's music
                m_hud.m_audioSource.loop = false;
                m_hud.m_audioSource.Stop();
            }
            else
            {
                // if the lobby music has been assigned
                if (m_lobbyMusic != null)
                {
                    // play the lobby music
                    m_audioSource.loop = true;
                    m_audioSource.clip = m_lobbyMusic;
                    m_audioSource.Play();
                }
            }
            // disable vibration
            foreach (Player current in m_players)
            {
                XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)current.m_playerNumber - 1, 0, 0); //. set the vibration stregnth 
            }

            // toggle all the children
            foreach (GameObject child in m_children)
            {
                child.SetActive(!m_active);
            }
            // disable the options menu
            foreach (GameObject opchild in m_optionMenu)
            {
                opchild.SetActive(false);
            }
            // toggle all the enemies
            foreach (MovingActor current in m_spawner.Enemies)
            {
                current.enabled = m_active;
            }
            // toggle any misc scipts
            foreach (MonoBehaviour script in m_gameplayScripts)
            {
                script.enabled = m_active;
            }
            // toggle all the players
            foreach (Player current in m_players)
            {
                current.enabled = m_active;
            }

            // Toggle the timescale
            if (m_active)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0f;
            }

            m_resumeButton.Select();
            m_resumeButton.OnSelect(null);
            m_king.enabled = m_active;
            m_spawner.enabled = m_active;
            m_hud.gameObject.SetActive(m_active);
            m_screenShake.enabled = m_active;
            Physics.autoSimulation = m_active;

            Cursor.visible = !m_active; // makes cursor visable
            m_active = !m_active;

            m_toggledThisFrame = true;
        }
    }

    /*
     * Handles opening up the option menu
     */
    public void OptionButton()
    {
        // turns off the pause menu's children
        foreach (GameObject child in m_children)
        {
            child.SetActive(false);
        }
        // turns on the option menu's children
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(true);
        }

        m_backButton.Select();
        m_optionOpen = true;
    }

    /*
     * Handles returning to the pause menu from the options menu
     */
    public void BackButton()
    {
        // turns on the pause menu's children
        foreach (GameObject child in m_children)
        {
            child.SetActive(true);
        }
        // turns off the option menu's children
        foreach (GameObject opchild in m_optionMenu)
        {
            opchild.SetActive(false);
        }
        m_resumeButton.Select();
        m_optionOpen = false;
    }
}
