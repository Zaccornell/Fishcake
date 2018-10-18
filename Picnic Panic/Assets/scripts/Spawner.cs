using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
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
    public int[] m_enemyCount;

    public Actor[] m_players = null;
    public PieKing m_king = null;
    public GameObject m_enemyPrefab = null;
    public HUD m_hud;
    public Vector2 m_spawnArea;
    public float m_spawnHeight;
    public float m_spawnJitter;

    public int m_enemyHealth;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_attackSpeed;
    public int m_attackDamage;
    public float m_agroRange;
    public float m_loseAgroRange;
    public float m_knockbackDistance;
    [HideInInspector] public List<Enemy> m_enemies = new List<Enemy>();

    private float m_spawnTimer;
    private float m_spawnDelay;
    private int m_currentRound;
    private int m_enemyToSpawn;
    private float m_roundTimer; 
    private int m_enemySpawned;

    public float RoundTimer
    {
        get { return m_roundTimer; }
    }
    public int EnemyTotal
    {
        get { return m_enemySpawned + m_enemyToSpawn; }
    }

    public event MyDel OnRoundEnd;

	// Use this for initialization
	void Start ()
    {
        CalculateDelay();

        m_enemyToSpawn += m_enemyCount[m_currentRound]; // adding the limit that needs to be spawned
        m_roundTimer = m_roundLength; // setting the timer for the round 
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_roundTimer -= Time.deltaTime; // counting down in delta time
        m_spawnTimer -= Time.deltaTime;

        // checking to see if there is no enemies spawned and to spawn and the timer is higher then 5 seconds 
        if (m_enemyToSpawn <= 0 && m_roundTimer > 5.0f && m_enemySpawned <= 0) 
        {
            m_roundTimer = 5.0f; // setting the timer to 5 secounds 
        }

        // round system
        if (m_roundTimer <= 0.0f)
        {
            m_roundTimer = m_roundLength; // setting the Round timer to the round length 
            // checking to see if it doesn't go over the limit
            if (m_currentRound + 1  < m_enemyCount.Length)
                m_currentRound++; // add to the current round

            m_enemyToSpawn += m_enemyCount[m_currentRound]; // adding the limit that needs to be spawned

            foreach(Actor player in m_players)
            {
                if (!player.gameObject.activeSelf)
                {
                    Player playerScript = (Player)player;

                    if (playerScript.CanRespawn)
                    {
                        playerScript.ResetValues();
                        playerScript.Respawn();
                    }
                }
            }
            OnRoundEnd();
            CalculateDelay();
        }

        if (m_spawnTimer <= 0.0f && m_enemyToSpawn > 0)
        {
            m_spawnTimer = m_spawnDelay;

            Vector3 spawnPosition = new Vector3();

            Rect playArea = new Rect(new Vector2(m_spawnArea.x / -2, m_spawnArea.y / -2), m_spawnArea);

            Vector3 playerPosition = new Vector3();

            int playerCount = 0;
            foreach(Actor current in m_players)
            {
                playerPosition += current.transform.position;
                playerCount++;
            }
            playerPosition /= playerCount;

            // if average position is not within a 10 unit radius of the king
            if (playerPosition.magnitude >= 10)
            {
                // Get opposite direction of the players
                Vector2 direction = new Vector2(-playerPosition.x, -playerPosition.z);
                direction += new Vector2(direction.y, -direction.x) * Random.Range(-m_spawnJitter, m_spawnJitter);
                direction.Normalize();

                // get the angle of the direction
                float angle = Mathf.Atan2(direction.y, direction.x);
                angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

                // get the nearest 90 degrees to the angle
                float rightAngle = Mathf.Round(angle / 90f) * 90;

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
                float dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
                // the spawn position is the calculated by the using the direction and distance to find the position at the edge of the square
                direction *= dist;
                spawnPosition.x = direction.x;
                spawnPosition.z = direction.y;
            }
            else
            {
                // Pick a random side of the map
                System.Random rnd = new System.Random();                
                int side = rnd.Next(1, 4);

                // Spawn position is randomly chosen on along the chosen side
                switch(side)
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

            // Allocate all values for the enemy
            GameObject newEnemy = Instantiate(m_enemyPrefab, spawnPosition, new Quaternion());
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();
            enemyScript.m_players = m_players;
            enemyScript.m_spawner = this;
            enemyScript.m_maxHealth = m_enemyHealth;
            enemyScript.m_attackDistance = m_attackDistance;
            enemyScript.m_attackRadius = m_attackRadius;
            enemyScript.m_attackSpeed = m_attackSpeed;
            enemyScript.m_attackDamage = m_attackDamage;
            enemyScript.m_king = m_king;
            enemyScript.m_agroRange = m_agroRange;
            enemyScript.m_knockBackDistance = m_knockbackDistance;
            enemyScript.m_height = m_spawnHeight;

            m_enemies.Add(enemyScript);
            m_enemyToSpawn--; // removing the limit to spanw 
            m_enemySpawned++; // adding what has been spawned 
        }
	}

    /*
     * Controls the length between enemy spawns
     * Will ensure all enemies spawn within the allotted time
     */
    private void CalculateDelay()
    {
        m_spawnDelay = (m_roundLength - m_roundEndBuffer) / m_enemyCount[m_currentRound];
    }


    public void EnemyDeath(Enemy enemy)
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
        m_enemyToSpawn = m_enemyCount[m_currentRound];
        m_roundTimer = m_roundLength;
        m_enemySpawned = 0;
        CalculateDelay();

        foreach (Enemy current in m_enemies)
        {
            Destroy(current.gameObject);
        }
        m_enemies.Clear();

    }

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

            Vector3 spawnPos = new Vector3();
            Vector3[] tempPos = new Vector3[2];
            tempPos[0] = new Vector3();
            tempPos[1] = new Vector3();

            Vector2 direction = new Vector2(avgPos.x, avgPos.z);
            direction = -direction;
            direction.Normalize();

            Vector2[] offset = new Vector2[2];
            offset[0] = new Vector2(direction.y, -direction.x);
            offset[0] = direction + offset[0] * m_spawnJitter;
            offset[0].Normalize();
            offset[1] = new Vector2(direction.y, -direction.x);
            offset[1] = direction - offset[1] * m_spawnJitter;
            offset[1].Normalize();

            // get the angle of the direction
            float angle = Mathf.Atan2(direction.y, direction.x);
            angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

            // get the nearest 90 degrees to the angle
            float rightAngle = Mathf.Round(angle / 90f) * 90;

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
            float dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
            // the spawn position is the calculated by the using the direction and distance to find the position at the edge of the square
            direction *= dist;
            spawnPos.x = direction.x;
            spawnPos.z = direction.y;

            for (int i = 0; i < 2; i++)
            {
                // get the angle of the direction
                float angle2 = Mathf.Atan2(offset[i].y, offset[i].x);
                angle2 = (angle2 > 0 ? angle2 : (2 * Mathf.PI + angle2)) * 360 / (2 * Mathf.PI);

                // get the nearest 90 degrees to the angle
                float rightAngle2 = Mathf.Round(angle2 / 90f) * 90;

                // get the angle between angle and rightAngle
                if (rightAngle2 > angle2)
                {
                    angle2 = rightAngle2 - angle2;
                }
                else
                {
                    angle2 = angle2 - rightAngle2;
                }

                // Get the distance to the edge of the game area
                float dist2 = (m_spawnArea.x / 2f) / Mathf.Cos(angle2 * (Mathf.PI / 180));
                // the spawn position is the calculated by the using the direction and distance to find the position at the edge of the square
                offset[i] *= dist2;
                tempPos[i].x = offset[i].x;
                tempPos[i].z = offset[i].y;
            }

            Gizmos.DrawLine(Vector3.zero, spawnPos);
            Gizmos.DrawLine(Vector3.zero, tempPos[0]);
            Gizmos.DrawLine(Vector3.zero, tempPos[1]);

            //for (int i = 0; i < 20; i++)
            //{
            //    Vector3 position = new Vector3();
            //    System.Random rnd = new System.Random();
            //    int test = rnd.Next(0, 2);
            //    Vector2 rand = new Vector2(Random.value, Random.value);
            //    if (rnd.Next(0, 2) == 1)
            //        rand.x = -rand.x;
            //    if (rnd.Next(0, 2) == 1)
            //        rand.y = -rand.y;

            //    rand.Normalize();

            //    float angle = Mathf.Atan2(rand.y, rand.x);
            //    angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

            //    float rightAngle = Mathf.Round(angle / 90f) * 90;

            //    bool tmp = rightAngle > angle;

            //    if (tmp)
            //    {
            //        angle = rightAngle - angle;
            //    }
            //    else
            //    {
            //        angle = angle - rightAngle;
            //    }

            //    float dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
            //    rand *= dist;
            //    position.x = rand.x;
            //    position.z = rand.y;

            //    Gizmos.DrawSphere(position, 1);
            //}
        }
    }
}
