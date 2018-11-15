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
    public Material[] m_playerMaterials;
    public GameObject[] m_playerDisplays;
    public Transform[] m_spawnPoints;
    public GameObject[] m_playerNumberCovers;
    public Sprite[] m_playerNumberSprites;
    public Spawner m_spawner;
    public HUD m_hud;
    public PauseMenu m_pauseMenu;
    public EndGame m_endGame;
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
    //private List<int> m_playerOrder;
    private bool[] m_playersReady;
	// Use this for initialization
	void Start ()
    {
        m_players = new List<Player>();
        //m_playerOrder = new List<int>();
        m_playersReady = new bool[4];

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
        bool canStart = true;

        for (int i = 0; i < 4; i++)
        {
            // when a controller presses the join button
            if (XCI.GetButtonDown(XboxButton.A, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i]))
            {
                if (!m_playersReady[i])
                {
                    m_playerSlots[i].gameObject.SetActive(false);
                    m_playerNumberCovers[i].SetActive(false);
                    m_playerJoin[i].enabled = false;
                    m_playersReady[i] = true;
                    m_startGame.enabled = false;

                    canStart = false;

                    m_playerDisplays[i].SetActive(true);
                }
            }
        }

        int playersReady = 0;
        for (int i = 0; i < m_playersReady.Length; i++)
        {
            if (m_playersReady[i])
            {             
                playersReady++;
            }
        }

        // if the players have joined and have readied up
        if (playersReady != 0 && canStart)
        {
            m_startGame.enabled = true;

            bool start = false;
            for (int i = 0; i < m_playersReady.Length; i++)
            {
                if (m_playersReady[i] && (XCI.GetButtonDown(XboxButton.Start, m_controllers[i]) || Input.GetKeyDown(m_testButtons[i])))
                {
                    start = true;
                }
            }

            // When player one presses the start button
            if (start)
            {
                // instantiate each player
                for (int i = 0; i < m_playersReady.Length; i++)
                {
                    if (m_playersReady[i])
                    {
                        GameObject currentPlayer = Instantiate(m_playerPrefabs[0], m_spawnPoints[i].position, m_spawnPoints[i].rotation);
                        SkinnedMeshRenderer[] tmp = currentPlayer.GetComponentsInChildren<SkinnedMeshRenderer>();
                        SkinnedMeshRenderer tmp2 = currentPlayer.GetComponentInChildren<SkinnedMeshRenderer>();
                        tmp2.material = m_playerMaterials[i];
                        Player playerScript = currentPlayer.GetComponent<Player>();

                        playerScript.m_playerNumber = i + 1;
                        playerScript.m_hud = m_hud;
                        playerScript.m_audioSourceSFX = m_audioSourceSFX;

                        if (m_playerNumberSprites.Length > 0)
                        {
                            playerScript.DisplayPlayerNumber(m_playerNumberSprites[Mathf.Max(0, m_playerNumberSprites.Length - 1)]);
                        }

                        m_players.Add(playerScript);
                    }
                }

                // give objects access to the players
                m_spawner.m_players = m_players.ToArray();
                m_hud.m_players = m_players.ToArray();
                m_pauseMenu.m_players = m_players.ToArray();
                m_endGame.m_players = m_players.ToArray();

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
                foreach (GameObject current in m_playerDisplays)
                {
                    current.SetActive(false);
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
