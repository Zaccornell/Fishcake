using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.AI;
/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/5
 */

public delegate void MyDel();

public class Spawner : MonoBehaviour
{
    public bool m_showDebug;
    public float m_roundLength;
    public float m_roundEndBuffer;

    // Ant
    public int[] m_antCount;
    private float m_antSpawnTimer;
    private float m_antSpawnDelay;
    private int m_antToSpawn;

    // Cockroach
    public int[] m_cockroachCount;
    private float m_cockroachSpawnTimer;
    private float m_cockroachSpawnDelay;
    private int m_cockroachToSpawn;

    public Actor[] m_players = null;
    public PieKing m_king = null;
    public GameObject m_antPrefab = null;
    public GameObject m_cockroachPrefab = null;
    public HUD m_hud;
    public Vector2 m_spawnArea;
    public float m_randomSpawnRadius;
    public float m_spawnJitter;
    public AudioSource m_audioSourceSFX;
    public AudioClip[] m_kingLaugh;
    public AudioClip[] m_roundStarter;


    private List<MovingActor> m_enemies = new List<MovingActor>();
    private int m_currentRound;
    private float m_roundTimer; 
    private int m_enemySpawned;
    private bool m_noDelay;


    public float RoundTimer
    {
        get { return m_roundTimer; }
    }
    public int EnemyTotal
    {
        get { return m_enemySpawned + m_antToSpawn + m_cockroachToSpawn; }
    }
    public int CurrentRound
    {
        get { return m_currentRound; }
    }
    public List<MovingActor> Enemies
    {
        get { return m_enemies; }
    }
    public bool NoDelay
    {
        set
        {
            m_noDelay = value;
            if (value == true)
            {
                m_antSpawnDelay = 0.1f;
                m_antSpawnTimer = 0.1f;
                m_cockroachSpawnDelay = 0.3f;
                m_cockroachSpawnTimer = 0.3f;
            }
        }
    }
    public event MyDel OnRoundEnd;

