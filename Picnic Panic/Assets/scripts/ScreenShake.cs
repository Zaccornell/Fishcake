using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class ScreenShake : MonoBehaviour
{
    public float m_shakeDuration;
    public float m_cooldownLength;
    public float m_shakeAmount;
    public CameraControl m_cameraControl;

    private float m_shakeTimer;
    private float m_cooldownTimer;
    private Vector3 m_originalPosition;
	// Use this for initialization
	void Start ()
    {
        m_originalPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_cooldownTimer > 0)
        {
            m_cooldownTimer -= Time.deltaTime;
        }

        // if the shake timer is still running
		if (m_shakeTimer > 0)
        {
            transform.localPosition = m_originalPosition + Random.insideUnitSphere * m_shakeAmount; // random position on in a sphere whoose radius is defined by shake amount
                                                                                                    // set the position of the camera to the new position

            m_shakeTimer -= Time.deltaTime; // count down
        }
        else
        {
            m_shakeTimer = 0f;
            transform.localPosition = m_originalPosition; // set the position of the camera back to the original
        }
	}

    /*
     * Function to set the shake timer to start the screen shake
     * Checks the player options for if the screen shake is turned on
     * Has a cooldown between each time the function can be called
     */
    public void StartShake()
    {
        // if screen shake is turned on 
        if (PlayerOptions.Instance.m_screenShake)
        {
            // if the cool down is finished
            if (m_cooldownTimer <= 0)
            {
                m_shakeTimer = m_shakeDuration; // start the shake timer
                m_cooldownTimer = m_cooldownLength; // start the cooldown timer
            }
        }
    }
}
