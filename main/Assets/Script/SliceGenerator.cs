using UnityEngine;
using System.Collections;

public class SliceGenerator : MonoBehaviour 
{
    public float m_Length;
    public float m_Radius;
    public int m_SideCount;
    public Transform m_GroundPrefab;


	// Use this for initialization
	void Start () 
    {
        float angleIncrement = 360f / m_SideCount;
        float radHalfAngle = angleIncrement * Mathf.Deg2Rad * 0.5f;
        float length = m_Radius * Mathf.Tan((float)radHalfAngle) * 2;

        for(int i = 0; i < m_SideCount; ++i)
        {
            Transform groundTransform = Instantiate(m_GroundPrefab);
            Vector3 angles = groundTransform.eulerAngles;
            angles.z = (float)angleIncrement * i;
            Vector3 scale = new Vector3((float)length * 0.1f, 1, m_Length);

            groundTransform.eulerAngles = angles;
            groundTransform.position = groundTransform.up * -m_Radius;
            groundTransform.localScale = scale;
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
