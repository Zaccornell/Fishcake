using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: John Plant
 * Date: 2018/11/29
 */
public class HealthSliderRotation : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;       // Use relative rotation should be used for this gameobject?


    private Quaternion m_RelativeRotation;          // The local rotatation at the start of the scene.


    private void Start()
    {
        m_RelativeRotation = transform.parent.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}