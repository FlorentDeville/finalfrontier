using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourScript : MonoBehaviour
{

    float m_angle = 0;
    Vector3 m_initialPosition;
	// Use this for initialization
	void Start ()
    {
        m_angle = 0;
        m_initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        const float SPEED = 5f;

        m_angle = m_angle + SPEED * Time.fixedDeltaTime;

        transform.SetPositionAndRotation(m_initialPosition, Quaternion.identity);

        transform.Rotate(Vector3.forward, m_angle);
        transform.Translate(Vector3.up * 600);

        transform.LookAt(Vector3.zero);
    }
}
