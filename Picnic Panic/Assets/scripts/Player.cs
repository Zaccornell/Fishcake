using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: John Plant
 * Date: 2018/10/3
 */

public class Player : MovingActor
{
    public float m_dashDistance;
    public float m_dashCooldown;
    public Renderer m_facing;
    public int m_playerNumber;

    private ParticleSystem m_dashParticle = null;
    private List<Collider> m_enemies = new List<Collider>();
    private float m_dashTimer;
    private string m_horizontalAxis;
    private string m_verticalAxis;
    private string m_attackButton;
    private string m_functionalX;
    private string m_functionalY;
    private string m_dashButton;
    private bool m_attackPressed = false;

    // Use this for initialization
    void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_dashParticle = GetComponentInChildren<ParticleSystem>();
        m_movement = new Vector3();
        m_health = m_maxHealth;

        if (m_playerNumber == 1)
        {
            m_horizontalAxis = "Horizontal" + m_playerNumber;
            m_verticalAxis = "Vertical" + m_playerNumber;
            m_attackButton = "Attack" + m_playerNumber;
            m_functionalX = "Functional Direction X" + m_playerNumber;
            m_functionalY = "Functional Direction Y" + m_playerNumber;
            m_dashButton = "Dash" + m_playerNumber;
        }
        else
        {
            m_horizontalAxis = "HorizontalKB";
            m_verticalAxis = "VerticalKB";
            m_attackButton = "Attack" + m_playerNumber;
            m_functionalX = "Functional Direction X" + m_playerNumber;
            m_functionalY = "Functional Direction Y" + m_playerNumber;
            m_dashButton = "Dash" + m_playerNumber;
        }
	}

    // Update is called once per frame
    void Update ()
    {
        m_dashTimer -= Time.deltaTime;
        m_attackTimer -= Time.deltaTime;

        m_movement.z = Input.GetAxisRaw(m_verticalAxis);
        m_movement.x = Input.GetAxisRaw(m_horizontalAxis);

        
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw(m_dashButton) == 1) && m_dashTimer < 0)
        {
            Vector3 dashVelocity = Vector3.Scale(m_movement, m_dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
            m_rigidBody.AddForce(dashVelocity * m_rigidBody.mass);
            m_dashTimer = m_dashCooldown;

            m_dashParticle.Play();
        }

        if ((Input.GetButtonDown("Fire1") || Input.GetAxisRaw(m_attackButton) != 0) && m_attackTimer <= 0)
        {
            if (m_attackPressed == false)
            {
                for (int i = 0; i < m_enemies.Count; i++)
                {
                    if (m_enemies[i] != null)
                    {
                        m_enemies[i].GetComponent<Enemy>().TakeDamage(10, this);
                    }
                    else
                    {
                        m_enemies.RemoveAt(i);
                    }
                }
                m_facing.material.color = new Color(1, 0, 0);
                m_canAttack = false;
                m_attackTimer = m_attackSpeed;
                m_attackPressed = true;
            }
        }
        if (Input.GetAxisRaw(m_attackButton) == 0)
        {
            m_attackPressed = false;
        }
        if (m_attackTimer < 0 && !m_canAttack)
        {
            m_facing.material.color = new Color(1, 1, 1);
            m_canAttack = true;
        }
    }

    private void FixedUpdate()
    {
        m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));

        Vector3 functional = new Vector3(Input.GetAxis(m_functionalX), 0, Input.GetAxis(m_functionalY));

        if (functional.magnitude > 0)
        {
            m_rigidBody.rotation = Quaternion.LookRotation(functional.normalized);
        }
        else if (m_movement.magnitude > 0)
        {
            m_rigidBody.rotation = Quaternion.LookRotation(m_movement.normalized);
        }

        if (m_dashParticle.isPlaying && m_rigidBody.velocity.magnitude < 1)
        {
            if (m_rigidBody.velocity.magnitude != 0)
            {
                m_dashParticle.Stop();
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            m_enemies.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            m_enemies.Remove(other);
        }
    }

    public override void TakeDamage(int damage, Actor attacker)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            //Destroy(gameObject);
            m_alive = false;
            gameObject.SetActive(false);
        }
    }
}
