using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/3
 */

public class Player : MovingActor
{
    public float m_knockBackDistance; // to able the designer to give a value how far the knock back will be 
    public float m_knockbackCooldown;
    public float m_dashDistance;
    public float m_dashCooldown;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_invulLength;
    public Renderer m_facing;
    public int m_playerNumber;
    public GameObject m_corpsePrefab;
    public HUD m_hud;

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
    private bool m_canRespawn = false;
    private float m_knockbackTimer;
    private float m_invulTimer;

    public bool CanRespawn
    {
        get { return m_canRespawn; }
    }

    // Use this for initialization
    void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_dashParticle = GetComponentInChildren<ParticleSystem>();
        m_movement = new Vector3();
        m_health = m_maxHealth;

        if (m_playerNumber == 0)
        {
            m_horizontalAxis = "HorizontalKB";
            m_verticalAxis = "VerticalKB";
            m_attackButton = "Attack1";
            m_functionalX = "Functional Direction X1";
            m_functionalY = "Functional Direction Y1";
            m_dashButton = "Dash1";
        }
        else
        {
            m_horizontalAxis = "Horizontal" + m_playerNumber;
            m_verticalAxis = "Vertical" + m_playerNumber;
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
        m_knockbackTimer -= Time.deltaTime;
        m_invulTimer -= Time.deltaTime;

        m_movement.z = Input.GetAxisRaw(m_verticalAxis);
        m_movement.x = Input.GetAxisRaw(m_horizontalAxis);

        
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw(m_dashButton) == 1) && m_dashTimer < 0)
        {
            if (m_movement.magnitude != 0)
            {
                Vector3 dashVelocity = Vector3.Scale(m_movement, m_dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                m_dashTimer = m_dashCooldown;

                m_invulTimer = m_invulLength;
                Physics.IgnoreLayerCollision(8, 9, true);
            }
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetAxisRaw(m_attackButton) != 0) && m_attackTimer <= 0)
        {
            if (m_attackPressed == false)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
                //List<GameObject> objects
                foreach (Collider current in hits)
                {
                    if (current.gameObject.tag == "Enemy")
                    {
                        current.gameObject.GetComponent<Enemy>().TakeDamage(m_attackDamage, this);
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

        if (m_invulTimer <= 0)
        {
            Physics.IgnoreLayerCollision(8, 9, false);
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

    public override void TakeDamage(int damage, Actor attacker)
    {
        if (m_invulTimer <= 0)
        {
            m_health -= damage;
            if (m_rigidBody.velocity.magnitude <= 0.1f && m_knockbackTimer <= 0)
            {
                // once you get hit by the enemy you get knocked back
                // giving us the feel of our players getting hit in game
                Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                m_knockbackTimer = m_knockbackCooldown;
            }
            if (m_health <= 0)
            {
                m_health = 0;
                //Destroy(gameObject);
                m_alive = false;
                gameObject.SetActive(false);
                Vector3 position = gameObject.transform.position;
                position.y -= 0.5f;
                Instantiate(m_corpsePrefab, position, gameObject.transform.rotation);
                m_canRespawn = m_hud.UseLife();
            }
        }
    }

    public void ResetValues()
    {
        m_health = m_maxHealth;
        m_canRespawn = false;
    }

    public void Respawn()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        spawnPos.Normalize();
        spawnPos *= 9;
        spawnPos.y = 1;
        gameObject.SetActive(true);
        gameObject.transform.position = spawnPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }
}
