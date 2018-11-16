using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainMenuSpawner : MonoBehaviour
{
    public GameObject m_antPrefab;
    public GameObject m_roachPrefab;

    public Vector2 m_spawnArea;

    public float m_antSpawnDelay;

    private float m_antSpawnTimer;

	// Use this for initialization
	void Start ()
    {
        m_antSpawnTimer = m_antSpawnDelay;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_antSpawnTimer -= Time.deltaTime;

        if (m_antSpawnTimer <= 0)
        {
            int side = Random.Range(0, 4);

            Vector3 spawnPos = new Vector3();
            switch (side)
            {
                // top
                case 0:
                    spawnPos = new Vector3(Random.Range(-m_spawnArea.x, m_spawnArea.x), transform.position.y, m_spawnArea.y);
                    break;
                // right
                case 1:
                    spawnPos = new Vector3(m_spawnArea.x, transform.position.y, Random.Range(-m_spawnArea.y, m_spawnArea.y));
                    break;
                // down
                case 2:
                    spawnPos = new Vector3(Random.Range(-m_spawnArea.x, m_spawnArea.x), transform.position.y, -m_spawnArea.y);
                    break;
                // left
                case 3:
                    spawnPos = new Vector3(-m_spawnArea.x, transform.position.y, Random.Range(-m_spawnArea.y, m_spawnArea.y));
                    break;
            }

            NavMeshHit hit = new NavMeshHit();
            NavMesh.SamplePosition(spawnPos, out hit, 5, -1);

            GameObject ant = Instantiate(m_antPrefab, hit.position, Quaternion.Euler(0, 0, 0));
            ant.GetComponent<MainMenuAnt>().SetTarget(-spawnPos);

            m_antSpawnTimer = m_antSpawnDelay;
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_spawnArea.x, 1, m_spawnArea.y));
    }
}
