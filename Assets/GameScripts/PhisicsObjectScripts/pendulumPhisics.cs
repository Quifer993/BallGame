using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pendulumPhisics : MonoBehaviour
{
    private float speed = 1.5f;
    private float limit = 20f;
    private bool randStart = false;
    private float random = 0f;
    public float scaleAngle = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle * scaleAngle);
    }
}
