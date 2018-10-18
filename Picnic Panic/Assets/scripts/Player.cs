using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

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
    public float m_vibrationLength;
    public float m_vibrationDeath;
    public float m_virbationRespawn;

    private XboxController m_controller;
    private ParticleSystem m_dashParticle = null;
    private float m_dashTimer;
    private bool m_attackPressed = false;
    private bool m_canRespawn = false;
    private float m_knockbackTimer;
    private float m_invulTimer;
    private bool m_invulToggle;
    private float m_vibrationTimer;
    private bool m_vibrationToggle;

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
        m_alive = true;

        m_controller = (XboxController)m_playerNumber;
    }

    // Update is called once per frame
    void Update ()
    {
        m_vibrationTimer -= Time.deltaTime;
        m_dashTimer -= Time.deltaTime;
        m_attackTimer -= Time.deltaTime;
        m_knockbackTimer -= Time.deltaTime;
        m_invulTimer -= Time.deltaTime;

        m_movement.z = XCI.GetAxisRaw(XboxAxis.LeftStickY, m_controller);
        m_movement.x = XCI.GetAxisRaw(XboxAxis.LeftStickX, m_controller);
        m_movement.Normalize();

        
        if ((Input.GetKeyDown(KeyCode.Space) || XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) == 1) && m_dashTimer < 0)
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

        if ((Input.GetMouseButtonDown(0) || XCI.GetAxisRaw(XboxAxis.RightTrigger, m_controller) != 0) && m_attackTimer <= 0)
        {
            if (m_attackPressed == false)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
                List<GameObject> enemies = new List<GameObject>();   
                
                foreach (Collider current in hits)
                {
                    bool unique = true;
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy == current.gameObject)
                        {
                            unique = false;
                        }
                    }

                    if (unique)
                    {
                        enemies.Add(current.gameObject);
                    }
                }

                foreach (GameObject current in enemies)
                {
                    if (current.tag == "Enemy")
                    {
                        current.GetComponent<Enemy>().TakeDamage(m_attackDamage, this);
                    }
                }
                m_facing.material.color = new Color(1, 0, 0);
                m_canAttack = false;
                m_attackTimer = m_attackSpeed;
                m_attackPressed = true;
            }
        }
        if (XCI.GetAxisRaw(XboxAxis.RightTrigger, m_controller) == 0)
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
        // counting the timer down 
        if (m_vibrationTimer <= 0)
        {
            GamePad.SetVibration((PlayerIndex)m_playerNumber -1, 0, 0); // stop the vibration 
            
        }
    }

    private void FixedUpdate()
    {
        m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));

        Vector3 functional = new Vector3(XCI.GetAxis(XboxAxis.RightStickX, m_controller), 0, XCI.GetAxis(XboxAxis.RightStickY, m_controller));

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
        if(!PlayerOptions.Instance.m_invulToggle)
        {
            if (m_invulTimer <= 0)
            {
                m_health -= damage;
                if (PlayerOptions.Instance.m_vibrationToggle)
                {
                     GamePad.SetVibration((PlayerIndex)m_playerNumber -1, 100, 100); //. set the vibration stregnth 
                }
                m_vibrationTimer = m_vibrationLength; // sets the timers for the vibration
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
                    Renderer[] renderers = GetComponentsInChildren<Renderer>();
                    foreach (Renderer current in renderers)
                    {
                        current.enabled = false;
                    }
                    Vector3 position = gameObject.transform.position;
                    position.y -= 0.5f;
                    Instantiate(m_corpsePrefab, position, gameObject.transform.rotation);
                    m_canRespawn = m_hud.UseLife();
                    m_vibrationTimer = m_vibrationDeath;
                }

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
        if (PlayerOptions.Instance.m_vibrationToggle)
        {
             GamePad.SetVibration((PlayerIndex)m_playerNumber - 1, 100, 100); //. set the vibration stregnth 
        }
        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        spawnPos.Normalize();
        spawnPos *= 9;
        spawnPos.y = 1;
        gameObject.transform.position = spawnPos;
        m_vibrationTimer = m_virbationRespawn;
        m_alive = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer current in renderers)
        {
            current.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * m_attackDistance, m_attackRadius);
    }
     public void FallDamage(int damage)
    {
        m_health -= damage;
        if (PlayerOptions.Instance.m_vibrationToggle)
        {
            GamePad.SetVibration((PlayerIndex)m_playerNumber - 1, 100, 100); //. set the vibration stregnth 
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
            m_vibrationTimer = m_vibrationDeath;
        }
    }

}
