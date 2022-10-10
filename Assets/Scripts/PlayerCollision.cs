using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<PlayerMovement>().rb;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Collision avec un mur
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero;
            Debug.Log("Collision avec un mur (Trigger)");
            Debug.Log(rb.velocity);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision avec un mur
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero;
            Debug.Log("Collision avec un mur (Collision)");
            Debug.Log(rb.velocity);
        }
    }
}
