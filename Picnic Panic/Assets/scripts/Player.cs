using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.UI;


/*
 * Author: Bradyn Corkill / John Plant
 * Date: 2018/10/3
 */

public class Player : MovingActor
{
    public bool m_showDebug;
    public float m_knockBackDistance; // to able the designer to give a value how far the knock back will be 
    public float m_knockbackCooldown;
    public float m_dashStrengthMin;
    public float m_dashStrengthMax;
    public float m_timeToFull;
    public float m_dashCooldown;
    public bool m_friendlyFire;
    public float m_attackDistance;
    public float m_attackRadius;
    public float m_attackHeightOffset;
    public float m_invulLength;
    public Renderer m_facing;
    public int m_playerNumber;
    public GameObject m_corpsePrefab;
    public HUD m_hud;
    public float m_vibrationLength;
    public float m_vibrationDeath;
    public float m_virbationRespawn;
    public int m_neededKills;
    public int m_healAmount;
    public float m_healRadius;
    public ParticleSystem m_healParticles;
    public AudioClip[] m_missAttacks;
    public AudioClip[] m_hitAttacks;
    public AudioClip[] m_playerDamage;
    public AudioClip[] m_playerAttack;
    public AudioClip[] m_playerDash;
    public AudioSource m_audioSource;
    public Text m_dashStrengthDisplay;
    public Text m_playerNumberDisplay;


    private XboxController m_controller;
    private float m_dashTimer;
    private bool m_attackPressed = false;
    private bool m_canRespawn = false;
    private float m_knockbackTimer;
    private float m_invulTimer;
    private bool m_invulToggle;
    private float m_vibrationTimer;
    private bool m_vibrationToggle;
    private int m_killCount;

    private bool m_dashing;
    private float m_dashStrength;

    public bool CanRespawn
    {
        get { return m_canRespawn; }
    }
    public XboxController Controller
    {
        get { return m_controller; }
    }

    // Use this for initialization
    void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_movement = new Vector3();
        m_health = m_maxHealth;
        m_alive = true;
        m_dashStrength = m_dashStrengthMin;

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

        // getting the button X to call a Funtion
        if (XCI.GetButtonDown(XboxButton.X, m_controller))
        {
            Playerheal();
        }

