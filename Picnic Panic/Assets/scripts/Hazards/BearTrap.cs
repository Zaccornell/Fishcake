using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    public float m_duration;

    private MovingActor m_affectedActor;
    private Vector3 m_position;
    private float m_originalSpeed;
    private float m_timer;
    private bool m_activated = false;
    private bool m_playerTrapped;

    private Animator m_animator;
	// Use this for initialization
	void Start ()
    {
        m_animator = GetComponent<Animator>();
	}

    /*
     * Handles stopping the affected actor's rigidbody from moving
     */
    private void FixedUpdate()
    {
        if (m_affectedActor != null)
        {
            m_affectedActor.RigidBody.position = m_position;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        m_timer -= Time.deltaTime;

        // when the timer runs out free the player and run the open anim
        if (m_timer <= 0 && m_playerTrapped)
        {
            m_affectedActor.m_speed = m_originalSpeed;
            m_animator.SetTrigger("Open");
            m_playerTrapped = false;
            m_affectedActor = null;
        }

        if (m_activated)
        {
            m_activated = m_animator.GetBool("Active");
        }
    }

    /*
     * Handles trapping players that enter the trap's trigger
     */
    private void OnTriggerEnter(Collider other)
    {
        if (!m_activated && other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                m_affectedActor = player;
                m_position = player.RigidBody.position;

                m_originalSpeed = player.m_speed;
                player.m_speed = 0;
                m_affectedActor = player;
                m_timer = m_duration;
                m_activated = true;
                m_playerTrapped = true;

                m_animator.SetBool("Active", true);
            }
        }
    }
}
