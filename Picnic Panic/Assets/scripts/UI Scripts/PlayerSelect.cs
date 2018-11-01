using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
public class PlayerSelect : MonoBehaviour
{
    public Text[] m_playerSlots;
    public Text m_startGame;
    public GameObject[] m_playerPrefabs;
    public Transform[] m_spawnPoints;
    public Spawner m_spawner;
    public HUD m_hud;
    public PauseMenu m_pauseMenu;
    public CameraControl m_camera;
    public MonoBehaviour[] m_gameplayScripts;
    public Image[] m_playerJoin;
    public AudioClip m_lobbyMusic;
    public AudioClip m_roundStart;
    public AudioSource m_audioSourceMusic;
    public AudioSource m_audioSourceSFX;

    private XboxController[] m_controllers;
    private KeyCode[] m_testButtons;
    private List<Player> m_players;
    private List<int> m_playerOrder;
    private List<int> m_characterIndex;
	// Use this for initialization
	void Start ()
    {
        m_players = new List<Player>();
        m_playerOrder = new List<int>();
        m_characterIndex = new List<int>();

        m_hud.gameObject.SetActive(false);

        foreach(MonoBehaviour current in m_gameplayScripts)
        {
            current.enabled = false;
        }
        m_spawner.enabled = false;
        m_hud.enabled = false;
        m_pauseMenu.enabled = false;
        m_camera.enabled = false;
        if (m_lobbyMusic != null)
        {
            m_audioSourceMusic.clip = m_lobbyMusic;
            m_audioSourceMusic.loop = true;
            m_audioSourceMusic.Play();         
        }

        m_testButtons = new KeyCode[4];
        m_testButtons[0] = KeyCode.H;
        m_testButtons[1] = KeyCode.J;
        m_testButtons[2] = KeyCode.K;
        m_testButtons[3] = KeyCode.L;

        m_controllers = new XboxController[4];
        for (int i = 0; i < 4; i++)
        {
            m_controllers[i] = (XboxController)(i + 1);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            // when a controller presses the join button
            if (XCI.GetButtonDown(XboxButton.A, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i]))
            {
                // if the controller has not already joined
                if (!m_playerOrder.Contains(i))
                {
                    m_playerSlots[m_playerOrder.Count].text = i.ToString();
                    m_playerJoin[m_playerOrder.Count].enabled = false;
                    m_playerOrder.Add(i); // add the current controller to the player list
                    m_characterIndex.Add(i > m_playerPrefabs.Length - 1 ? m_playerPrefabs.Length - 1 : i);
                    m_startGame.enabled = false;
                }
            }
        }

        
        foreach (int current in m_playerOrder)
        {
            if (XCI.GetButtonDown(XboxButton.DPadUp, m_controllers[current]))
            {
                if ((m_characterIndex[current] += 1) > m_playerPrefabs.Length - 1)
                {
                    m_characterIndex[current] = 0;
                }
                m_playerSlots[current].text = m_characterIndex[current].ToString();
            }
            if (XCI.GetButtonDown(XboxButton.DPadDown, m_controllers[current]))
            {
                if ((m_characterIndex[current] -= 1) < 0)
                {
                    m_characterIndex[current] = m_playerPrefabs.Length - 1;                    
                }
                m_playerSlots[current].text = m_characterIndex[current].ToString();
            }
        }

        // if the players have joined and have readied up
        if (m_playerOrder.Count != 0)
        {
            m_startGame.enabled = true;

            // When player one presses the start button
            if (XCI.GetButtonDown(XboxButton.Start, m_controllers[m_playerOrder[0]]) || Input.GetKeyDown(m_testButtons[m_playerOrder[0]]))
            {
                // instantiate each player
                for (int i = 0; i < m_playerOrder.Count; i++)
                {
                    GameObject currentPlayer = Instantiate(m_playerPrefabs[m_characterIndex[i]], m_spawnPoints[i].position, m_spawnPoints[i].rotation);
                    Player playerScript = currentPlayer.GetComponent<Player>();

                    playerScript.m_playerNumber = m_playerOrder[i] + 1;
                    playerScript.m_hud = m_hud;
                    playerScript.m_audioSourceSFX = m_audioSourceSFX;

                    playerScript.DisplayPlayerNumber();

                    m_players.Add(playerScript);                    
                }

                // give objects access to the players
                m_spawner.m_players = m_players.ToArray();
                m_hud.m_players = m_players.ToArray();
                m_pauseMenu.m_players = m_players.ToArray();

                List<Actor> cameraTargets = new List<Actor>();
                foreach (Actor current in m_camera.m_Targets)
                {
                    cameraTargets.Add(current);
                }
                foreach (Player current in m_players)
                {
                    cameraTargets.Add(current);
                }
                m_camera.m_Targets = cameraTargets.ToArray();

                foreach (MonoBehaviour current in m_gameplayScripts)
                {
                    current.enabled = true;
                }
                m_spawner.enabled = true;
                m_hud.enabled = true;
                m_pauseMenu.enabled = true;
                m_camera.enabled = true;

                m_hud.gameObject.SetActive(true);
                m_audioSourceMusic.loop = false;
                m_audioSourceMusic.Stop();
                if (m_roundStart != null)
                {
                    m_audioSourceMusic.PlayOneShot(m_roundStart);
                 
                }
                this.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }

}