	// Use this for initialization
	void Start ()
    {
        CalculateDelay();

        m_antSpawnTimer = m_antSpawnDelay;
        m_cockroachSpawnTimer = m_cockroachSpawnDelay;

        // Adds the set amount of enemies for the current round + an extra 25% for each player after 2
        m_antToSpawn += m_antCount[m_currentRound] + Mathf.RoundToInt(m_antCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned
        m_cockroachToSpawn += m_cockroachCount[m_currentRound] + Mathf.RoundToInt(m_cockroachCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned

        m_roundTimer = m_roundLength; // setting the timer for the round 
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Timers
        m_roundTimer -= Time.deltaTime; 
        m_antSpawnTimer -= Time.deltaTime;
        m_cockroachSpawnTimer -= Time.deltaTime;
        //

        // if no delay is turned on
        if (m_noDelay)
        {
            Vector3 spawnPosition = new Vector3();

            // Pick a random side of the map
            int side = Random.Range(1, 5);
            // Spawn position is randomly chosen on along the chosen side
            switch (side)
            {
                case 1:
                    spawnPosition.x = m_spawnArea.x / 2;
                    spawnPosition.z = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    break;
                case 2:
                    spawnPosition.x = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    spawnPosition.z = m_spawnArea.y / -2;
                    break;
                case 3:
                    spawnPosition.x = m_spawnArea.x / -2;
                    spawnPosition.z = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    break;
                case 4:
                    spawnPosition.x = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    spawnPosition.z = m_spawnArea.y / 2;
                    break;
            }

            // Spawn ant
            if (m_antSpawnTimer <= 0.0f)
            {
                m_antSpawnTimer = m_antSpawnDelay;

                NavMeshHit hit = new NavMeshHit();
                if (NavMesh.SamplePosition(spawnPosition, out hit, 5, -1))
                {
                    // Spawn the enemy prefab at the determined location
                    GameObject newEnemy = Instantiate(m_antPrefab, spawnPosition, Quaternion.LookRotation((m_king.transform.position - spawnPosition).normalized));
                    // Allocate needed values in the enemy script
                    Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                    enemyScript.Players = m_players;
                    enemyScript.Spawner = this;
                    enemyScript.King = m_king;
                    enemyScript.m_audioSourceSFX = m_audioSourceSFX;

                    m_enemies.Add(enemyScript);
                    m_antToSpawn--; // removing the limit to spanw 
                    m_enemySpawned++; // adding what has been spawned 
                }
            }

            // Spawn cockroach
            if (m_cockroachSpawnTimer <= 0.0f)
            {
                m_cockroachSpawnTimer = m_cockroachSpawnDelay;

                NavMeshHit hit = new NavMeshHit();
                if (NavMesh.SamplePosition(spawnPosition, out hit, 5, -1))
                {
                    // Spawn the cockroach prefab at the determined location
                    GameObject newCockroach = Instantiate(m_cockroachPrefab, hit.position, Quaternion.LookRotation((m_king.transform.position - spawnPosition).normalized));
                    // Allocate needed values in the enemy script
                    Cockroach cockroachScript = newCockroach.GetComponent<Cockroach>();
                    cockroachScript.Spawner = this;
                    cockroachScript.King = m_king;

                    m_enemies.Add(cockroachScript);
                    m_cockroachToSpawn--;
                    m_enemySpawned++;
                }
            }
        }
        else
        {
            // checking to see if there is no enemies to spawn and the timer is higher then 5 seconds 
            if ((m_antToSpawn <= 0 && m_cockroachToSpawn <= 0) && m_roundTimer > 5.0f && m_enemySpawned <= 0)
            {
                m_roundTimer = 5.0f; // setting the timer to 5 secounds 
                if (m_kingLaugh.Length > 0)
                {
                    m_audioSourceSFX.PlayOneShot(m_kingLaugh[Random.Range(0, m_kingLaugh.Length)]);
                }
            }

            // round system
            // if the round timer is finished 
            if (m_roundTimer <= 0.0f)
            {
                if (m_roundStarter.Length > 0)
                {
                    int index = Random.Range(0, m_roundStarter.Length);
                    if (m_roundStarter[index] != null)
                    {
                        m_audioSourceSFX.PlayOneShot(m_roundStarter[index]);
                    }
                }
                m_roundTimer = m_roundLength; // start the round timer
                // checking to see if it doesn't go over the limit
                if (m_currentRound + 1 < m_antCount.Length)
                    m_currentRound++; // add to the current round
                
                m_antToSpawn += m_antCount[m_currentRound] + Mathf.RoundToInt(m_antCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned
                m_cockroachToSpawn += m_cockroachCount[m_currentRound] + Mathf.RoundToInt(m_cockroachCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned

                OnRoundEnd(); // event call
                CalculateDelay();
            }

            // Spawn ant
            if (m_antSpawnTimer <= 0.0f && m_antToSpawn > 0)
            {
                m_antSpawnTimer = m_antSpawnDelay; // start the ant spawn timer

                Vector3 spawnPosition = GetSpawnPosition(); // get the spawn position

                NavMeshHit hit = new NavMeshHit();
                // if the spawn position is valid
                if (NavMesh.SamplePosition(spawnPosition, out hit, 5, -1))
                {
                    // Spawn the enemy prefab at the determined location
                    GameObject newEnemy = Instantiate(m_antPrefab, spawnPosition, Quaternion.LookRotation((m_king.transform.position - spawnPosition).normalized));
                    // Allocate needed values in the enemy script
                    Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                    enemyScript.Players = m_players;
                    enemyScript.Spawner = this;
                    enemyScript.King = m_king;
                    enemyScript.m_audioSourceSFX = m_audioSourceSFX;

                    m_enemies.Add(enemyScript);
                    m_antToSpawn--; // removing the limit to spanw 
                    m_enemySpawned++; // adding what has been spawned 
                }
            }

            // Spawn cockroach
            if (m_cockroachSpawnTimer <= 0.0f && m_cockroachToSpawn > 0)
            {
                m_cockroachSpawnTimer = m_cockroachSpawnDelay; // start the cockroach spawn timer

                Vector3 spawnPosition = GetSpawnPosition(); // get the spawn position

                NavMeshHit hit = new NavMeshHit();
                // if the spawn position is valid
                if (NavMesh.SamplePosition(spawnPosition, out hit, 5, -1))
                {
                    // Spawn the cockroach prefab at the determined location
                    GameObject newCockroach = Instantiate(m_cockroachPrefab, hit.position, Quaternion.LookRotation((m_king.transform.position - spawnPosition).normalized));
                    // Allocate needed values in the enemy script
                    Cockroach cockroachScript = newCockroach.GetComponent<Cockroach>();
                    cockroachScript.Spawner = this;
                    cockroachScript.King = m_king;

                    m_enemies.Add(cockroachScript);
                    m_cockroachToSpawn--;
                    m_enemySpawned++;
                }
            }
        }
	}

    /*
     * Controls the length between enemy spawns
     * Will ensure all enemies spawn between the start of the round and the end of round buffer
     */
    private void CalculateDelay()
    {
        // delay = time / amount
        m_antSpawnDelay = (m_roundLength - m_roundEndBuffer) / (m_antCount[m_currentRound] + Mathf.RoundToInt(m_antCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)));
        m_antSpawnTimer = m_antSpawnDelay;
        m_cockroachSpawnDelay = (m_roundLength - m_roundEndBuffer) / (m_cockroachCount[m_currentRound] + Mathf.RoundToInt(m_cockroachCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)));
        m_cockroachSpawnTimer = m_cockroachSpawnDelay;
    }
    
    /*
     * Calculate the spawn position for the enemies
     * enemies spawn on the edge of a set rectangle
     * returns the spawn position as a vector3
     */
    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = new Vector3();
        Vector3 playerPosition = new Vector3();

        int playerCount = 0;
        // get average position of all the players
        foreach (Actor current in m_players)
        {
            if (current.Alive)
            {
                playerPosition += current.transform.position;
                playerCount++;
            }
        }
        playerPosition /= playerCount;

        // if average position is not within a specified radius of the king
        if (playerPosition.magnitude >= m_randomSpawnRadius)
        {
            // Get opposite direction of the players
            Vector2 direction = new Vector2(-playerPosition.x, -playerPosition.z);
            direction += new Vector2(direction.y, -direction.x) * Random.Range(-m_spawnJitter, m_spawnJitter);
            direction.Normalize();

            // get the angle of the direction
            float angle = Mathf.Atan2(direction.y, direction.x);
            angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

            float baseAngle = angle;

            // Get the angles that point to the corner of the rect
            float[] cornerAngles = new float[4];
            cornerAngles[0] = Mathf.Atan2(m_spawnArea.y / 2, m_spawnArea.x / 2);
            cornerAngles[0] = (cornerAngles[0] > 0 ? cornerAngles[0] : (2 * Mathf.PI + cornerAngles[0])) * 360 / (2 * Mathf.PI);

            cornerAngles[1] = Mathf.Atan2(m_spawnArea.y / 2, -m_spawnArea.x / 2);
            cornerAngles[1] = (cornerAngles[1] > 0 ? cornerAngles[1] : (2 * Mathf.PI + cornerAngles[1])) * 360 / (2 * Mathf.PI);

            cornerAngles[2] = Mathf.Atan2(-m_spawnArea.y / 2, -m_spawnArea.x / 2);
            cornerAngles[2] = (cornerAngles[2] > 0 ? cornerAngles[2] : (2 * Mathf.PI + cornerAngles[2])) * 360 / (2 * Mathf.PI);

            cornerAngles[3] = Mathf.Atan2(-m_spawnArea.y / 2, m_spawnArea.x / 2);
            cornerAngles[3] = (cornerAngles[3] > 0 ? cornerAngles[3] : (2 * Mathf.PI + cornerAngles[3])) * 360 / (2 * Mathf.PI);

            // get the nearest 90 degrees to the angle
            float rightAngle = 0;
            if (baseAngle > 0 && baseAngle < cornerAngles[0])
            {
                rightAngle = 0;
            }
            if (baseAngle > cornerAngles[0] && baseAngle < cornerAngles[1])
            {
                rightAngle = 90;
            }
            if (baseAngle > cornerAngles[1] && baseAngle < cornerAngles[2])
            {
                rightAngle = 180;
            }
            if (baseAngle > cornerAngles[2] && baseAngle < cornerAngles[3])
            {
                rightAngle = 270;
            }
            if (baseAngle > cornerAngles[3] && baseAngle < 360)
            {
                rightAngle = 360;
            }

            // get the angle between angle and rightAngle
            if (rightAngle > angle)
            {
                angle = rightAngle - angle;
            }
            else
            {
                angle = angle - rightAngle;
            }

            // Get the distance to the edge of the game area
            float dist = 0;
            if ((baseAngle > 0 && baseAngle < cornerAngles[0]) || (baseAngle > cornerAngles[1] && baseAngle < cornerAngles[2]) || (baseAngle > cornerAngles[3] && baseAngle < 360))
            {
                dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
            }
            else if ((baseAngle > cornerAngles[0] && baseAngle < cornerAngles[1]) || (baseAngle > cornerAngles[2] && baseAngle < cornerAngles[3]))
            {
                dist = (m_spawnArea.y / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
            }
            // the spawn position is the calculated by the using the direction and distance to find the position at the edge of the square
            direction *= dist;
            spawnPosition.x = direction.x;
            spawnPosition.z = direction.y;
        }
        else
        {
            // Pick a random side of the map
            int side = Random.Range(1, 5);

            // Spawn position is randomly chosen on along the chosen side
            switch (side)
            {
                case 1:
                    spawnPosition.x = m_spawnArea.x / 2;
                    spawnPosition.z = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    break;
                case 2:
                    spawnPosition.x = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    spawnPosition.z = m_spawnArea.y / -2;
                    break;
                case 3:
                    spawnPosition.x = m_spawnArea.x / -2;
                    spawnPosition.z = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    break;
                case 4:
                    spawnPosition.x = Random.Range(m_spawnArea.y / -2, m_spawnArea.y / 2);
                    spawnPosition.z = m_spawnArea.y / 2;
                    break;
            }
        }

        return spawnPosition;
    }

    /*
     * Removes the enemy from the enemy array
     * Params:
     *      Enemy: the enemy that has died and needs to be removed
     */
    public void EnemyDeath(MovingActor enemy)
    {
        m_enemies.Remove(enemy);
        m_enemySpawned--; // removing what has been spawned
    }

    /*
     * Resets all the values of the spawner back to what they were at the start
     */
    public void ResetValues()
    {
        m_currentRound = 0;
        m_antToSpawn = m_antCount[m_currentRound] + Mathf.RoundToInt(m_antCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned
        m_cockroachToSpawn = m_cockroachCount[m_currentRound] + Mathf.RoundToInt(m_cockroachCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned
        m_roundTimer = m_roundLength; // start the round timer
        m_enemySpawned = 0;
        CalculateDelay(); // calculate the delay between spawns
        m_antSpawnTimer = m_antSpawnDelay; // start the ant spawn timer
        m_cockroachSpawnTimer = m_cockroachSpawnDelay; // start the cockroach spawn timer

        foreach (MovingActor current in m_enemies)
        {
            Destroy(current.gameObject);
        }
        m_enemies.Clear();
    }

    /*
     * Handles drawing debug
     */
    public void OnDrawGizmos()
    {
        if (m_showDebug)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(m_spawnArea.x, 5, m_spawnArea.y));

            Vector3 avgPos = new Vector3();
            int playerCount = 0;
            foreach (Actor current in m_players)
            {
                avgPos += current.transform.position;
                playerCount++;
            }
            avgPos /= playerCount;
            Gizmos.DrawSphere(avgPos, 1);

            Vector3[] spawnAreaPositions = new Vector3[3];
            spawnAreaPositions[0] = new Vector3();
            spawnAreaPositions[1] = new Vector3();
            spawnAreaPositions[2] = new Vector3();

            Vector2[] directions = new Vector2[3];
            if (m_players.Length > 0)
                directions[0] = new Vector2(avgPos.x, avgPos.z);
            else
                directions[0] = Vector2.down;
            directions[0] = -directions[0];
            directions[0].Normalize();

            directions[1] = new Vector2(directions[0].y, -directions[0].x);
            directions[1] = directions[0] + directions[1] * m_spawnJitter;
            directions[1].Normalize();
            directions[2] = new Vector2(directions[0].y, -directions[0].x);
            directions[2] = directions[0] - directions[2] * m_spawnJitter;
            directions[2].Normalize();

            for (int i = 0; i < 3; i++)
            {
                // get the angle of the direction
                float angle = Mathf.Atan2(directions[i].y, directions[i].x);
                angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

                float baseAngle = angle; // 

                // Get the angles that point to the corner of the rect
                float[] cornerAngles = new float[4];
                cornerAngles[0] = Mathf.Atan2(m_spawnArea.y / 2, m_spawnArea.x / 2);
                cornerAngles[0] = (cornerAngles[0] > 0 ? cornerAngles[0] : (2 * Mathf.PI + cornerAngles[0])) * 360 / (2 * Mathf.PI);

                cornerAngles[1] = Mathf.Atan2(m_spawnArea.y / 2, -m_spawnArea.x / 2);
                cornerAngles[1] = (cornerAngles[1] > 0 ? cornerAngles[1] : (2 * Mathf.PI + cornerAngles[1])) * 360 / (2 * Mathf.PI);

                cornerAngles[2] = Mathf.Atan2(-m_spawnArea.y / 2, -m_spawnArea.x / 2);
                cornerAngles[2] = (cornerAngles[2] > 0 ? cornerAngles[2] : (2 * Mathf.PI + cornerAngles[2])) * 360 / (2 * Mathf.PI);

                cornerAngles[3] = Mathf.Atan2(-m_spawnArea.y / 2, m_spawnArea.x / 2);
                cornerAngles[3] = (cornerAngles[3] > 0 ? cornerAngles[3] : (2 * Mathf.PI + cornerAngles[3])) * 360 / (2 * Mathf.PI);

                // get the nearest 90 degrees to the angle
                float rightAngle = 0;
                if (baseAngle > 0 && baseAngle < cornerAngles[0])
                {
                    rightAngle = 0;
                }
                if (baseAngle > cornerAngles[0] && baseAngle < cornerAngles[1])
                {
                    rightAngle = 90;
                }
                if (baseAngle > cornerAngles[1] && baseAngle < cornerAngles[2])
                {
                    rightAngle = 180;
                }
                if (baseAngle > cornerAngles[2] && baseAngle < cornerAngles[3])
                {
                    rightAngle = 270;
                }
                if (baseAngle > cornerAngles[3] && baseAngle < 360)
                {
                    rightAngle = 360;
                }

                // get the angle between angle and rightAngle
                if (rightAngle > angle)
                {
                    angle = rightAngle - angle;
                }
                else
                {
                    angle = angle - rightAngle;
                }

                // Get the distance to the edge of the game area
                float dist = 0;
                if ((baseAngle > 0 && baseAngle < cornerAngles[0]) || (baseAngle > cornerAngles[1] && baseAngle < cornerAngles[2]) || (baseAngle > cornerAngles[3] && baseAngle < 360))
                {
                    dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
                }
                else if ((baseAngle > cornerAngles[0] && baseAngle < cornerAngles[1]) || (baseAngle > cornerAngles[2] && baseAngle < cornerAngles[3]))
                {
                    dist = (m_spawnArea.y / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
                }
                // the spawn position is the calculated by the using the direction and distance to find the position at the edge of the square
                directions[i] *= dist;
                spawnAreaPositions[i].x = directions[i].x;
                spawnAreaPositions[i].z = directions[i].y;
            }

            Gizmos.DrawLine(Vector3.zero, spawnAreaPositions[0]);
            Gizmos.DrawLine(Vector3.zero, spawnAreaPositions[1]);
            Gizmos.DrawLine(Vector3.zero, spawnAreaPositions[2]);

            Gizmos.DrawWireSphere(Vector3.zero, m_randomSpawnRadius);
        }
    }

    /*
     * Assigns the player's respawn function to the OnRoundEnd event if the players are set to spawn at the end of the round
     */
    private void OnEnable()
    {
        foreach (Actor player in m_players)
        {
            if (((Player)player).m_respawnOnRoundEnd)
            {
                OnRoundEnd += new MyDel(((Player)player).Respawn);
            }
        }
    }
}
