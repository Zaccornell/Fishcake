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
    public int[] m_enemyCount;
    private int m_currentRound;
    private int m_enemyToSpawn;
    private float m_roundTimer; 
    private int m_enemySpawned;

    public Actor[] m_players = null;
    public PieKing m_king = null;
    public GameObject m_enemyPrefab = null;
    public Vector2 m_spawnArea;
    public float m_spawnHeight;
    public HUD m_hud;

    public int m_enemyHealth;
    public float m_spawnDelay;
    public float m_spawnJitter;
    public int m_maxSpawns;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_attackSpeed;
    public int m_attackDamage;
    public float m_agroRange;
    public float m_loseAgroRange;
    public float m_knockbackDistance;
    [HideInInspector] public List<Enemy> m_enemies = new List<Enemy>();

    private float m_timer;

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
        RandomizeDelay();

        m_enemyToSpawn += m_enemyCount[m_currentRound]; // adding the limit that needs to be spawned
        m_roundTimer = m_roundLength; // setting the timer for the round 
	}
	
	// Update is called once per frame
	void Update ()
    {
        // checking to see if there is no enemies spawned and to spawn and the timer is higher then 5 seconds 
        if (m_enemyToSpawn <= 0 && m_roundTimer > 5.0f && m_enemySpawned <= 0) 
        {
            m_roundTimer = 5.0f; // setting the timer to 5 secounds 
        }
        m_roundTimer -= Time.deltaTime; // counting down in delta time
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
        }
        m_timer -= Time.deltaTime;

        if (m_timer <= 0.0f && m_enemyToSpawn > 0)
        {
            RandomizeDelay();

            Vector3 spawnPosition = new Vector3();

            Rect playArea = new Rect(new Vector2(m_spawnArea.x / -2, m_spawnArea.y / -2), m_spawnArea);

            Vector3 playerPosition = new Vector3();

            int playerCount = 0;
            foreach(Actor current in m_players)
            {
                if (current.transform.position.magnitude >= 10)
                {
                    playerPosition += current.transform.position;
                    playerCount++;
                }
            }
            //foreach(Enemy current in m_enemies)
            //{
            //    playerPosition += current.transform.position;
            //}
            //playerPosition /= (m_players.Length * 10 + m_enemies.Count);
            if (playerCount > 0)
            {
                playerPosition /= playerCount;

                Vector3 direction = -playerPosition;
                direction.y = 0;
                direction.Normalize();

                //Vector3 jitter = new Vector3(Random.value, 0, Random.value);
                //jitter.Normalize();
                //jitter *= 0;


                //Vector3 position = direction + jitter;

                //spawnPosition.x = Mathf.Min(Mathf.Max(position.x, playArea.xMin), playArea.xMax);
                //spawnPosition.z = Mathf.Min(Mathf.Max(position.z, playArea.yMin), playArea.yMax);
                //spawnPosition.y = m_spawnHeight;

                Vector3 position = new Vector3();

                float angle = Mathf.Atan2(direction.z, direction.x);
                angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

                float rightAngle = Mathf.Round(angle / 90f) * 90;

                if (rightAngle > angle)
                {
                    angle = rightAngle - angle;
                }
                else
                {
                    angle = angle - rightAngle;
                }

                float dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
                direction *= dist;
                spawnPosition.x = direction.x;
                spawnPosition.z = direction.z;
            }
            else
            {
                System.Random rnd = new System.Random();                
                int side = rnd.Next(1, 4);

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

    private void RandomizeDelay()
    {
        m_timer = m_spawnDelay + (m_spawnJitter * Random.Range(-1, 1));
    }

    public void EnemyDeath(Enemy enemy)
    {
        m_enemies.Remove(enemy);
        m_enemySpawned--; // removing what has been spawned
    }

    public void ResetValues()
    {
        m_currentRound = 0;
        m_enemyToSpawn = m_enemyCount[m_currentRound];
        m_roundTimer = m_roundLength;
        m_enemySpawned = 0;
        RandomizeDelay();

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

            for (int i = 0; i < 20; i++)
            {
                Vector3 position = new Vector3();
                System.Random rnd = new System.Random();
                int test = rnd.Next(0, 2);
                Vector2 rand = new Vector2(Random.value, Random.value);
                if (rnd.Next(0, 2) == 1)
                    rand.x = -rand.x;
                if (rnd.Next(0, 2) == 1)
                    rand.y = -rand.y;

                rand.Normalize();

                float angle = Mathf.Atan2(rand.y, rand.x);
                angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

                float rightAngle = Mathf.Round(angle / 90f) * 90;

                bool tmp = rightAngle > angle;

                if (tmp)
                {
                    angle = rightAngle - angle;
                }
                else
                {
                    angle = angle - rightAngle;
                }

                float dist = (m_spawnArea.x / 2f) / Mathf.Cos(angle * (Mathf.PI / 180));
                rand *= dist;
                position.x = rand.x;
                position.z = rand.y;

                Gizmos.DrawSphere(position, 1);
            }
        }
    }
}
