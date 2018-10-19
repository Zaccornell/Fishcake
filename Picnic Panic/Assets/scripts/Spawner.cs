﻿using System.Collections;
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
    public float m_randomSpawnRadius;
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

        m_enemyToSpawn += m_enemyCount[m_currentRound] + Mathf.RoundToInt(m_enemyCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned
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

            m_enemyToSpawn += m_enemyCount[m_currentRound] + Mathf.RoundToInt(m_enemyCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0)); // adding the limit that needs to be spawned

            foreach (Actor player in m_players)
            {
                if (!player.gameObject.activeSelf || !player.Alive)
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

            Vector3 playerPosition = new Vector3();

            int playerCount = 0;
            foreach(Actor current in m_players)
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
        m_enemyToSpawn = m_enemyCount[m_currentRound] + Mathf.RoundToInt(m_enemyCount[m_currentRound] * 0.25f * (m_players.Length > 2 ? m_players.Length - 2 : 0));
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
}
