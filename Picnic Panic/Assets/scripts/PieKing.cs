using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *Author: Bradyn Corkill / John Plant
 * Date: 2018/10/3
 */
public class PieKing : Actor
{
    public Spawner m_spawner; // calling spanwer class
    public int m_restoreHealth; // setting the amout of health to restore per round
    public ScreenShake m_shake;
    public Image m_warningBorder;
    public AudioClip[] m_damageSounds;
    public float m_damageSoundCooldown;
    public AudioSource m_audioSourceSFX;
    public Player[] m_players;
    public Material[] m_pieKingFaces;

    private Slider m_healthBar;
    private float m_damageSoundTimer;
    private MeshRenderer m_renderer;

    // Use this for initialization
    void Start ()
    {
        m_health = m_maxHealth; // setting the health to the max health 
        m_alive = true;
        m_healthBar = GetComponentInChildren<Slider>();
        m_renderer = GetComponent<MeshRenderer>();
    }

    void Awake()
    {
        m_spawner.OnRoundEnd += new MyDel(RoundEnd); // Adding the RoundEnd function to the spawner's OnRoundEnd event
    }

    // Update is called once per frame
    void Update ()
    {
        m_damageSoundTimer -= Time.deltaTime;

        List<float> distance = new List<float>();
        for (int i = 0; i < m_players.Length; i++)
        {
            distance.Add((transform.position - m_players[i].transform.position).sqrMagnitude);
        }
        int lowestIndex = 0;
        for (int i = 1; i < m_players.Length; i++)
        {
            if (distance[i] < distance[lowestIndex])
            {
                lowestIndex = i;
            }
        }

        Vector3 direction = (m_players[lowestIndex].transform.position - transform.position).normalized;

        // get the angle of the direction
        float angle = Mathf.Atan2(direction.z, direction.x);
        angle = (angle > 0 ? angle : (2 * Mathf.PI + angle)) * 360 / (2 * Mathf.PI);

        if (angle > 0 && angle <= 90)
        {
            m_renderer.material = m_pieKingFaces[0];
        }
        else if (angle > 90 && angle <= 180)
        {
            m_renderer.material = m_pieKingFaces[1];
        }
        else if (angle > 180 && angle <= 270)
        {
            m_renderer.material = m_pieKingFaces[2];
        }
        else if (angle > 270 && angle <= 360)
        {
            m_renderer.material = m_pieKingFaces[3];
        }
    }

    // calling from another source to damage the king
    public override void TakeDamage(int damage, Actor attacker)
    {
        // if god mode is not turned on
		if (!PlayerOptions.Instance.m_invulToggle)
		{
        	m_health -= damage; // taking damage 
            m_healthBar.value = m_health; // update the health bar

            if (m_damageSounds.Length > 0)
            {
                // play random sound
                int index = Random.Range(0, m_damageSounds.Length);
                if (m_damageSounds[index] != null)
                {
                    m_audioSourceSFX.PlayOneShot(m_damageSounds[index]);
                }
            }

            // start the screen shake
            if (m_shake != null)
			    m_shake.StartShake();

            // if the king was killed
            if (m_health < 0)
            {
                m_health = 0;                
            }

            // if the king's health is less than or equal to 30%
            if ((float)m_health / m_maxHealth <= 0.3)
            {
                m_warningBorder.enabled = true; // enable the warning border
            }
		}
    }

    // end of each round will call this function
    void RoundEnd()
    {
        m_health += m_restoreHealth; // restoring the health
        m_healthBar.value = m_health; // update the health bar
        if (m_health >= m_maxHealth) // if the health is higher than max health
        {
            m_health = m_maxHealth; // reset health to max health 
        }
        if ((float)m_health / m_maxHealth > 0.3)
        {
            m_warningBorder.enabled = false;
        }
    }

    /*
     * Resets the kings health back to full
     */
    public void ResetValues()
    {
        m_health = m_maxHealth;
    }
}
