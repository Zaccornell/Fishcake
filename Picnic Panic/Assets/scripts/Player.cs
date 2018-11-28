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
    #region Variables
    #region Public
    public float m_knockBackDistance; // to able the designer to give a value how far the knock back will be 
    public float m_knockbackCooldown;
    public float m_dashStrengthMin;
    public float m_dashStrengthMax;
    public float m_timeToFull;
    public float m_dashCooldown;
    public bool m_instantDash;
    public int m_playerNumber;
    public GameObject m_corpsePrefab;
    public HUD m_hud;
    public Slider m_healthSlider;
    public Collider m_weaponCollider;
    #region Vibration
    public float m_vibrationLength;
    public float m_vibrationDeath;
    public float m_virbationRespawn;
    #endregion
    public int m_neededKills;
    public int m_healAmount;
    public float m_healRadius;
    public GameObject m_healZonePrefab;
    public ParticleSystem m_healParticles;
    #region Music
    public AudioClip[] m_missAttacks;
    public AudioClip[] m_hitAttacks;
    public AudioClip[] m_playerDamage;
    public AudioClip[] m_playerAttack;
    public AudioClip[] m_playerDash;
    public AudioClip[] m_playerFall;
    public AudioClip[] m_playerdie;
    public AudioClip[] m_playerHeals;
    public AudioSource m_audioSourceSFX;
    #endregion
    public Text m_dashStrengthDisplay;
    public Image m_playerNumberDisplay;
    public int m_healCount;
    public Slider m_healReady;
    public bool m_respawnOnRoundEnd;
    public float m_respawnLength;
    #endregion

    #region Private
    private XboxController m_controller;
    private float m_dashTimer;
    private bool m_attackPressed = false;
    private bool m_canRespawn = false;
    private float m_knockbackTimer;
    private float m_invulTimer;
    private float m_vibrationTimer;
    private int m_killCount;
    private Collider m_collider;

    private bool m_dashing;
    private float m_dashStrength;

    private int m_kills;
    private int m_deaths;
    private int m_antKills;
    private int m_roachKills;
    private int m_healsUsed;
    private float m_healLerpTimer;
    private bool m_countUp;
    private float m_respawnTimer;
    #endregion
    #endregion

    #region Get/Set
    public bool CanRespawn
    {
        get { return m_canRespawn; }
    }
    public XboxController Controller
    {
        get { return m_controller; }
    }
    public int Kills
    {
        get { return m_kills; }
    }
    public int Deaths
    {
        get { return m_deaths; }
    }
    public int AntKills
    {
        get { return m_antKills; }
    }
    public int RoachKills
    {
        get { return m_roachKills; }
    }
    public int HealsUsed
    {
        get { return m_healsUsed; }
    }
    public bool DashReady
    {
        get { return m_dashTimer <= 0; }
    }
    #endregion

    // Use this for initialization
    void Start ()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
        m_movement = new Vector3();
        m_health = m_maxHealth;
        m_alive = true;
        m_dashStrength = m_dashStrengthMin;
        m_healthSlider.maxValue = m_maxHealth;
        m_healReady.maxValue = m_neededKills;
        m_healReady.value = 0;

        m_controller = (XboxController)m_playerNumber;
    }

    // Update is called once per frame
    void Update ()
    {
        // Timers
        m_vibrationTimer -= Time.deltaTime;
        m_attackTimer -= Time.deltaTime;
        m_knockbackTimer -= Time.deltaTime;
        m_invulTimer -= Time.deltaTime;
        m_dashTimer -= Time.deltaTime;
        if (!m_respawnOnRoundEnd)
            m_respawnTimer -= Time.deltaTime;
        //

        m_healthSlider.value = m_health;

        m_movement.z = XCI.GetAxisRaw(XboxAxis.LeftStickY, m_controller);
        m_movement.x = XCI.GetAxisRaw(XboxAxis.LeftStickX, m_controller);
        m_movement.Normalize();

        m_animator.SetFloat("Character Walk", m_movement.magnitude);
        m_animator.SetFloat("Rotation", (Vector3.Dot(transform.forward, m_movement) + 1) / 2);

        // getting the button X to call a Funtion
        if (XCI.GetButtonDown(XboxButton.X, m_controller))
        {
            Playerheal();
        }

        // If the dash as a charge up or is instant
        if (m_instantDash)
        {
            // if the player presses the dash button
            if ((Input.GetKeyDown(KeyCode.Space) || XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) == 1) && m_dashTimer < 0)
            {
                // if the player is moving
                if (m_movement.magnitude != 0)
                {
                    // get the dash velocity and apply it to the player
                    Vector3 dashVelocity = Vector3.Scale(m_movement, m_dashStrengthMax * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                    m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                    m_dashTimer = m_dashCooldown;

                    // Dash sounds
                    if (m_playerDash.Length > 0)
                    {
                        int index = Random.Range(0, m_playerDash.Length);
                        if (m_playerDash[index] != null)
                        {
                            m_audioSourceSFX.PlayOneShot(m_playerDash[index]);
                        }
                    }
                }
            }
        }
        else
        {
            // When dash button is pressed        
            if ((Input.GetKeyDown(KeyCode.Space) || XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) > 0) && m_dashTimer < 0)
            {
                m_dashing = true;
                m_dashStrengthDisplay.enabled = true;
            }
            // when dash button is held
            if (m_dashing && XCI.GetAxisRaw(XboxAxis.LeftTrigger, m_controller) > 0)
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
                // if the player is moving
                if (m_movement.magnitude != 0)
                {
                    // get the dash velocity and apply it to the player
                    Vector3 dashVelocity = Vector3.Scale(m_movement, m_dashStrength * new Vector3((Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * m_rigidBody.drag + 1)) / -Time.deltaTime)));
                    m_rigidBody.AddForce(dashVelocity, ForceMode.VelocityChange);
                    m_dashTimer = m_dashCooldown;

                    // Sounds
                    if (m_playerDash.Length > 0)
                    {
                        int index = Random.Range(0, m_playerDash.Length);
                        if (m_playerDash[index] != null)
                        {
                            m_audioSourceSFX.PlayOneShot(m_playerDash[index]);
                        }
                    }
                }

                m_dashing = false;
                m_dashStrength = m_dashStrengthMin;
                m_dashStrengthDisplay.enabled = false;
            }
        }

        // when the dash button is held
        if (m_dashing)
        {
            m_dashStrengthDisplay.text = m_dashStrength.ToString("#.0"); // display the dash strength      
        }

        // Attack
        if ((Input.GetMouseButtonDown(0) || XCI.GetAxisRaw(XboxAxis.RightTrigger, m_controller) != 0 || XCI.GetButtonDown(XboxButton.A, m_controller)) && m_attackTimer <= 0)
        {
            // check if the player has just pressed the attack button
            if (m_attackPressed == false)
            {
                // start the attack animation
                m_animator.SetTrigger("Attack Pressed");               

                m_canAttack = false;
                m_attackTimer = m_attackSpeed;
                m_attackPressed = true;
            }
        }
        // if the user has let go of the attack button
        if (XCI.GetAxisRaw(XboxAxis.RightTrigger, m_controller) == 0)
        {
            m_attackPressed = false;
        }
        // if the player can attack again
        if (m_attackTimer < 0 && !m_canAttack)
        {
            m_canAttack = true;
        }

        // when the vibration is done
        if (m_vibrationTimer <= 0)
        {            
            GamePad.SetVibration((PlayerIndex)m_playerNumber -1, 0, 0); // stop the vibration             
        }

        // TEMP - REMOVE
        if (m_killCount >= m_neededKills)
        {
            if (m_countUp)
                m_healLerpTimer += Time.deltaTime;
            else
                m_healLerpTimer -= Time.deltaTime;
            if (m_healLerpTimer > 1 || m_healLerpTimer < 0)
                m_countUp = !m_countUp;
        }

        // respawn the player if they don't respawn on the end of the round
        if (!m_respawnOnRoundEnd && !m_alive && m_canRespawn && m_respawnTimer <= 0)
        {
            Respawn();
        }
    }

    /*
     * Handles rigid body movement and functional direction
     */
    private void FixedUpdate()
    {
        if (m_useForce)
        {
            m_rigidBody.AddForce(m_movement * m_speed * 2, ForceMode.Acceleration);
        }
        else
        {
            m_rigidBody.MovePosition(m_rigidBody.position + (m_movement * Time.deltaTime * m_speed));
        }   

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
                        m_audioSourceSFX.PlayOneShot(m_playerDamage[index]);

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
                    if (m_playerdie.Length > 0)
                    {
                        int index = Random.Range(0, m_playerdie.Length);
                        if (m_playerdie[index] != null)
                        {
                            m_audioSourceSFX.PlayOneShot(m_playerdie[index]);

                        }
                    }
                    m_killCount = 0; // resetting kill count once die
                    m_healParticles.Stop(); // stop particles 
                    m_health = 0;
                    m_alive = false;

                    Death();

                    GameObject corpse = Instantiate(m_corpsePrefab, gameObject.transform.position, gameObject.transform.rotation);
                    corpse.AddComponent<PlayerCorpse>();
                    m_canRespawn = m_hud.UseLife();
                    if (!m_respawnOnRoundEnd)
                    {
                        m_respawnTimer = m_respawnLength;
                    }
                    m_vibrationTimer = m_vibrationDeath;
                }
            }
        }
    }

    /* 
     * Resets the players values to default
     */
    public void ResetValues()
    {
        m_health = m_maxHealth;
        m_canRespawn = false;
    }

    /*
     * Handles respawnning dead players
     * Spawns them at a radius of 9 from the pieking
     * Re-enables all the colliders
     */
    public void Respawn()
    {
        if (m_canRespawn)
        {
            if (PlayerOptions.Instance.m_vibrationToggle)
            {
                GamePad.SetVibration((PlayerIndex)m_playerNumber - 1, 100, 100); //. set the vibration stregnth 
            }

            Vector3 spawnPos;
            //bool validPos = false;

            // Don't spawn in areas with items in it
            //do
            //{
                spawnPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                spawnPos.Normalize();
                spawnPos *= 9;

            //    validPos = Physics.CheckCapsule(new Vector3(spawnPos.x, 0.5f, spawnPos.z), new Vector3(spawnPos.x, 0.5f + 1.72f, spawnPos.z), 0.5f);
            //} while (!validPos);

            gameObject.transform.position = spawnPos;
            m_vibrationTimer = m_virbationRespawn;
            m_alive = true;
            m_health = m_maxHealth;
            m_canRespawn = false;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer current in renderers)
            {
                current.enabled = true;
            }
            m_collider.enabled = true;
            m_rigidBody.isKinematic = false;
            m_healthSlider.gameObject.SetActive(true);
            m_healReady.value = 0;
            m_healReady.gameObject.SetActive(true);
        }
    }

    /*
     * Handles hiding the player's renderers on death
     */
    public void Death()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer current in renderers)
        {
            current.enabled = false;
        }
        m_collider.enabled = false;
        m_rigidBody.isKinematic = true;
        m_healthSlider.gameObject.SetActive(false);
        m_healReady.gameObject.SetActive(false);

        m_deaths++;
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
                if (m_playerFall.Length > 0)
                {
                    int index = Random.Range(0, m_playerFall.Length);
                    if (m_playerFall[index] != null)
                    {
                        m_audioSourceSFX.PlayOneShot(m_playerFall[index]);

                    }
                }
                m_healParticles.Stop(); // stop particles 
                m_health = 0;
                m_alive = false;

                Death();

                GameObject corpse = Instantiate(m_corpsePrefab, gameObject.transform.position, gameObject.transform.rotation);
                corpse.AddComponent<PlayerCorpse>();
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
            m_healCount++;
            m_healsUsed++;

            if (m_playerHeals.Length > 0)
            {
                int index = Random.Range(0, m_playerHeals.Length);
                if (m_playerHeals[index] != null)
                {
                    m_audioSourceSFX.PlayOneShot(m_playerHeals[index]);

                }
            }
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = 0;
            Instantiate(m_healZonePrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));

            m_killCount -= m_neededKills; // removing amount of kills from kill count
            m_healParticles.Stop(); // stop particles
            m_healParticles.Clear();
            m_healReady.value = 0;
        }
    }
    

    public void RestoreHealth(int amount)
    {
        m_health += amount;
        if (m_health >= m_maxHealth) // checking to see if the health is more than max health
        {
            m_health = m_maxHealth;// if so setting the health to max health 
        }
    }

    public void DisplayPlayerNumber(Sprite sprite)
    {
        m_playerNumberDisplay.sprite = sprite;
    }

    /*
     * Handles the player's attack
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            MovingActor enemy = other.gameObject.GetComponent<MovingActor>();
            if (enemy != null)
            {
                enemy.TakeDamage(m_attackDamage, this);
                if (!enemy.Alive)
                {
                    m_killCount++; // adding a plus one to kill count 
                    m_healReady.value = m_killCount;
                    m_kills++;
                    if (m_killCount > m_neededKills) // checking if the kill count is above the max amount
                    {
                        m_killCount = m_neededKills; // setting the kill count to the max amount
                    }
                    if (m_killCount >= m_neededKills)
                    {
                        m_healParticles.Play();
                    }

                    if (enemy.m_type == Type.Ant)
                    {
                        m_antKills++;
                    }
                    else if (enemy.m_type == Type.Cockroach)
                    {
                        m_roachKills++;
                    }
                }
            }
        }
        // if friendly fire is on
        if (PlayerOptions.Instance.m_firendlyFire && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().TakeDamage(m_attackDamage, this);
        }
    }

    /*    
     * Enables and disables the weapon for the attack animation
     */
    void EnableWeapon()
    {
        m_weaponCollider.enabled = true;
    }
    void DisableWeapon()
    {
        m_weaponCollider.enabled = false;
    }
}
