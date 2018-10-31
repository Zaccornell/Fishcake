using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float m_shakeDuration;
    public float m_cooldownLength;
    public float m_shakeAmount;

    private float m_shakeTimer;
    private float m_cooldownTimer;
    private Vector3 m_originalPosition;
	// Use this for initialization
	void Start ()
    {
        m_originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_cooldownTimer > 0)
        {
            m_cooldownTimer -= Time.deltaTime;
        }

		if (m_shakeTimer > 0)
        {
            transform.position = m_originalPosition + Random.insideUnitSphere * m_shakeAmount;

            m_shakeTimer -= Time.deltaTime;
        }
        else
        {
            m_shakeTimer = 0f;
            transform.position = m_originalPosition;
        }
	}

    public void StartShake()
    {
        if (m_cooldownTimer <= 0)
        {
            m_shakeTimer = m_shakeDuration;
            m_cooldownTimer = m_cooldownLength;
        }
    }
}
