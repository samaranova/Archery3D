using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody arrowRb;

    void Start()
    {
        arrowRb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If an arrow hits a target, destroy the arrow
        if (collision.gameObject.tag == "Target")
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        // Continuously add a force to the arrow to simulate a real arrows physics that falls arrow head first
        arrowRb.AddForceAtPosition(arrowRb.velocity * -0.2f, transform.TransformPoint(0, -0.3f, 0));
    }
}
