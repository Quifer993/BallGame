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

    public float force = 0.0000000000000000001f; // сила, с которой маятник будет толкать игрока
    public Vector3 direction = Vector3.forward; // направление движения маятника
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angle = limit * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle * scaleAngle);
    }


/*    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //rb.AddForce(direction * force, ForceMode.Force);
            }
            else
            {
                Debug.Log("ge");

            }
        }
    }*/
}