        // When dash button is pressed        
        if ((Input.GetKeyDown(KeyCode.Space) || XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) == 1) && m_dashTimer < 0)
        {
            m_dashing = true;
            m_dashStrengthDisplay.enabled = true;
        }
        // when dash button is held
        if (m_dashing && XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) == 1)
        {
            m_dashStrength += (Time.deltaTime / m_timeToFull) * (m_dashStrengthMax - m_dashStrengthMin);
            if (m_dashStrength > m_dashStrengthMax)
            {
                m_dashStrength = m_dashStrengthMax;
            }
        }
        // when dash button is let go
        if (m_dashing && XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) == 0)
        {
            if (m_movement.magnitude != 0)
            {
                Vector3 dashVelocity = Vector3.Scale(m_movement, m_dashStrength * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                m_dashTimer = m_dashCooldown;
                if (m_playerDash.Length > 0)
                {
                    int index = Random.Range(0, m_playerDash.Length);
                    if (m_playerDash[index] != null)
                    {
                        m_audioSource.PlayOneShot(m_playerDash[index]);
                    }
                }
               

                m_invulTimer = m_invulLength;
                Physics.IgnoreLayerCollision(8, 9, true);
            }

            m_dashing = false;
            m_dashStrength = m_dashStrengthMin;
            m_dashStrengthDisplay.enabled = false;
        }

        if (m_dashing)
        {
            m_dashStrengthDisplay.text = m_dashStrength.ToString("#.0#");            
        }

        // Attack
        if ((Input.GetMouseButtonDown(0) || XCI.GetAxisRaw(XboxAxis.RightTrigger, m_controller) != 0 || XCI.GetButtonDown(XboxButton.A, m_controller)) && m_attackTimer <= 0)
        {
            if (m_attackPressed == false)
            {
                Vector3 height = transform.position;
                height.y += m_attackHeightOffset;
                //m_audioSource.PlayOneShot(m_playerAttack[Random.Range(0, m_playerAttack.Length)]);
                Collider[] hits = Physics.OverlapSphere(height + transform.forward * m_attackDistance, m_attackRadius);
                List<GameObject> targets = new List<GameObject>();   
                
                foreach (Collider current in hits)
                {
                    bool unique = true;
                    foreach (GameObject target in targets)
                    {
                        if (target == current.gameObject)
                        {
                            unique = false;
                        }
                    }

                    if (unique)
                    {
                        targets.Add(current.gameObject);
                    }
                }
                bool AttackHit = false;
                foreach (GameObject current in targets)
                {
                    if (current.tag == "Enemy")
                    {
                      
                         current.GetComponent<MovingActor>().TakeDamage(m_attackDamage, this);
                      
                        AttackHit = true;
                        if (!current.GetComponent<MovingActor>().Alive)
                        {
                            m_killCount++; // adding a plus one to kill count 
                            if (m_killCount > m_neededKills) // checking if the kill count is above the max amount
                            {
                                m_killCount = m_neededKills; // setting the kill count to the max amount
                            }
                            if (m_killCount >= m_neededKills)
                            {
                                m_healParticles.Play();
                            }
                        }     
                    }
                    if (m_friendlyFire && current != gameObject && current.tag == "Player")
                    {
                    
                        current.GetComponent<Player>().TakeDamage(m_attackDamage, this);
                    }
                }
                if (AttackHit)
                {
                    if (m_hitAttacks.Length > 0)
                    {
                        int index = Random.Range(0, m_hitAttacks.Length);
                        if (m_hitAttacks[index] != null)
                        {
                            m_audioSource.PlayOneShot(m_hitAttacks[index]);

                        }
                    }
                   
                }
                else
                {
                    if (m_missAttacks.Length > 0)
                    {
                        int index = Random.Range(0, m_missAttacks.Length);
                        if (m_missAttacks[index] != null)
                        {
                            m_audioSource.PlayOneShot(m_missAttacks[index]);

                        }
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

    /*
     * Handles rigid body movement
     */
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
    }

    /*
     * Causes the player to take damage
     * Params: 
     *          Damage: the amount of damage to take
     *          Attacker: the actor that attacked the player
     */
    public override void TakeDamage(int damage, Actor attacker)
    {
        // if invulnerability is turned off
        if(!PlayerOptions.Instance.m_invulToggle)
        {
            // if the player has not been hit recently
            if (m_invulTimer <= 0)
            {
                m_health -= damage;
                if (m_playerDamage.Length > 0)
                {
                    int index = Random.Range(0, m_playerDamage.Length);
                    if (m_playerDamage[index] != null)
                    {
                        m_audioSource.PlayOneShot(m_playerDamage[index]);

                    }
                }
               
                if (PlayerOptions.Instance.m_vibrationToggle)
                {
                     GamePad.SetVibration((PlayerIndex)m_playerNumber -1, 100, 100); //. set the vibration stregnth 
                }
                m_vibrationTimer = m_vibrationLength; // sets the timers for the vibration


                // once you get hit by the enemy you get knocked back
                // giving us the feel of our players getting hit in game
                if (m_rigidBody.velocity.magnitude <= 0.1f && m_knockbackTimer <= 0)
                {
                    Vector3 dashVelocity = Vector3.Scale((gameObject.transform.position - attacker.gameObject.transform.position).normalized, m_knockBackDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                    m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                    m_knockbackTimer = m_knockbackCooldown;
                }
                // if the player has died
                if (m_health <= 0)
                {

                    m_killCount = 0; // resetting kill count once die
                    m_healParticles.Stop(); // stop particles 
                    m_health = 0;
                    m_alive = false;

                    Death();

                    Vector3 position = gameObject.transform.position;
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
        spawnPos *= 9;;
        gameObject.transform.position = spawnPos;
        m_vibrationTimer = m_virbationRespawn;
        m_alive = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer current in renderers)
        {
            current.enabled = true;
        }
        m_rigidBody.isKinematic = false;
    }
    public void Death()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer current in renderers)
        {
            current.enabled = false;
        }
        m_rigidBody.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        if (m_showDebug)
        {
            Vector3 height = transform.position;
            height.y += m_attackHeightOffset;

            Gizmos.DrawSphere(height + transform.forward * m_attackDistance, m_attackRadius);
        }
    }
    public void FallDamage(int damage)
    {
        if (m_alive)
        {
            m_health -= damage;
            if (PlayerOptions.Instance.m_vibrationToggle)
            {
                GamePad.SetVibration((PlayerIndex)m_playerNumber - 1, 100, 100); //. set the vibration stregnth 
            }
            if (m_health <= 0)
            {
                m_healParticles.Stop(); // stop particles 
                m_health = 0;
                m_alive = false;

                Death();

                Vector3 position = gameObject.transform.position;
                position.y -= 0.5f;
                Instantiate(m_corpsePrefab, position, gameObject.transform.rotation);
                m_canRespawn = m_hud.UseLife();
                m_vibrationTimer = m_vibrationDeath;
            }
        }
    }

    // Healing Player and other players around them 
    private void Playerheal()
    {
        if (m_killCount >= m_neededKills) // checking to see if the amount of kills 
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, m_healRadius); // getting the Players in the Radius around them 
            foreach (Collider item in targets) // looping though all the targets found in the radius
            {
                if (item.tag == "Player")// checking to see if the targets are players
                {
                    item.GetComponent<Player>().ShareHealing(); // Running the ShareHealing Funtion in each Player class
                }
               
            }
            m_killCount -= m_neededKills; // removing amount of kills from kill count
        }
        m_healParticles.Stop(); // stop particles 
    }

    // healing the player
    public void ShareHealing()
    {
        m_health += m_healAmount; // adding the amount of health 
        if (m_health >= m_maxHealth) // checking to see if the health is more than max health
        {
            m_health = m_maxHealth;// if so setting the health to max health 
        }
    }

    public void DisplayPlayerNumber()
    {
        m_playerNumberDisplay.text = m_playerNumber.ToString();
    }
}
