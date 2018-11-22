using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    public float m_waitLength = 5;
    public float m_disappearLength = 5;

    private float m_waitTimer;
    private float m_disappearTimer;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Animator m_animator;
	// Use this for initialization
	void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetTrigger("Death");

        m_waitTimer = m_waitLength;
        m_disappearTimer = m_disappearLength;

        m_startPos = gameObject.transform.position;
        m_endPos = gameObject.transform.position + Vector3.down;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_waitTimer -= Time.deltaTime;
        if (m_waitTimer <= 0)
        {
            m_disappearTimer -= Time.deltaTime;

            gameObject.transform.position = Vector3.Lerp(m_endPos, m_startPos, m_disappearTimer / m_disappearLength);

            if (m_disappearTimer <= 0)
            {
                Destroy(gameObject);
            }
        }


        
	}
}
