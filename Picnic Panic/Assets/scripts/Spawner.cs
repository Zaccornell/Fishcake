using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/5
 */
public class Spawner : MonoBehaviour
{
    private int m_currentRound;
    public float m_roundLength;
    public int[] m_enemyCount;
    private int m_enemyToSpawn;
    private float m_roundTimer;
    private int m_enemySpawned;

    public Actor[] m_players = null;
    public PieKing m_king = null;
    public GameObject m_enemyPrefab = null;
    public Vector2 m_spawnArea;
    public float m_spawnHeight;

    public int m_enemyHealth;
    public float m_spawnDelay;
    public float m_spawnJitter;
    public int m_maxSpawns;
    public float m_attackRange;
    public float m_attackSpeed;
    public int m_attackDamage;
    public float m_agroRange;
    public float m_loseAgroRange;
    [HideInInspector] public List<GameObject> m_enemies = new List<GameObject>();

    private float m_timer;
    private int m_currentSpawns;
    private int m_enemiesToSpawn;

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
                    Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    spawnPos.Normalize();
                    spawnPos *= 9;
                    spawnPos.y = 1;
                    ((Player)player).ResetValues();
                    player.gameObject.SetActive(true);
                    player.gameObject.transform.position = spawnPos;
                }
            }

        }
        m_timer -= Time.deltaTime;

        if (m_timer <= 0.0f && m_enemyToSpawn > 0)
        {
            RandomizeDelay();

            Vector3 spawnPosition = new Vector3();

            Rect playArea = new Rect(new Vector2(m_spawnArea.x / -2, m_spawnArea.y / -2), m_spawnArea);

            Vector3 playerPosition = new Vector3();
            foreach(Actor current in m_players)
            {
                playerPosition += current.transform.position;
            }
            playerPosition /= m_players.Length;

            Vector3 direction = -playerPosition;
            direction.y = 0;
            direction.Normalize();
            direction *= 35;

            Vector3 jitter = new Vector3(Random.value, 0, Random.value);
            jitter.Normalize();
            jitter *= 5;

            Vector3 position = direction + jitter;

            spawnPosition.x = Mathf.Min(Mathf.Max(position.x, playArea.xMin), playArea.xMax);
            spawnPosition.z = Mathf.Min(Mathf.Max(position.z, playArea.yMin), playArea.yMax);
            spawnPosition.y = m_spawnHeight;

            GameObject newEnemy = Instantiate(m_enemyPrefab, spawnPosition, new Quaternion());
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();
            enemyScript.m_players = m_players;
            enemyScript.m_spawner = this;
            enemyScript.m_maxHealth = m_enemyHealth;
            enemyScript.m_attackRange = m_attackRange;
            enemyScript.m_attackSpeed = m_attackSpeed;
            enemyScript.m_attackDamage = m_attackDamage;
            enemyScript.m_king = m_king;
            enemyScript.m_agroRange = m_agroRange;

            m_enemies.Add(newEnemy);
            m_enemyToSpawn--; // removing the limit to spanw 
            m_enemySpawned++; // adding what has been spawned 
        }
	}

    private void RandomizeDelay()
    {
        m_timer = m_spawnDelay + (m_spawnJitter * Random.Range(-1, 1));
    }

    public void EnemyDeath(GameObject enemy)
    {
        m_currentSpawns--;
        m_enemies.Remove(enemy);
        m_enemySpawned--; // removing what has been spawned
    }
}
