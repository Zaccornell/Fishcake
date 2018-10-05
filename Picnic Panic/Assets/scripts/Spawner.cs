using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Actor[] m_players = null;
    public GameObject m_enemyPrefab = null;
    public int m_health;
    public float m_spawnDelay;
    public float m_spawnJitter;
    public int m_maxSpawns;
    public float m_attackRange;
    public float m_attackSpeed;
    [HideInInspector] public List<GameObject> m_enemies = new List<GameObject>();

    private float m_timer;
    private int m_currentSpawns;

	// Use this for initialization
	void Start ()
    {
        RandomizeDelay();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0.0f && m_currentSpawns < m_maxSpawns)
        {
            RandomizeDelay();

            Vector3 spawnPosition = new Vector3();

            Rect playArea = new Rect(new Vector2(-25, -25), new Vector2(50, 50));

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
            spawnPosition.y = 0.5f;

            GameObject newEnemy = Instantiate(m_enemyPrefab, spawnPosition, new Quaternion());
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();
            enemyScript.m_players = m_players;
            enemyScript.m_spawner = this;
            enemyScript.m_maxHealth = m_health;
            enemyScript.m_attackRange = m_attackRange;
            enemyScript.m_attackSpeed = m_attackSpeed;

            m_enemies.Add(newEnemy);
            m_currentSpawns++;
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
    }
}
